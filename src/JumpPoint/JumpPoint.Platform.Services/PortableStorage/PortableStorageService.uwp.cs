using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Interop;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class PortableStorageService
    {
        static async Task<IList<PortableDrive>> PlatformGetDrives()
        {
            var drives = new List<PortableDrive>();
            var portableDevices = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
            foreach (var item in portableDevices)
            {
                var drive = await PlatformGetDriveFromId(item.Id);
                if (drive != null)
                {
                    drives.Add(drive);
                }
            }
            return drives;
        }

        static async Task<IList<string>> PlatformGetDrivePaths()
        {
            var drives = new List<string>();
            var portableDevices = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
            foreach (var item in portableDevices)
            {
                try
                {
                    var drive = StorageDevice.FromId(item.Id);
                    if (drive != null)
                    {
                        drives.Add(string.IsNullOrEmpty(drive.Path) ? $"{Prefix.UNMOUNTED}{drive.DisplayName}" : drive.Path);
                    }
                }
                catch (Exception)
                {
                    // Ignore drive
                }
            }
            return drives;
        }

        static async Task<IList<StorageItemBase>> PlatformGetItems(IPortableDirectory directory)
        {
            var items = new List<StorageItemBase>();
            if (directory.Path.GetPathKind() == PathKind.Unmounted)
            {
                if (directory.Context != null && directory.Context.Context is StorageFolder folder)
                {
                    var portableItems = await folder.GetItemsAsync();

                    foreach (var item in portableItems)
                    {
                        if (item.IsOfType(StorageItemTypes.File))
                        {
                            var i = await StorageService.GetFile(StorageType.Portable, item as StorageFile);
                            if (i is PortableFile f)
                            {
                                f.Path = Path.Combine(directory.Path, item.Name);
                                items.Add(f);
                            }
                        }
                        else if (item.IsOfType(StorageItemTypes.Folder))
                        {
                            var i = await StorageService.GetFolder(StorageType.Portable, item as StorageFolder);
                            if (i is PortableFolder f)
                            {
                                f.Path = Path.Combine(directory.Path, item.Name);
                                items.Add(f);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var item in FileInterop.GetItems(StorageType.Portable, directory.Path))
                {
                    if (item is StorageItemBase i)
                    {
                        items.Add(i);
                    }
                }
            }
            return items;
        }

        static async Task<IList<PortableFolder>> PlatformGetFolders(IPortableDirectory directory)
        {
            var items = new List<PortableFolder>();
            if (directory.Path.GetPathKind() == PathKind.Unmounted)
            {
                if (directory.Context != null && directory.Context.Context is StorageFolder folder)
                {
                    var portableFolders = await folder.GetFoldersAsync();

                    foreach (var item in portableFolders)
                    {
                        var i = await StorageService.GetFolder(StorageType.Portable, item);
                        if (i is PortableFolder f)
                        {
                            f.Path = Path.Combine(directory.Path, item.Name);
                            items.Add(f);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in FileInterop.GetFolders(StorageType.Portable, directory.Path))
                {
                    if (item is PortableFolder i)
                    {
                        items.Add(i);
                    }
                }
            }
            return items;
        }

        static async Task<IList<PortableFile>> PlatformGetFiles(IPortableDirectory directory)
        {
            var items = new List<PortableFile>();
            if (directory.Path.GetPathKind() == PathKind.Unmounted)
            {
                if (directory.Context != null && directory.Context.Context is StorageFolder folder)
                {
                    var portableFiles = await folder.GetFilesAsync();

                    foreach (var item in portableFiles)
                    {
                        var i = await StorageService.GetFile(StorageType.Portable, item);
                        if (i is PortableFile f)
                        {
                            f.Path = Path.Combine(directory.Path, item.Name);
                            items.Add(f);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in FileInterop.GetFiles(StorageType.Portable, directory.Path))
                {
                    if (item is PortableFile i)
                    {
                        items.Add(i);
                    }
                }
            }
            return items;
        }

        static async Task<PortableDrive> PlatformGetDriveFromId(string deviceId)
        {
            try
            {
                var device = StorageDevice.FromId(deviceId);
                if (device != null)
                {
                    var drive = await StorageService.GetDrive(StorageType.Portable, device);
                    if (drive is PortableDrive portableDrive)
                    {
                        portableDrive.DeviceId = deviceId;
                        return portableDrive;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        static async Task<PortableDrive> PlatformGetDrive(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Unmounted)
                {
                    var deviceName = path.Remove(0, Prefix.UNMOUNTED.Length).Trim('\\');
                    var portableDevices = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
                    foreach (var item in portableDevices)
                    {
                        if (item.Name == deviceName)
                        {
                            var drive = await PlatformGetDriveFromId(item.Id);
                            if (drive != null)
                            {
                                return drive;
                            }
                        }
                    }
                }
                else
                {
                    var item = await StorageFolder.GetFolderFromPathAsync(path);
                    if (item != null)
                    {
                        var drive = await StorageService.GetDrive(StorageType.Portable, item);
                        return drive as PortableDrive;
                    }
                }
                return null;
            }
            catch (ArgumentException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Path is not valid: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (FileNotFoundException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Drive does not exist: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        static async Task<PortableFolder> PlatformGetFolder(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Unmounted)
                {
                    var crumbs = path.GetBreadcrumbs();
                    StorageFolder currentFolder = null;
                    foreach (var item in crumbs)
                    {
                        if (item.PathType == PathType.Drive)
                        {
                            var drive = await PlatformGetDrive(item.Path);
                            if (drive != null)
                            {
                                currentFolder = drive.Context.Context as StorageFolder;
                            }
                        }
                        else if (item.PathType == PathType.Folder)
                        {
                            if (currentFolder != null)
                            {
                                currentFolder = await currentFolder.GetFolderAsync(item.DisplayName);
                            }
                        }
                    }

                    if (currentFolder != null)
                    {
                        var folder = await StorageService.GetFolder(StorageType.Portable, currentFolder) as PortableFolder;
                        folder.Path = path;
                        return folder;
                    }

                    return null;
                }
                else
                {
                    return FileInterop.GetFolder(StorageType.Portable, path) as PortableFolder;
                }
            }
            catch (ArgumentException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Path is not valid: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (FileNotFoundException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Folder does not exist: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

        static async Task<PortableFile> PlatformGetFile(string path)
        {
            try
            {
                if (path.GetPathKind() == PathKind.Unmounted)
                {
                    var fileName = Path.GetFileName(path);

                    var crumbs = Path.GetDirectoryName(path).GetBreadcrumbs();
                    StorageFolder currentFolder = null;
                    foreach (var item in crumbs)
                    {
                        if (item.PathType == PathType.Drive)
                        {
                            var drive = await PlatformGetDrive(item.Path);
                            if (drive != null)
                            {
                                currentFolder = drive.Context.Context as StorageFolder;
                            }
                        }
                        else if (item.PathType == PathType.Folder)
                        {
                            if (currentFolder != null)
                            {
                                currentFolder = await currentFolder.GetFolderAsync(item.DisplayName);
                            }
                        }
                    }

                    if (currentFolder != null)
                    {
                        var file = await currentFolder.GetFileAsync(fileName);
                        if (file != null)
                        {
                            var pfile = await StorageService.GetFile(StorageType.Portable, file) as PortableFile;
                            pfile.Path = path;
                            return pfile;
                        }
                    }
                    return null;
                }
                else
                {
                    return FileInterop.GetFile(StorageType.Portable, path) as PortableFile;
                }
            }
            catch (ArgumentException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Path is not valid: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (FileNotFoundException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Folder does not exist: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (UnauthorizedAccessException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return null;
            }
        }

    }
}
