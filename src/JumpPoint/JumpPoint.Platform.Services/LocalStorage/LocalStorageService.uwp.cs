using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items.LocalStorage;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Interop;
using NittyGritty.Extensions;
using Windows.Storage;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class LocalStorageService
    {

        static async Task<IList<LocalDrive>> PlatformGetDrives()
        {
            var drives = new List<LocalDrive>();
            var driveInfos = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed || d.DriveType == DriveType.CDRom);
            var removableDrivePaths = await PortableStorageService.GetDrivePaths();
            foreach (var driveInfo in driveInfos)
            {
                var root = driveInfo.RootDirectory.FullName;
                var d = await PlatformGetDrive(root);
                if (d != null && !removableDrivePaths.Contains(root))
                {
                    drives.Add(d);
                }
            }
            return drives;
        }

        static async Task<IList<StorageItemBase>> PlatformGetItems(ILocalDirectory directory)
        {
            var items = new List<StorageItemBase>();
            foreach (var item in FileInterop.GetItems(StorageType.Local, directory.Path))
            {
                if (item is StorageItemBase i)
                {
                    items.Add(i);
                }
            }
            return await Task.FromResult(items);
        }

        static async Task<LocalDrive> PlatformGetDrive(string path)
        {
            try
            {
                var driveTemplate = StorageService.GetDriveTemplate(path);
                if (driveTemplate != DriveTemplate.Unknown)
                {
                    var item = await StorageFolder.GetFolderFromPathAsync(path);
                    if (item != null)
                    {
                        var drive = await StorageService.GetDrive(StorageType.Local, item);
                        drive.DriveTemplate = driveTemplate;
                        return drive as LocalDrive;
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

        static async Task<LocalFolder> PlatformGetFolder(string path)
        {
            try
            {
                return await Task.FromResult(FileInterop.GetFolder(StorageType.Local, path) as LocalFolder);
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

        static async Task<LocalFile> PlatformGetFile(string path)
        {
            try
            {
                return await Task.FromResult(FileInterop.GetFile(StorageType.Local, path) as LocalFile);
            }
            catch (ArgumentException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"Path is not valid: {path}"), MessengerTokens.ExceptionManagement);
                return null;
            }
            catch (FileNotFoundException ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, $"File does not exist: {path}"), MessengerTokens.ExceptionManagement);
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
