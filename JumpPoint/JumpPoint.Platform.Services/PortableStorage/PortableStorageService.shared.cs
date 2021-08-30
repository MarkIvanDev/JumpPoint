using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class PortableStorageService
    {
        public static event EventHandler<PortableDriveCollectionChangedEventArgs> PortableDriveCollectionChanged;

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

    public class PortableDriveCollectionChangedEventArgs
    {
        public PortableDriveCollectionChangedEventArgs(NotifyCollectionChangedAction action, string deviceId, PortableDrive drive)
        {
            Action = action;
            DeviceId = deviceId;
            Drive = drive;
        }

        public NotifyCollectionChangedAction Action { get; }

        public string DeviceId { get; }

        public PortableDrive Drive { get; }
    }

}
