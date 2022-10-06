using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fief.DataProtector;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.OpenDrive;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Extensions;
using OpenDriveSharp;
using SQLite;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services.OpenDrive
{
    public static class OpenDriveService
    {
        private static readonly string PATH_PREFIX = $@"cloud:\{CloudStorageProvider.OpenDrive.Humanize()}\";
        private const string OPENDRIVE_DATAFILE = "opendrive.db";

        private static readonly SQLiteAsyncConnection connection;
        private static readonly DataProtectorService dataProtectorService;
        private static readonly Dictionary<int, OpenDriveClient> clients;

        static OpenDriveService()
        {
            connection = new SQLiteAsyncConnection(new SQLiteConnectionString(
                Path.Combine(FileSystem.AppDataDirectory, "Data", OPENDRIVE_DATAFILE),
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                true));
            dataProtectorService = new DataProtectorService();
            clients = new Dictionary<int, OpenDriveClient>();
        }

        public static async Task Initialize()
        {
            await connection.CreateTableAsync<OpenDriveAccount>().ConfigureAwait(false);
            var accounts = await connection.Table<OpenDriveAccount>().ToListAsync().ConfigureAwait(false);
            foreach (var item in accounts)
            {
                var client = new OpenDriveClient(item.Email, await dataProtectorService.Decrypt(item.Password).ConfigureAwait(false));
                if (!string.IsNullOrEmpty(item.SessionId))
                {
                    client.SetSessionId(await dataProtectorService.Decrypt(item.SessionId).ConfigureAwait(false));
                }
                clients[item.Id] = client;
            }
        }

        private static string GetAvailableName(SQLiteConnection db, string desiredName)
        {
            var namePart = desiredName.Trim();
            var name = namePart;
            var number = 2;
            while (db.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {nameof(OpenDriveAccount)} WHERE {nameof(OpenDriveAccount.Name)} = ?", name) > 0)
            {
                name = $"{namePart} ({number})";
                number += 1;
            }
            return name;
        }

        public static async Task<IList<OpenDriveAccount>> GetAccounts()
        {
            return await connection.Table<OpenDriveAccount>().OrderBy(a => a.Name).ToListAsync().ConfigureAwait(false);
        }

        public static async Task<OpenDriveAccount> AddAccount(string name, string email, string password)
        {
            try
            {
                var client = new OpenDriveClient(email, password);
                var sessionId = await client.GetSessionId().ConfigureAwait(false);
                var encryptedSessionId = !string.IsNullOrEmpty(sessionId) ? await dataProtectorService.Encrypt(sessionId).ConfigureAwait(false) : string.Empty;
                var encryptedPassword = await dataProtectorService.Encrypt(password).ConfigureAwait(false);
                var account = new OpenDriveAccount
                {
                    Name = name,
                    Email = email,
                    Password = encryptedPassword,
                    SessionId = encryptedSessionId
                };
                await connection.RunInTransactionAsync(db =>
                {
                    account.Name = GetAvailableName(db, account.Name);
                    db.Insert(account);
                }).ConfigureAwait(false);
                clients[account.Id] = client;
                return account;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> RenameAccount(OpenDriveAccount account, string newName)
        {
            var name = account.Name;
            await connection.RunInTransactionAsync(db =>
            {
                name = GetAvailableName(db, newName);
                db.Execute($"UPDATE {nameof(OpenDriveAccount)} SET {nameof(OpenDriveAccount.Name)} = ? " +
                    $"WHERE {nameof(OpenDriveAccount.Id)} = ?",
                    name, account.Id);
            }).ConfigureAwait(false);
            return name;
        }

        public static async Task RemoveAccount(OpenDriveAccount account)
        {
            if (clients.TryGetValue(account.Id, out var client))
            {
                var sessionId = await client.GetSessionId().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    await client.Logout(sessionId).ConfigureAwait(false);
                }
                await connection.ExecuteAsync(
                    $"DELETE FROM {nameof(OpenDriveAccount)} WHERE {nameof(OpenDriveAccount.Id)} = ?", account.Id).ConfigureAwait(false);
                clients.Remove(account.Id);
            }
        }

        public static async Task<IList<OpenDriveDrive>> GetDrives()
        {
            var drives = new List<OpenDriveDrive>();
            foreach (var item in clients)
            {
                var account = await connection.FindWithQueryAsync<OpenDriveAccount>(
                    $"SELECT * FROM {nameof(OpenDriveAccount)} WHERE {nameof(OpenDriveAccount.Id)} = ?", item.Key);
                var info = await OpenDriveHelper.GetDirectoryInfo(item.Value, "0").ConfigureAwait(false);
                drives.Add(new OpenDriveDrive(account, info as OpenDriveDriveInfo));
            }
            return drives;
        }

        public static async Task<IList<StorageItemBase>> GetItems(OpenDriveDrive drive)
        {
            var items = new List<StorageItemBase>();
            try
            {
                if (drive.Info != null && drive.Account != null && clients.TryGetValue(drive.Account.Id, out var client))
                {
                    var driveItems = await OpenDriveHelper.GetItems(client, drive.Info).ConfigureAwait(false);
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

        public static async Task<IList<StorageItemBase>> GetItems(OpenDriveFolder folder)
        {
            var items = new List<StorageItemBase>();
            try
            {
                if (folder.Info != null && folder.Account != null && clients.TryGetValue(folder.Account.Id, out var client))
                {
                    var driveItems = await OpenDriveHelper.GetItems(client, folder.Info).ConfigureAwait(false);
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

        public static async Task<OpenDriveDrive> GetDrive(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var cloudCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (cloudCrumb != null && cloudCrumb.DisplayName.Equals(CloudStorageProvider.OpenDrive.Humanize(), StringComparison.OrdinalIgnoreCase) && 
                        lastCrumb != null && lastCrumb.AppPath == AppPath.Drive)
                    {
                        var account = await connection.FindWithQueryAsync<OpenDriveAccount>(
                            $"SELECT * FROM {nameof(OpenDriveAccount)} WHERE {nameof(OpenDriveAccount.Name)} = ?", lastCrumb.DisplayName);
                        if (account != null && clients.TryGetValue(account.Id, out var client))
                        {
                            var info = await OpenDriveHelper.GetDirectoryInfo(client, "0").ConfigureAwait(false);
                            return new OpenDriveDrive(account, info as OpenDriveDriveInfo);
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

        public static async Task<OpenDriveFolder> GetFolder(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var cloudCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                    var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (cloudCrumb != null && cloudCrumb.DisplayName.Equals(CloudStorageProvider.OpenDrive.Humanize(), StringComparison.OrdinalIgnoreCase) &&
                        driveCrumb != null && lastCrumb != null)
                    {
                        var account = await connection.FindWithQueryAsync<OpenDriveAccount>(
                            $"SELECT * FROM {nameof(OpenDriveAccount)} WHERE {nameof(OpenDriveAccount.Name)} = ?", driveCrumb.DisplayName);
                        if (account != null && clients.TryGetValue(account.Id, out var client))
                        {
                            var innerPath = path.Replace(PATH_PREFIX, string.Empty).Replace("\\", "/");
                            var folderIdResult = await client.GetFolderIdByPath(innerPath);
                            if (folderIdResult.IsSuccessful && folderIdResult is FolderIdByPathResult folderId)
                            {
                                var folderInfo = await OpenDriveHelper.GetDirectoryInfo(client, folderId.FolderId);
                                return folderInfo.Convert(account) as OpenDriveFolder;
                            }
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

        public static async Task<OpenDriveFile> GetFile(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var cloudCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                    var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (cloudCrumb != null && cloudCrumb.DisplayName.Equals(CloudStorageProvider.OpenDrive.Humanize(), StringComparison.OrdinalIgnoreCase) &&
                        driveCrumb != null && lastCrumb != null)
                    {
                        var account = await connection.FindWithQueryAsync<OpenDriveAccount>(
                            $"SELECT * FROM {nameof(OpenDriveAccount)} WHERE {nameof(OpenDriveAccount.Name)} = ?", driveCrumb.DisplayName);
                        if (account != null && clients.TryGetValue(account.Id, out var client))
                        {
                            var innerPath = path.Replace(PATH_PREFIX, string.Empty).Replace("\\", "/");
                            var fileIdResult = await client.GetFileIdByPath(innerPath);
                            if (fileIdResult.IsSuccessful && fileIdResult is FileIdByPathResult fileId)
                            {
                                var fileInfo = await OpenDriveHelper.GetFileInfo(client, fileId.FileId);
                                return fileInfo.Convert(account) as OpenDriveFile;
                            }
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

        public static async Task<Stream> GetContent(OpenDriveFile file)
        {
            try
            {
                if (file?.Account != null && file?.Info != null && clients.TryGetValue(file.Account.Id, out var client))
                {
                    var contentResult = await client.Download(file.Info.Id);
                    if (contentResult.IsSuccessful && contentResult is DownloadFileResult content)
                    {
                        return content.Raw.ToMemoryStream();
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OpenDriveFolder> CreateFolder(OpenDriveDrive drive, string name, CreateOption option)
        {
            try
            {
                if (drive?.Info != null && drive?.Account != null && clients.TryGetValue(drive.Account.Id, out var client))
                {
                    var newFolder = await OpenDriveHelper.CreateFolder(client, name, drive.Info, option);
                    if (newFolder != null)
                    {
                        return newFolder.Convert(drive.Account) as OpenDriveFolder;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OpenDriveFolder> CreateFolder(OpenDriveFolder folder, string name, CreateOption option)
        {
            try
            {
                if (folder?.Info != null && folder?.Account != null && clients.TryGetValue(folder.Account.Id, out var client))
                {
                    var newFolder = await OpenDriveHelper.CreateFolder(client, name, folder.Info, option);
                    if (newFolder != null)
                    {
                        return newFolder.Convert(folder.Account) as OpenDriveFolder;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OpenDriveFile> CreateFile(OpenDriveDrive drive, string name, CreateOption option, byte[] content)
        {
            try
            {
                if (drive?.Info != null && drive?.Account != null && clients.TryGetValue(drive.Account.Id, out var client))
                {
                    var newFile = await OpenDriveHelper.CreateFile(client, name, drive.Info, option, content);
                    if (newFile != null)
                    {
                        return newFile.Convert(drive.Account) as OpenDriveFile;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<OpenDriveFile> CreateFile(OpenDriveFolder folder, string name, CreateOption option, byte[] content)
        {
            try
            {
                if (folder?.Info != null && folder?.Account != null && clients.TryGetValue(folder.Account.Id, out var client))
                {
                    var newFile = await OpenDriveHelper.CreateFile(client, name, folder.Info, option, content);
                    if (newFile != null)
                    {
                        return newFile.Convert(folder.Account) as OpenDriveFile;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Rename(OpenDriveFolder folder, string name, RenameOption option)
        {
            try
            {
                if (folder?.Info != null && folder?.Account != null && clients.TryGetValue(folder.Account.Id, out var client))
                {
                    var newName = await OpenDriveHelper.RenameFolder(client, folder.Info, name, option);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        folder.Info.Name = newName;
                    }
                    return newName;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task<string> Rename(OpenDriveFile file, string name, RenameOption option)
        {
            try
            {
                if (file?.Info != null && file?.Account != null && clients.TryGetValue(file.Account.Id, out var client))
                {
                    var newName = await OpenDriveHelper.RenameFile(client, file.Info, name, option);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        file.Info.Name = newName;
                    }
                    return newName;
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static async Task Delete(OpenDriveFolder folder)
        {
            if (folder?.Account != null && clients.TryGetValue(folder.Account.Id, out var client))
            {
                await client.TrashFolder(new List<string> { folder.Info.Id });
            }
        }

        public static async Task Delete(OpenDriveFile file)
        {
            if (file?.Account != null && clients.TryGetValue(file.Account.Id, out var client))
            {
                await client.TrashFile(new List<string> { file.Info.Id });
            }
        }

        public static string GetUrl(OpenDriveFile file)
        {
            return file?.Info?.WebLink;
        }

    }
}
