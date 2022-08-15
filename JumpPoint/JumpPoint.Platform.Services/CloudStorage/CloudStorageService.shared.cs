using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.OneDrive;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services.OneDrive;

namespace JumpPoint.Platform.Services
{
    public static partial class CloudStorageService
    {

        public static async Task Initialize()
        {
            await OneDriveService.Initialize();
        }

        public static async Task<IList<CloudAccount>> GetAccounts()
        {
            var accounts = new List<CloudAccount>();
            accounts.AddRange(await GetAccounts(CloudStorageProvider.OneDrive));
            return accounts;
        }

        public static async Task<IList<CloudAccount>> GetAccounts(CloudStorageProvider provider)
        {
            var accounts = new List<CloudAccount>();
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    accounts.AddRange(await OneDriveService.GetAccounts());
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }
            return accounts;
        }

        public static async Task<CloudAccount> AddAccount(CloudStorageProvider provider)
        {
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.AddAccount();

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task<string> RenameAccount(CloudAccount account, string newName)
        {
            switch (account.Provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.RenameAccount((OneDriveAccount)account, newName);

                case CloudStorageProvider.Unknown:
                default:
                    return account.Name;
            }
        }

        public static async Task RemoveAccount(CloudAccount account)
        {
            switch (account.Provider)
            {
                case CloudStorageProvider.OneDrive:
                    await OneDriveService.RemoveAccount((OneDriveAccount)account);
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }
        }

        public static CloudStorageProvider GetProvider(string path)
        {
            if (path.GetPathKind() == PathKind.Cloud)
            {
                var crumbs = path.GetBreadcrumbs();
                var providerCrumb = crumbs.FirstOrDefault(c => c.AppPath == AppPath.Cloud);
                if (providerCrumb != null && Enum.TryParse<CloudStorageProvider>(providerCrumb.DisplayName, out var provider))
                {
                    return provider;
                }
            }
            
            return CloudStorageProvider.Unknown;
        }

        public static async Task<IList<CloudDrive>> GetDrives()
        {
            var drives = new List<CloudDrive>();
            drives.AddRange(await GetDrives(CloudStorageProvider.OneDrive));
            return drives;
        }

        public static async Task<IList<CloudDrive>> GetDrives(CloudStorageProvider provider)
        {
            var drives = new List<CloudDrive>();
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    drives.AddRange(await OneDriveService.GetDrives());
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }

            return drives;
        }

        public static async Task<IList<StorageItemBase>> GetItems(ICloudDirectory directory)
        {
            var items = new List<StorageItemBase>();
            switch (directory.Provider)
            {
                case CloudStorageProvider.OneDrive when directory is OneDriveDrive odd:
                    items.AddRange(await OneDriveService.GetItems(odd));
                    break;
                case CloudStorageProvider.OneDrive when directory is OneDriveFolder odf:
                    items.AddRange(await OneDriveService.GetItems(odf));
                    break;

                case CloudStorageProvider.Unknown:
                default:
                    break;
            }

            return items;
        }

        public static async Task<CloudDrive> GetDrive(string path)
        {
            var provider = GetProvider(path);
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.GetDrive(path);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task<CloudFolder> GetFolder(string path)
        {
            var provider = GetProvider(path);
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.GetFolder(path);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task<CloudFile> GetFile(string path)
        {
            var provider = GetProvider(path);
            switch (provider)
            {
                case CloudStorageProvider.OneDrive:
                    return await OneDriveService.GetFile(path);

                case CloudStorageProvider.Unknown:
                default:
                    return null;
            }
        }

        public static async Task OpenFile(FileBase file)
        {
            try
            {
                switch (file)
                {
                    case OneDriveFile odFile:
                        await Xamarin.Essentials.Browser.OpenAsync(odFile.GraphItem.WebUrl);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        public static async Task<FileBase> DownloadFile(FileBase file)
        {
            switch (file)
            {
                case OneDriveFile odFile:
                    var odContent = await OneDriveService.GetContent(odFile);
                    if (odContent != null)
                    {
                        return await StorageService.DownloadFile(file.Name, odContent);
                    }
                    break;
                default:
                    break;
            }
            return null;
        }

        public static async Task<CloudFolder> CreateFolder(DirectoryBase directory, string name, CreateOption option)
        {
            switch (directory)
            {
                case OneDriveDrive odDrive:
                    return await OneDriveService.CreateFolder(odDrive, name, option);

                case OneDriveFolder odFolder:
                    return await OneDriveService.CreateFolder(odFolder, name, option);

                default:
                    return null;
            }
        }

        public static async Task<CloudFile> CreateFile(DirectoryBase directory, string name, CreateOption option)
        {
            switch (directory)
            {
                case OneDriveDrive odDrive:
                    return await OneDriveService.CreateFile(odDrive, name, option);

                case OneDriveFolder odFolder:
                    return await OneDriveService.CreateFile(odFolder, name, option);

                default:
                    return null;
            }
        }

        public static async Task<string> Rename(StorageItemBase item, string name, RenameOption option)
        {
            switch (item)
            {
                case OneDriveFolder odFolder:
                    return await OneDriveService.Rename(odFolder, name, option);

                case OneDriveFile odFile:
                    return await OneDriveService.Rename(odFile, name, option);
                default:
                    return string.Empty;
            }
        }

        public static async Task Delete(IList<StorageItemBase> items)
        {
            foreach (var item in items)
            {
                switch (item)
                {
                    case OneDriveFolder odFolder:
                        await OneDriveService.Delete(odFolder);
                        break;

                    case OneDriveFile odFile:
                        await OneDriveService.Delete(odFile);
                        break;

                    default:
                        break;
                }
            }
        }

    }
}
