using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Interop;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models.Extensions;
using Nito.AsyncEx;
using Windows.Devices.Enumeration;
using Windows.Devices.Portable;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class PortableStorageService
    {
        private static readonly AsyncLock mutex;
        private static readonly AsyncLazy<Task> lazyInitialize;
        private static readonly List<PortableDrive> drives;
        private static readonly DeviceWatcher deviceWatcher = null;

        static PortableStorageService()
        {
            mutex = new AsyncLock();
            lazyInitialize = new AsyncLazy<Task>(Initialize);
            drives = new List<PortableDrive>();
            deviceWatcher = DeviceInformation.CreateWatcher(StorageDevice.GetDeviceSelector());
            deviceWatcher.EnumerationCompleted += OnEnumerationCompleted;
            deviceWatcher.Added += OnDeviceAdded;
            deviceWatcher.Removed += OnDeviceRemoved;
            deviceWatcher.Updated += OnDeviceUpdated;
            deviceWatcher.Start();
        }

        #region Monitor Devices

        static async Task<Task> Initialize()
        {
            using (await mutex.LockAsync())
            {
                var portableDevices = await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector());
                foreach (var item in portableDevices)
                {
                    var drive = await PlatformGetDriveFromId(item.Id);
                    if (drive != null)
                    {
                        drives.Add(drive);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private static NotifyCollectionChangedAction AddOrUpdate(PortableDrive drive)
        {
            var index = drives.FindIndex(i => i.DeviceId == drive.DeviceId);
            if (index == -1)
            {
                drives.Add(drive);
                return NotifyCollectionChangedAction.Add;
            }
            else
            {
                drives[index] = drive;
                return NotifyCollectionChangedAction.Replace;
            }
        }

        private static async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                var drive = await PlatformGetDriveFromId(args.Id);
                if (drive != null)
                {
                    var action = AddOrUpdate(drive);
                    PortableDriveCollectionChanged?.Invoke(null, new PortableDriveCollectionChangedEventArgs(action, args.Id, drive));
                }
            }
        }

        private static async void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                var index = drives.FindIndex(i => i.DeviceId == args.Id);
                if (index != -1)
                {
                    drives.RemoveAt(index);
                    PortableDriveCollectionChanged?.Invoke(null, new PortableDriveCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, args.Id, null));
                }
            }
        }

        private static async void OnDeviceUpdated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                var drive = await PlatformGetDriveFromId(args.Id);
                if (drive != null)
                {
                    var action = AddOrUpdate(drive);
                    PortableDriveCollectionChanged?.Invoke(null, new PortableDriveCollectionChangedEventArgs(action, args.Id, drive));
                }
            }
        }

        private static async void OnEnumerationCompleted(DeviceWatcher sender, object args)
        {
            await lazyInitialize;
        }

        #endregion

        static async Task<IList<PortableDrive>> PlatformGetDrives()
        {
            await lazyInitialize;

            return drives;
        }

        static async Task<IList<string>> PlatformGetDrivePaths()
        {
            await lazyInitialize;

            return drives.Select(i => i.Path).ToList();
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
                        if (item.AppPath == AppPath.Drive)
                        {
                            var drive = await PlatformGetDrive(item.Path);
                            if (drive != null)
                            {
                                currentFolder = drive.Context.Context as StorageFolder;
                            }
                        }
                        else if (item.AppPath == AppPath.Folder)
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
                        if (item.AppPath == AppPath.Drive)
                        {
                            var drive = await PlatformGetDrive(item.Path);
                            if (drive != null)
                            {
                                currentFolder = drive.Context.Context as StorageFolder;
                            }
                        }
                        else if (item.AppPath == AppPath.Folder)
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
