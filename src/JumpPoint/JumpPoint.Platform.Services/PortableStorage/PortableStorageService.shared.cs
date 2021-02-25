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
        public static Task<IList<PortableDrive>> GetDrives()
            => PlatformGetDrives();

        public static Task<IList<string>> GetDrivePaths()
            => PlatformGetDrivePaths();

        public static Task<IList<StorageItemBase>> GetItems(IPortableDirectory directory)
            => PlatformGetItems(directory);

        public static Task<PortableDrive> GetDriveFromId(string deviceId)
            => PlatformGetDriveFromId(deviceId);

        public static Task<PortableDrive> GetDrive(string path)
            => PlatformGetDrive(path);

        public static Task<PortableFolder> GetFolder(string path)
            => PlatformGetFolder(path);

        public static Task<PortableFile> GetFile(string path)
            => PlatformGetFile(path);
    }
}
