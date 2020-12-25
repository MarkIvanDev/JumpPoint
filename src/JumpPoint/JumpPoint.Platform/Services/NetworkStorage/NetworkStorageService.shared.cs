using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.NetworkStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;

namespace JumpPoint.Platform.Services
{
    public static partial class NetworkStorageService
    {
        public static Task<IList<NetworkDrive>> GetDrives()
            => PlatformGetDrives();

        public static Task<IList<StorageItemBase>> GetItems(INetworkDirectory directory)
            => PlatformGetItems(directory);
        public static Task<IList<NetworkFolder>> GetFolders(INetworkDirectory directory)
            => PlatformGetFolders(directory);

        public static Task<IList<NetworkFile>> GetFiles(INetworkDirectory directory)
            => PlatformGetFiles(directory);

        public static Task<NetworkDrive> GetDrive(string path)
            => PlatformGetDrive(path);

        public static Task<NetworkFolder> GetFolder(string path)
            => PlatformGetFolder(path);

        public static Task<NetworkFile> GetFile(string path)
            => PlatformGetFile(path);

    }
}
