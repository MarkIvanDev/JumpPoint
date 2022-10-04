using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Interop;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.LocalStorage;
using JumpPoint.Platform.Items.NetworkStorage;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;
using NittyGritty.Platform.Storage;
using Windows.Storage;
using WinStorage = Windows.Storage;
using CreationCollisionOption = Windows.Storage.CreationCollisionOption;
using NameCollisionOption = Windows.Storage.NameCollisionOption;
using NittyGritty.Extensions;
using JumpPoint.Extensions;
using NittyGritty.Utilities;

namespace JumpPoint.Platform.Services
{
    public static partial class StorageService
    {
        public static async Task<DriveBase> GetDrive(StorageType storageType, StorageFolder drive)
        {
            try
            {
                var props = await drive.Properties.RetrievePropertiesAsync(
                    new List<string>
                    {
                        BasicProperties.Key.DateAccessed,
                        BasicProperties.Key.DateModified,
                        BasicProperties.Key.Size,
                        BasicProperties.Key.FileSystem,
                        BasicProperties.Key.FreeSpace,
                        BasicProperties.Key.Capacity
                    });
                var fileSystem = props[BasicProperties.Key.FileSystem] as string;
                var freeSpace = props[BasicProperties.Key.FreeSpace] as ulong?;
                var capacity = props[BasicProperties.Key.Capacity] as ulong?;
                var usedSpace = capacity.HasValue && freeSpace.HasValue ? capacity - freeSpace : null;

                switch (storageType)
                {
                    case StorageType.Local:
                        return new LocalDrive(
                            name: drive.DisplayName,
                            path: drive.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: drive.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)drive.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?)
                        {
                            FileSystem = fileSystem,
                            FreeSpace = freeSpace,
                            Capacity = capacity,
                            UsedSpace = usedSpace
                        };

                    case StorageType.Portable:
                        return new PortableDrive(
                            name: drive.DisplayName,
                            context: string.IsNullOrEmpty(drive.Path) ? new NGFolder(drive) : null,
                            path: string.IsNullOrEmpty(drive.Path) ? $@"{Prefix.UNMOUNTED}{drive.DisplayName}" : drive.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: drive.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)drive.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?)
                        {
                            FileSystem = fileSystem,
                            FreeSpace = freeSpace,
                            Capacity = capacity,
                            UsedSpace = usedSpace
                        };

                    case StorageType.Network:
                        return new NetworkDrive(
                            name: drive.DisplayName,
                            path: drive.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: drive.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)drive.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?)
                        {
                            FileSystem = fileSystem,
                            FreeSpace = freeSpace,
                            Capacity = capacity,
                            UsedSpace = usedSpace
                        };

                    case StorageType.Cloud:
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        public static async Task<FolderBase> GetFolder(StorageType storageType, StorageFolder folder)
        {
            try
            {
                var props = await folder.Properties.RetrievePropertiesAsync(
                    new List<string>
                    {
                        BasicProperties.Key.DateAccessed,
                        BasicProperties.Key.DateModified,
                        BasicProperties.Key.Size
                    });

                switch (storageType)
                {
                    case StorageType.Local:
                        return new LocalFolder(
                            path: folder.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: folder.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)folder.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?);

                    case StorageType.Portable:
                        return new PortableFolder(
                            context: string.IsNullOrEmpty(folder.Path) ? new NGFolder(folder) : null,
                            path: folder.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: folder.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)folder.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?);

                    case StorageType.Network:
                        return new NetworkFolder(
                            path: folder.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: folder.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)folder.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?);

