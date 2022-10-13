using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.WslStorage;

namespace JumpPoint.Platform.Services
{
    public static partial class WslStorageService
    {
        static Task<IList<WslDrive>> PlatformGetDrives()
            => throw new NotImplementedException();

        static Task<IList<StorageItemBase>> PlatformGetItems(IWslDirectory directory)
            => throw new NotImplementedException();

        static Task<WslDrive> PlatformGetDrive(string path)
            => throw new NotImplementedException();

        static Task<WslFolder> PlatformGetFolder(string path)
            => throw new NotImplementedException();

        static Task<WslFile> PlatformGetFile(string path)
            => throw new NotImplementedException();
    }
}
