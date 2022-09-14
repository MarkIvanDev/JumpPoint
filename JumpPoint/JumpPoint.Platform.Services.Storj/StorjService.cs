using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fief.DataProtector;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storj;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Extensions;
using SQLite;
using uplink.NET.Models;
using Xamarin.Essentials;

namespace JumpPoint.Platform.Services.Storj
{
    public static class StorjService
    {
        private const string STORJ_DATAFILE = "storj.db";

        private static readonly SQLiteAsyncConnection connection;
        private static readonly DataProtectorService dataProtectorService;
        private static readonly Dictionary<string, Access> accessGrants;

        static StorjService()
        {
            connection = new SQLiteAsyncConnection(new SQLiteConnectionString(
                Path.Combine(FileSystem.AppDataDirectory, "Data", STORJ_DATAFILE),
                SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex,
                true));
            dataProtectorService = new DataProtectorService();
            accessGrants = new Dictionary<string, Access>();
        }

        public static async Task Initialize()
        {
            await connection.CreateTableAsync<StorjAccount>();
            var accounts = await connection.Table<StorjAccount>().ToListAsync();
            foreach (var item in accounts)
            {
                accessGrants.Add(item.AccessGrant, new Access(await dataProtectorService.Decrypt(item.AccessGrant)));
            }
        }

        private static string GetAvailableName(SQLiteConnection db, string desiredName)
        {
            var namePart = desiredName.Trim();
            var name = namePart;
            var number = 2;
            while (db.ExecuteScalar<int>(
                $"SELECT COUNT(*) FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.Name)} = ?", name) > 0)
            {
                name = $"{namePart} ({number})";
                number += 1;
            }
            return name;
        }

        public static async Task<IList<StorjAccount>> GetAccounts()
        {
            return await connection.Table<StorjAccount>().OrderBy(a => a.Name).ToListAsync().ConfigureAwait(false);
        }

        public static async Task<StorjAccount> AddAccount(string name, string email, string accessGrant)
        {
            var encryptedAccessGrant = await dataProtectorService.Encrypt(accessGrant);
            accessGrants[encryptedAccessGrant] = new Access(accessGrant);
            var account = new StorjAccount
            {
                Name = name,
                Email = email,
                AccessGrant = encryptedAccessGrant
            };
            await connection.RunInTransactionAsync(db =>
            {
                account.Name = GetAvailableName(db, account.Name);
                db.Insert(account);
            }).ConfigureAwait(false);
            return account;
        }

        public static async Task<string> RenameAccount(StorjAccount account, string newName)
        {
            var name = account.Name;
            await connection.RunInTransactionAsync(db =>
            {
                name = GetAvailableName(db, newName);
                db.Execute($"UPDATE {nameof(StorjAccount)} SET {nameof(StorjAccount.Name)} = ? " +
                    $"WHERE {nameof(StorjAccount.AccessGrant)} = ?",
                    name, account.AccessGrant);
            }).ConfigureAwait(false);
            return name;
        }

        public static async Task RemoveAccount(StorjAccount account)
        {
            await connection.ExecuteAsync(
                $"DELETE FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.AccessGrant)} = ?", account.AccessGrant).ConfigureAwait(false);
            accessGrants.Remove(account.AccessGrant);
        }

        public static async Task<IList<StorjDrive>> GetDrives()
        {
            var drives = new List<StorjDrive>();
            foreach (var item in accessGrants)
            {
                var account = await connection.FindWithQueryAsync<StorjAccount>(
                    $"SELECT * FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.AccessGrant)} = ?", item.Key);
                drives.Add(new StorjDrive(account));
            }
            return drives;
        }

