using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.OneDrive;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services.OneDrive;

namespace JumpPoint.Platform.Services
{
    public static class CloudStorageService
    {
        public static CloudStorageProvider GetProvider(string path)
        {
            if (path.GetPathKind() == PathKind.Cloud)
            {
                var crumbs = path.GetBreadcrumbs();
                var providerCrumb = crumbs.FirstOrDefault(c => c.PathType == PathType.CloudStorage);
                if (providerCrumb != null && Enum.TryParse<CloudStorageProvider>(providerCrumb.DisplayName, out var provider))
                {
                    return provider;
                }
            }
            
            return CloudStorageProvider.Unknown;
        }

        public static Task Rename(StorageItemBase item, string name, RenameCollisionOption option)
        {
            return Task.CompletedTask;
        }

        public static Task Delete(IList<StorageItemBase> items, bool deletePermanently)
        {
            return Task.CompletedTask;
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

        public static Task<CloudFolder> CreateFolder(DirectoryBase directory, string name)
        {
            return Task.FromResult<CloudFolder>(null);
        }

        public static Task<CloudFile> CreateFile(DirectoryBase directory, string name)
        {
            return Task.FromResult<CloudFile>(null);
        }

    }
}
