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
        static Task<IList<LocalDrive>> PlatformGetDrives()
            => throw new NotImplementedException();

        static Task<IList<StorageItemBase>> PlatformGetItems(ILocalDirectory folderBase)
            => throw new NotImplementedException();

        static Task<DirectoryBase> PlatformGetDirectory(string path)
            => throw new NotImplementedException();

        static Task<IList<LocalFolder>> PlatformGetFolders(ILocalDirectory directory)
            => throw new NotImplementedException();

        static Task<IList<LocalFile>> PlatformGetFiles(ILocalDirectory directory)
            => throw new NotImplementedException();

        static Task<LocalDrive> PlatformGetDrive(string path)
            => throw new NotImplementedException();

        static Task<LocalFolder> PlatformGetFolder(string path)
            => throw new NotImplementedException();

        static Task<LocalFile> PlatformGetFile(string path)
            => throw new NotImplementedException();

    }
}
