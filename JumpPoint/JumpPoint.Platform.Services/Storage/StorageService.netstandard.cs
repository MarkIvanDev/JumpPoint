using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;

namespace JumpPoint.Platform.Services
{
    public static partial class StorageService
    {
        static Task<string> PlatformRename(StorageItemBase item, string name, RenameOption option)
            => throw new NotImplementedException();

        static Task<FolderBase> PlatformCreateFolder(DirectoryBase directory, string name, CreateOption option)
            => throw new NotImplementedException();

        static Task<FileBase> PlatformCreateFile(DirectoryBase directory, string name, CreateOption option, byte[] content)
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

        static Task<FileBase> PlatformDownloadFile(string fileName, Stream content)
            => throw new NotImplementedException();
    }
}
