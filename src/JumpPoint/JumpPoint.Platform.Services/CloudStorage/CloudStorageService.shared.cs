using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static class CloudStorageService
    {
        public static CloudStorageProvider GetProvider(string path)
        {
            if (path.GetPathKind() == PathKind.Cloud)
            {

            }
            
            return CloudStorageProvider.Unknown;
        }

        public static Task Rename(StorageItemBase item, string name, RenameCollisionOption option)
        {
            return Task.CompletedTask;
        }

        public static Task<IList<CloudFolder>> GetFolders(ICloudDirectory directory)
        {
            return Task.FromResult<IList<CloudFolder>>(new List<CloudFolder>());
        }

        public static Task<IList<CloudFile>> GetFiles(ICloudDirectory directory)
        {
            return Task.FromResult<IList<CloudFile>>(new List<CloudFile>());
        }

        public static Task<CloudDrive> GetDrive(string path)
        {
            return Task.FromResult<CloudDrive>(null);
        }

        public static Task<CloudFolder> GetFolder(string path)
        {
            return Task.FromResult<CloudFolder>(null);
        }

        public static Task<CloudFile> GetFile(string path)
        {
            return Task.FromResult<CloudFile>(null);
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