                    case StorageType.Cloud:
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        public static async Task<FileBase> GetFile(StorageType storageType, StorageFile file)
        {
            try
            {
                var props = await file.Properties.RetrievePropertiesAsync(
                    new List<string>
                    {
                        BasicProperties.Key.DateAccessed,
                        BasicProperties.Key.DateModified,
                        BasicProperties.Key.Size
                    });

                switch (storageType)
                {
                    case StorageType.Local:
                        return new LocalFile(
                            path: file.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: file.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)file.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?);

                    case StorageType.Portable:
                        return new PortableFile(
                            context: string.IsNullOrEmpty(file.Path) ? new NGFile(file) : null,
                            path: file.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: file.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)file.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?);

                    case StorageType.Network:
                        return new NetworkFile(
                            path: file.Path,
                            dateAccessed: props[BasicProperties.Key.DateAccessed] as DateTimeOffset?,
                            dateCreated: file.DateCreated,
                            dateModified: props[BasicProperties.Key.DateModified] as DateTimeOffset?,
                            attributes: (System.IO.FileAttributes?)file.Attributes,
                            size: props[BasicProperties.Key.Size] as ulong?);

                    case StorageType.Cloud:
                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        public static async Task<string> GetPath(IStorageItem2 item)
        {
            if (!string.IsNullOrEmpty(item.Path)) return item.Path;

            var pathBuilder = new StringBuilder();
            pathBuilder.Append(item.Name);
            var parent = await item.GetParentAsync();
            if (!string.IsNullOrEmpty(parent?.Path))
            {
                // This happens if the StorageFile is streamed
                pathBuilder.Insert(0, $"{Prefix.UNMOUNTED}{parent?.Path}{Path.DirectorySeparatorChar}");
            }
            else
            {
                while (parent != null)
                {
                    var name = parent.Name;
                    var displayName = parent.DisplayName;
                    parent = await item.GetParentAsync();

                    if (parent != null)
                    {
                        pathBuilder.Insert(0, $"{name}{Path.DirectorySeparatorChar}");
                    }
                    else
                    {
                        pathBuilder.Insert(0, $"{Prefix.UNMOUNTED}{displayName}{Path.DirectorySeparatorChar}");
                    }
                }
            }
            return pathBuilder.ToString();
        }

        static async Task<string> PlatformRename(StorageItemBase item, string name, RenameOption option)
        {
            if (item is FileBase file)
            {
                var storageFile = await FileInterop.GetStorageFile(file);
                if (storageFile != null)
                {
                    await storageFile.RenameAsync(name, (NameCollisionOption)option);
                    return storageFile.Name;
                }
            }
            else if (item is DirectoryBase directory)
            {
                var storageFolder = await FileInterop.GetStorageFolder(directory);
                if (storageFolder != null)
                {
                    await storageFolder.RenameAsync(name, (NameCollisionOption)option);
                    return storageFolder.Name;
                }
            }
            return string.Empty;
        }

        static async Task<FolderBase> PlatformCreateFolder(DirectoryBase directory, string name, CreateOption option)
        {
            var storageFolder = await FileInterop.GetStorageFolder(directory);
            if (storageFolder != null)
            {
                var newFolder = await storageFolder.CreateFolderAsync(name, (CreationCollisionOption)option);
                return await GetFolder(directory.StorageType, newFolder);
            }
            return null;
        }

        static async Task<FileBase> PlatformCreateFile(DirectoryBase directory, string name, CreateOption option, byte[] content)
        {
            var storageFile = await FileInterop.GetStorageFolder(directory);
            if (storageFile != null)
            {
                var newFile = await CodeHelper.InvokeOrDefault(async () => await storageFile.CreateFileAsync(name, (CreationCollisionOption)option));
                if (newFile != null)
                {
                    if (content != null)
                    {
                        await CodeHelper.InvokeOrDefault(async () =>
                        {
                            using (var stream = await newFile.OpenStreamForWriteAsync())
                            {
                                stream.Write(content, 0, content.Length);
                            }
                        });
                    }
                    return await GetFile(directory.StorageType, newFile);
                }
            }
            return null;
        }

        static async Task PlatformCopyItem(DirectoryBase destination, StorageItemBase item)
        {
            try
            {
                var destinationFolder = await FileInterop.GetStorageFolder(destination);
                if (destinationFolder is null) return;

                var storageitem = await FileInterop.GetStorageItem(item);
                if (storageitem is StorageFile storageFile)
                {
                    await storageFile.CopyAsync(destinationFolder, storageFile.Name, NameCollisionOption.GenerateUniqueName);
                }
                else if (storageitem is StorageFolder storageFolder)
                {
                    await CopyFolder(destinationFolder, storageFolder);
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        static async Task CopyFolder(StorageFolder destination, StorageFolder source)
        {
            var destinationFolder = await destination.CreateFolderAsync(source.Name, CreationCollisionOption.OpenIfExists);

            foreach (var file in await source.GetFilesAsync())
            {
                await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.GenerateUniqueName);
            }
            foreach (var folder in await source.GetFoldersAsync())
            {
                await CopyFolder(destinationFolder, folder);
            }
        }

        static async Task PlatformMoveItem(DirectoryBase destination, StorageItemBase item)
        {
            try
            {
                var destinationFolder = await FileInterop.GetStorageFolder(destination);
                if (destinationFolder is null) return;

                var storageitem = await FileInterop.GetStorageItem(item);
                if (storageitem is StorageFile storageFile)
                {
                    if ((storageFile.Attributes & WinStorage.FileAttributes.Temporary) == WinStorage.FileAttributes.Temporary)
                    {
                        await storageFile.CopyAsync(destinationFolder, storageFile.Name, NameCollisionOption.GenerateUniqueName);
                    }
                    else
                    {
                        await storageFile.MoveAsync(destinationFolder, storageFile.Name, NameCollisionOption.GenerateUniqueName);
                    }
                }
                else if (storageitem is StorageFolder storageFolder)
                {
                    await MoveFolder(destinationFolder, storageFolder);
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        static async Task MoveFolder(StorageFolder destination, StorageFolder source)
        {
            var destinationFolder = await destination.CreateFolderAsync(source.Name, CreationCollisionOption.OpenIfExists);

            foreach (var file in await source.GetFilesAsync())
            {
                if ((file.Attributes & WinStorage.FileAttributes.Temporary) == WinStorage.FileAttributes.Temporary)
                {
                    await file.CopyAsync(destinationFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                }
                else
                {
                    await file.MoveAsync(destinationFolder, file.Name, NameCollisionOption.GenerateUniqueName);
                }
            }
            foreach (var folder in await source.GetFoldersAsync())
            {
                await MoveFolder(destinationFolder, folder);
            }
            await source.DeleteAsync();
        }

        static async Task<bool> PlatformExists(StorageItemBase item)
        {
            try
            {
                var storageItem = await FileInterop.GetStorageItem(item);
                if (storageItem != null)
                {
                    var props = await storageItem.GetBasicPropertiesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        static async Task PlatformLoad(FileBase file)
        {
            try
            {
                if (!HasBuiltInIcon(file.FileType))
                {
                    var storageFile = await FileInterop.GetStorageFile(file);
                    if (storageFile is null) return;

                    file.DisplayType = storageFile.DisplayType;
                    file.ContentType = storageFile.ContentType;

                    var thumbnail = await storageFile.GetThumbnailAsync(
                        Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 58, Windows.Storage.FileProperties.ThumbnailOptions.UseCurrentScale);
                    file.Thumbnail = thumbnail.AsStreamForRead();
                }
            }
            catch (Exception)
            {
                //Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        static async Task PlatformLoadProperties(FileBase file, FileBaseProperties properties)
        {
            try
            {
                var storageFile = await FileInterop.GetStorageFile(file);
                if (storageFile is null) return;

                if ((properties & FileBaseProperties.Basic) == FileBaseProperties.Basic)
                {
                    file.Name = storageFile.Name;
                    file.DisplayName = storageFile.DisplayName;
                    file.DisplayType = storageFile.DisplayType;
                    file.FileType = storageFile.FileType;
                    file.ContentType = storageFile.ContentType;
                    var props = await storageFile.Properties.RetrievePropertiesAsync(BasicProperties.Key.Common());
                    file.DateAccessed = props[BasicProperties.Key.DateAccessed] as DateTimeOffset?;
                    file.DateCreated = props[BasicProperties.Key.DateCreated] as DateTimeOffset?;
                    file.DateModified = props[BasicProperties.Key.DateModified] as DateTimeOffset?;
                    file.Attributes = props[BasicProperties.Key.Attributes] is uint att ? (System.IO.FileAttributes?)att : null;
                    file.Size = props[BasicProperties.Key.Size] as ulong?;
                    file.SizeOnDisk = props[BasicProperties.Key.SizeOnDisk] as ulong?;
                }

                if ((properties & FileBaseProperties.Core) == FileBaseProperties.Core)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(CoreProperties.Key.All());
                    file.CoreProperties = CoreProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Document) == FileBaseProperties.Document)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(DocumentProperties.Key.All());
                    file.DocumentProperties = DocumentProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Image) == FileBaseProperties.Image)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(ImageProperties.Key.All());
                    file.ImageProperties = ImageProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Photo) == FileBaseProperties.Photo)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(PhotoProperties.Key.All());
                    file.PhotoProperties = PhotoProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Media) == FileBaseProperties.Media)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(MediaProperties.Key.All());
                    file.MediaProperties = MediaProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Audio) == FileBaseProperties.Audio)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(AudioProperties.Key.All());
                    file.AudioProperties = AudioProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Music) == FileBaseProperties.Music)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(MusicProperties.Key.All());
                    file.MusicProperties = MusicProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.Video) == FileBaseProperties.Video)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(VideoProperties.Key.All());
                    file.VideoProperties = VideoProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.DRM) == FileBaseProperties.DRM)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(DrmProperties.Key.All());
                    file.DrmProperties = DrmProperties.Extract(props);
                }

                if ((properties & FileBaseProperties.GPS) == FileBaseProperties.GPS)
                {
                    var props = await storageFile.Properties.RetrievePropertiesAsync(GpsProperties.Key.All());
                    file.GpsProperties = GpsProperties.Extract(props);
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        static async Task<FileBase> PlatformDownloadFile(string fileName, Stream content)
        {
            try
            {
                var file = await DownloadsFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                await IOHelper.WriteBytes(file, content.ToByteArray());
                return await GetFile(StorageType.Local, file);
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
