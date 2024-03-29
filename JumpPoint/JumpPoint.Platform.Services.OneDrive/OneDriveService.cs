﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
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

        private static string GetAvailableName(SQLiteConnection db, string desiredName)
        {
            var namePart = desiredName.Trim();
            var name = namePart;
            var number = 2;
            while (db.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Name)} = ?", name) > 0)
            {
                name = $"{namePart} ({number})";
                number += 1;
            }
            return name;
        }

        public static async Task<IList<OneDriveAccount>> GetAccounts()
        {
            return await connection.Table<OneDriveAccount>().OrderBy(a => a.Name).ToListAsync();
        }

        public static async Task<OneDriveAccount> AddAccount()
        {
            var accounts = await clientApp.GetAccountsAsync().ConfigureAwait(false);
            var account = await clientApp.GetNewAccount().ConfigureAwait(false);
            if (account != null && !accounts.Any(a => string.Equals(a.HomeAccountId.Identifier, account.HomeAccountId.Identifier)))
            {
                var graphClient = clientApp.GetGraphServiceClient(account);
                var displayName = await graphClient.GetDisplayName().ConfigureAwait(false);
                graphClients[account.HomeAccountId.Identifier] = graphClient;
                var oneDriveAccount = new OneDriveAccount
                {
                    Identifier = account.HomeAccountId.Identifier,
                    Name = displayName ?? "Personal",
                    Email = account.Username
                };
                await connection.RunInTransactionAsync(db =>
                {
                    oneDriveAccount.Name = GetAvailableName(db, oneDriveAccount.Name);
                    db.Insert(oneDriveAccount);
                }).ConfigureAwait(false);
                return oneDriveAccount;
            }
            return null;
        }

        public static async Task<string> RenameAccount(OneDriveAccount account, string newName)
        {
            var name = account.Name;
            await connection.RunInTransactionAsync(db =>
            {
                name = GetAvailableName(db, newName);
                db.Execute($"UPDATE {nameof(OneDriveAccount)} SET {nameof(OneDriveAccount.Name)} = ? " +
                    $"WHERE {nameof(OneDriveAccount.Identifier)} = ?",
                    name, account.Identifier);
            }).ConfigureAwait(false);
            return name;
        }

        public static async Task RemoveAccount(OneDriveAccount account)
        {
            var a = await clientApp.GetAccountAsync(account.Identifier).ConfigureAwait(false);
            await clientApp.RemoveAsync(a).ConfigureAwait(false);
            await connection.ExecuteAsync(
                $"DELETE FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Identifier)} = ?", account.Identifier).ConfigureAwait(false);
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

        public static async Task<IList<StorageItemBase>> GetItems(OneDriveFolder folder)
        {
            var items = new List<StorageItemBase>();
            try
            {
                if (folder.Account != null && graphClients.TryGetValue(folder.Account.Identifier, out var graphClient))
                {
                    var driveItems = await graphClient.GetDriveItems(folder.GraphItem);
                    foreach (var item in driveItems)
                    {
                        var i = item.Convert(folder.Account);
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
                    var cloudCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (cloudCrumb != null && cloudCrumb.DisplayName.Equals(CloudStorageProvider.OneDrive.Humanize(), StringComparison.OrdinalIgnoreCase) &&
                        lastCrumb != null && lastCrumb.AppPath == AppPath.Drive)
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
                    var cloudCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                    var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (cloudCrumb != null && cloudCrumb.DisplayName.Equals(CloudStorageProvider.OneDrive.Humanize(), StringComparison.OrdinalIgnoreCase) &&
                        driveCrumb != null && lastCrumb != null)
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
                    var cloudCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                    var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (cloudCrumb != null && cloudCrumb.DisplayName.Equals(CloudStorageProvider.OneDrive.Humanize(), StringComparison.OrdinalIgnoreCase) &&
                        driveCrumb != null && lastCrumb != null)
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

        public static async Task<Stream> GetContent(OneDriveFile file)
        {
            try
            {
                var crumbs = file.Path.GetBreadcrumbs();
                var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                var lastCrumb = crumbs.LastOrDefault();
                if (driveCrumb != null && lastCrumb != null)
                {
                    var account = await connection.FindWithQueryAsync<OneDriveAccount>(
                            $"SELECT * FROM {nameof(OneDriveAccount)} WHERE {nameof(OneDriveAccount.Name)} = ?", driveCrumb.DisplayName);
                    if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                    {
                        var content = await graphClient.GetContent(file.GraphItem.Id);
                        return content;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OneDriveFolder> CreateFolder(OneDriveDrive drive, string name, CreateOption option)
        {
            return await InternalCreateFolder(drive.Account, drive.GraphItem.Root.Id, name, option);
        }

        public static async Task<OneDriveFolder> CreateFolder(OneDriveFolder folder, string name, CreateOption option)
        {
            return await InternalCreateFolder(folder.Account, folder.GraphItem.Id, name, option);
        }

        private static async Task<OneDriveFolder> InternalCreateFolder(OneDriveAccount account, string id, string name, CreateOption option)
        {
            try
            {
                if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                {
                    var newFolder = await graphClient.CreateFolder(id, name, option);
                    if (newFolder != null)
                    {
                        return newFolder.Convert(account) as OneDriveFolder;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OneDriveFile> CreateFile(OneDriveDrive drive, string name, CreateOption option, byte[] content)
        {
            return await InternalCreateFile(drive.Account, drive.GraphItem.Root.Id, name, option, content);
        }

        public static async Task<OneDriveFile> CreateFile(OneDriveFolder folder, string name, CreateOption option, byte[] content)
        {
            return await InternalCreateFile(folder.Account, folder.GraphItem.Id, name, option, content);
        }

        private static async Task<OneDriveFile> InternalCreateFile(OneDriveAccount account, string id, string name, CreateOption option, byte[] content)
        {
            try
            {
                if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                {
                    var newFile = await graphClient.CreateFile(id, name, option, content);
                    if (newFile != null)
                    {
                        return newFile.Convert(account) as OneDriveFile;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Rename(OneDriveFolder folder, string name, RenameOption option)
        {
            return await InternalRename(folder.Account, folder.GraphItem.Id, name, option);
        }

        public static async Task<string> Rename(OneDriveFile file, string name, RenameOption option)
        {
            return await InternalRename(file.Account, file.GraphItem.Id, name, option);
        }

        private static async Task<string> InternalRename(OneDriveAccount account, string id, string name, RenameOption option)
        {
            try
            {
                if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                {
                    var newName = await graphClient.Rename(id, name, option);
                    return newName;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task Delete(OneDriveFolder folder)
        {
            await InternalDelete(folder.Account, folder.GraphItem.Id);
        }

        public static async Task Delete(OneDriveFile file)
        {
            await InternalDelete(file.Account, file.GraphItem.Id);
        }

        private static async Task InternalDelete(OneDriveAccount account, string id)
        {
            try
            {
                if (account != null && graphClients.TryGetValue(account.Identifier, out var graphClient))
                {
                    await graphClient.Delete(id);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
