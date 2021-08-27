using System;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;

namespace JumpPoint.Platform.Services
{
    public static partial class StorageService
    {
        static Task<string> PlatformRename(StorageItemBase item, string name, RenameCollisionOption option)
            => throw new NotImplementedException();

        static Task<FolderBase> PlatformCreateFolder(DirectoryBase directory, string name)
            => throw new NotImplementedException();

        static Task<FileBase> PlatformCreateFile(DirectoryBase directory, string name)
            => throw new NotImplementedException();

        static Task PlatformCopyItem(DirectoryBase destination, StorageItemBase item)
            => throw new NotImplementedException();

        static Task PlatformMoveItem(DirectoryBase destination, StorageItemBase item)
            => throw new NotImplementedException();

        static Task<bool> PlatformExists(StorageItemBase item)
            => throw new NotImplementedException();

        static Task PlatformLoad(FileBase file)
            => throw new NotImplementedException();

        static Task PlatformLoadProperties(FileBase file, FileBaseProperties properties)
            => throw new NotImplementedException();
    }
}
