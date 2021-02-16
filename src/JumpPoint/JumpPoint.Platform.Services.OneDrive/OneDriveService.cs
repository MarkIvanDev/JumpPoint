﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.OneDrive;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using SQLite;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services.OneDrive
{
    public static class OneDriveService
    {
        private const string ONEDRIVE_DATAFILE = "onedrive.db";

        private static readonly SQLiteAsyncConnection connection;
        private static readonly IPublicClientApplication clientApp;
        private static readonly Dictionary<string, GraphServiceClient> graphClients;

        static OneDriveService()
        {
            connection = new SQLiteAsyncConnection(new SQLiteConnectionString(
                Path.Combine(FileSystem.AppDataDirectory, "Data", ONEDRIVE_DATAFILE),
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                true));
            clientApp = OneDriveServiceExtensions.CreateClientApp();
            graphClients = new Dictionary<string, GraphServiceClient>();
        }

        public static async Task Initialize()
        {
            await connection.CreateTableAsync<OneDriveAccount>();
            var accounts = await clientApp.GetAccountsAsync().ConfigureAwait(false);
            foreach (var item in accounts)
            {
                var graphClient = clientApp.GetGraphServiceClient(item);
                graphClients[item.HomeAccountId.Identifier] = graphClient;
            }
        }

        private static void InsertAccount(SQLiteConnection db, OneDriveAccount account)
        {
            var name = account.Name;
            var number = 2;
            while (db.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Name)} = ?", name) > 0)
            {
                name = $"{name} ({number})";
                number += 1;
            }
            account.Name = name;
            db.Insert(account);
        }

        public static async Task<OneDriveAccount> AddAccount()
        {
            var accounts = await clientApp.GetAccountsAsync().ConfigureAwait(false);
            var account = await clientApp.GetNewAccount();
            if (account != null && !accounts.Any(a => string.Equals(a.HomeAccountId.Identifier, account.HomeAccountId.Identifier)))
            {
                var graphClient = clientApp.GetGraphServiceClient(account);
                var displayName = await graphClient.GetDisplayName();
                graphClients[account.HomeAccountId.Identifier] = graphClient;
                var oneDriveAccount = new OneDriveAccount
                {
                    Identifier = account.HomeAccountId.Identifier,
                    Name = displayName ?? "Personal",
                    Email = account.Username
                };
                await connection.RunInTransactionAsync(db => InsertAccount(db, oneDriveAccount));
                return oneDriveAccount;
            }
            return null;
        }

        public static async Task RemoveAccount(OneDriveAccount account)
        {
            var a = await clientApp.GetAccountAsync(account.Identifier).ConfigureAwait(false);
            await clientApp.RemoveAsync(a);
            await connection.ExecuteAsync($"DELETE FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Identifier)} = ?", account.Identifier);
            graphClients.Remove(account.Identifier);
        }

        public static async Task<IList<OneDriveDrive>> GetDrives()
        {
            var drives = new List<OneDriveDrive>();
            foreach (var item in graphClients)
            {
                var account = await connection.FindWithQueryAsync<OneDriveAccount>(
                    $"SELECT * FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Identifier)} = ?", item.Key);
                var drive = await item.Value.GetDrive();
                drives.Add(new OneDriveDrive(account, drive));
            }
            return drives;
        }

        public static async Task<IList<StorageItemBase>> GetItems(OneDriveDrive drive)
        {
            var items = new List<StorageItemBase>();
            try
            {
                if (drive.Account != null && graphClients.TryGetValue(drive.Account.Identifier, out var graphClient))
                {
                    var driveItems = await graphClient.GetDriveItems();
                    foreach (var item in driveItems)
                    {
                        var i = item.Convert(drive.Account);
                        if (i != null)
                        {
                            items.Add(i);
                        }
                    }
                }
                return items;
            }
            catch (Exception)
            {
                return items;
            }
        }

        public static async Task<OneDriveDrive> GetDrive(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var lastCrumb = crumbs.LastOrDefault();
                    if (lastCrumb != null && lastCrumb.PathType == PathType.Drive)
                    {
                        var account = await connection.FindWithQueryAsync<OneDriveAccount>(
                            $"SELECT * FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Name)} = ?", lastCrumb.DisplayName);
                        if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                        {
                            return new OneDriveDrive(account, await graphClient.GetDrive());
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OneDriveFolder> GetFolder(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var driveCrumb = crumbs.FirstOrDefault(c => c.PathType == PathType.Drive);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (driveCrumb != null && lastCrumb != null)
                    {
                        var account = await connection.FindWithQueryAsync<OneDriveAccount>(
                            $"SELECT * FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Name)} = ?", driveCrumb.DisplayName);
                        if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                        {
                            var itemPath = lastCrumb.Path.Remove(0, driveCrumb.Path.TrimEnd('\\').Length);
                            var driveItem = await graphClient.GetDriveItem(itemPath.Replace('\\', '/'));
                            return driveItem.Convert(account) as OneDriveFolder;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OneDriveFile> GetFile(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var driveCrumb = crumbs.FirstOrDefault(c => c.PathType == PathType.Drive);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (driveCrumb != null && lastCrumb != null)
                    {
                        var account = await connection.FindWithQueryAsync<OneDriveAccount>(
                            $"SELECT * FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Name)} = ?", driveCrumb.DisplayName);
                        if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                        {
                            var itemPath = lastCrumb.Path.Remove(0, driveCrumb.Path.TrimEnd('\\').Length);
                            var driveItem = await graphClient.GetDriveItem(itemPath.Replace('\\', '/'));
                            return driveItem.Convert(account) as OneDriveFile;
                        }
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
