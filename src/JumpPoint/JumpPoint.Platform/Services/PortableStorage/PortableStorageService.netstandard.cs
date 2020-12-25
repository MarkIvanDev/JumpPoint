using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class PortableStorageService
    {
        static Task<IList<PortableDrive>> PlatformGetDrives()
            => throw new NotImplementedException();

        static Task<IList<string>> PlatformGetDrivePaths()
            => throw new NotImplementedException();

        static Task<IList<StorageItemBase>> PlatformGetItems(IPortableDirectory directory)
            => throw new NotImplementedException();

        static Task<IList<PortableFolder>> PlatformGetFolders(IPortableDirectory directory)
            => throw new NotImplementedException();

        static Task<IList<PortableFile>> PlatformGetFiles(IPortableDirectory directory)
            => throw new NotImplementedException();

        static Task<PortableDrive> PlatformGetDriveFromId(string deviceId)
            => throw new NotImplementedException();

        static Task<PortableDrive> PlatformGetDrive(string path)
            => throw new NotImplementedException();

        static Task<PortableFolder> PlatformGetFolder(string path)
            => throw new NotImplementedException();

        static Task<PortableFile> PlatformGetFile(string path)
            => throw new NotImplementedException();

    }
}
