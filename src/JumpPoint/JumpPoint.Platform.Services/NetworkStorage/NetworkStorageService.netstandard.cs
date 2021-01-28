using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.NetworkStorage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class NetworkStorageService
    {

        static Task<IList<NetworkDrive>> PlatformGetDrives()
            => throw new NotImplementedException();

        static Task<IList<StorageItemBase>> PlatformGetItems(INetworkDirectory directory)
            => throw new NotImplementedException();

        static Task<IList<NetworkFolder>> PlatformGetFolders(INetworkDirectory directory)
            => throw new NotImplementedException();

        static Task<IList<NetworkFile>> PlatformGetFiles(INetworkDirectory directory)
            => throw new NotImplementedException();

        static Task<NetworkDrive> PlatformGetDrive(string path)
            => throw new NotImplementedException();

        static Task<NetworkFolder> PlatformGetFolder(string path)
            => throw new NotImplementedException();

        static Task<NetworkFile> PlatformGetFile(string path)
            => throw new NotImplementedException();

    }
}
