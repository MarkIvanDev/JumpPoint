using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.LocalStorage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class LocalStorageService
    {

        public static Task<IList<LocalDrive>> GetDrives()
            => PlatformGetDrives();

        public static Task<IList<StorageItemBase>> GetItems(ILocalDirectory directory)
            => PlatformGetItems(directory);
        
        public static Task<DirectoryBase> GetDirectory(string path)
            => PlatformGetDirectory(path);

        public static Task<IList<LocalFolder>> GetFolders(ILocalDirectory directory)
            => PlatformGetFolders(directory);

        public static Task<IList<LocalFile>> GetFiles(ILocalDirectory directory)
            => PlatformGetFiles(directory);

        public static Task<LocalDrive> GetDrive(string path)
            => PlatformGetDrive(path);

        public static Task<LocalFolder> GetFolder(string path)
            => PlatformGetFolder(path);

        public static Task<LocalFile> GetFile(string path)
            => PlatformGetFile(path);

    }
}