        public static async Task<IList<StorageItemBase>> GetItems(StorjDrive drive)
        {
            var items = new List<StorageItemBase>();
            try
            {
                if (drive.Account != null && accessGrants.TryGetValue(drive.Account.AccessGrant, out var accessGrant))
                {
                    var buckets = await accessGrant.GetBuckets();
                    foreach (var item in buckets)
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

        public static async Task<IList<StorageItemBase>> GetItems(StorjFolder folder)
        {
            var items = new List<StorageItemBase>();
            try
            {
                if (folder.Bucket != null && folder.Account != null && accessGrants.TryGetValue(folder.Account.AccessGrant, out var accessGrant))
                {
                    var prefix = folder.StorjObject is null ? string.Empty : folder.StorjObject.Key;
                    var objects = await accessGrant.GetObjects(folder.Bucket, prefix);
                    foreach (var item in objects)
                    {
                        var i = item.Convert(folder.Account, folder.Bucket);
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

        public static async Task<StorjDrive> GetDrive(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var lastCrumb = crumbs.LastOrDefault();
                    if (lastCrumb != null && lastCrumb.AppPath == AppPath.Drive)
                    {
                        var account = await connection.FindWithQueryAsync<StorjAccount>(
                            $"SELECT * FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.Name)} = ?", lastCrumb.DisplayName);
                        if (account != null)
                        {
                            return new StorjDrive(account);
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

        public static async Task<StorjFolder> GetFolder(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                    var bucketCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Folder);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (driveCrumb != null && bucketCrumb != null && lastCrumb != null)
                    {
                        var account = await connection.FindWithQueryAsync<StorjAccount>(
                            $"SELECT * FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.Name)} = ?", driveCrumb.DisplayName);
                        if (account != null && accessGrants.TryGetValue(account.AccessGrant, out var accessGrant))
                        {
                            var bucket = await accessGrant.GetBucket(bucketCrumb.DisplayName);
                            if (bucketCrumb != lastCrumb)
                            {
                                var location = $@"cloud:\Storj\{account.Name}\{bucketCrumb.DisplayName}\";
                                var @object = await accessGrant.GetObject(bucket, path.Substring(location.Length).Replace("\\", "/").WithEnding("/"));
                                return @object.Convert(account, bucket) as StorjFolder;
                            }
                            else
                            {
                                return bucket.Convert(account) as StorjFolder;
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

        public static async Task<StorjFile> GetFile(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Cloud)
                {
                    var crumbs = path.GetBreadcrumbs();
                    var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                    var bucketCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Folder);
                    var lastCrumb = crumbs.LastOrDefault();
                    if (driveCrumb != null && bucketCrumb != null && lastCrumb != null)
                    {
                        var account = await connection.FindWithQueryAsync<StorjAccount>(
                            $"SELECT * FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.Name)} = ?", driveCrumb.DisplayName);
                        if (account != null && accessGrants.TryGetValue(account.AccessGrant, out var accessGrant))
                        {
                            var bucket = await accessGrant.GetBucket(bucketCrumb.DisplayName);
                            if (bucket != null)
                            {
                                var location = $@"cloud:\Storj\{account.Name}\{bucket.Name}\";
                                var storjObject = await accessGrant.GetObject(bucket, path.Substring(location.Length).Replace('\\', '/'));
                                if (storjObject != null)
                                {
                                    return storjObject.Convert(account, bucket) as StorjFile;
                                }
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

        public static async Task<Stream> GetContent(StorjFile file)
        {
            try
            {
                var crumbs = file.Path.GetBreadcrumbs();
                var driveCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Drive);
                var bucketCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Folder);
                var lastCrumb = crumbs.LastOrDefault();
                if (driveCrumb != null && bucketCrumb != null && lastCrumb != null)
                {
                    var account = await connection.FindWithQueryAsync<StorjAccount>(
                            $"SELECT * FROM {nameof(StorjAccount)} WHERE {nameof(StorjAccount.Name)} = ?", driveCrumb.DisplayName);
                    if (account != null && accessGrants.TryGetValue(account.AccessGrant, out var accessGrant))
                    {
                        var bucket = await accessGrant.GetBucket(bucketCrumb.DisplayName);
                        if (bucket != null)
                        {
                            var content = await accessGrant.GetContent(bucket, file.StorjObject.Key);
                            return content;
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

        public static async Task<StorjFolder> CreateFolder(StorjFolder folder, string name, CreateOption option)
        {
            try
            {
                if (accessGrants.TryGetValue(folder.Account.AccessGrant, out var access))
                {
                    var newFolder = await access.CreateFolder(folder.Bucket, $"{(folder.StorjObject is null ? "" : folder.StorjObject.Key)}{name}/", option);
                    if (newFolder != null)
                    {
                        return newFolder.Convert(folder.Account, folder.Bucket) as StorjFolder;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<StorjFile> CreateFile(StorjFolder folder, string name, CreateOption option, byte[] content)
        {
            try
            {
                if (accessGrants.TryGetValue(folder.Account.AccessGrant, out var access))
                {
                    var newFile = await access.CreateFile(folder.Bucket, $"{(folder.StorjObject is null ? "" : folder.StorjObject.Key)}{name}", option, content);
                    if (newFile != null)
                    {
                        return newFile.Convert(folder.Account, folder.Bucket) as StorjFile;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> Rename(StorjFolder folder, string name, RenameOption option)
        {
            try
            {
                if (folder.StorjObject != null && accessGrants.TryGetValue(folder.Account.AccessGrant, out var access))
                {
                    var segments = folder.StorjObject.Key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    segments[segments.Length - 1] = name;
                    var newKey = string.Join("/", segments).WithEnding("/");
                    var newName = await access.Rename(folder.Bucket, folder.StorjObject.Key, newKey, option);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        segments[segments.Length - 1] = newName;
                        folder.StorjObject.Key = string.Join("/", segments).WithEnding("/");
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

        public static async Task<string> Rename(StorjFile file, string name, RenameOption option)
        {
            try
            {
                if (file.StorjObject != null && accessGrants.TryGetValue(file.Account.AccessGrant, out var access))
                {
                    var segments = file.StorjObject.Key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    segments[segments.Length - 1] = name;
                    var newKey = string.Join("/", segments);
                    var newName = await access.Rename(file.Bucket, file.StorjObject.Key, newKey, option);
                    if (!string.IsNullOrEmpty(newName))
                    {
                        segments[segments.Length - 1] = newName;
                        file.StorjObject.Key = string.Join("/", segments);
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

        public static async Task Delete(StorjFolder folder)
        {
            if (folder.StorjObject != null && accessGrants.TryGetValue(folder.Account.AccessGrant, out var access))
            {
                await access.Delete(folder.Bucket, folder.StorjObject.Key);
            }
        }

        public static async Task Delete(StorjFile file)
        {
            if (accessGrants.TryGetValue(file.Account.AccessGrant, out var access))
            {
                await access.Delete(file.Bucket, file.StorjObject.Key);
            }
        }

        public static string GetUrl(StorjFile file)
        {
            if (accessGrants.TryGetValue(file.Account.AccessGrant, out var access))
            {
                var url = access.CreateShareURL(file.Bucket.Name, file.StorjObject.Key, false, true);
                return url.Replace("https://gateway.storjshare.io/", "https://link.storjshare.io/");
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
