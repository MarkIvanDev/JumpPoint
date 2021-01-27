using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items.NetworkStorage;
using JumpPoint.Platform.Items.Storage;
using Windows.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class NetworkStorageService
    {
        
        static async Task<IList<NetworkDrive>> PlatformGetDrives()
        {
            var drives = new List<NetworkDrive>();
            var driveInfos = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Network);
            foreach (var driveInfo in driveInfos)
            {
                var d = await PlatformGetDrive(driveInfo.RootDirectory.FullName);
                if (d != null)
                {
                    drives.Add(d);
                }
            }
            return drives;
        }

        static async Task<IList<StorageItemBase>> PlatformGetItems(INetworkDirectory directory)
        {
            var items = new List<StorageItemBase>();
            var networkDirectory = await StorageFolder.GetFolderFromPathAsync(directory.Path);
            var networkItems = await networkDirectory.GetItemsAsync();

            foreach (var item in networkItems)
            {
                if (item.IsOfType(StorageItemTypes.File))
                {
                    var i = await StorageService.GetFile(StorageType.Network, item as StorageFile);
                    if (i is NetworkFile f)
                    {
                        items.Add(f);
                    }
                }
                else if (item.IsOfType(StorageItemTypes.Folder))
                {
                    var i = await StorageService.GetFolder(StorageType.Network, item as StorageFolder);
                    if (i is NetworkFolder f)
                    {
                        items.Add(f);
                    }
                }
            }
            return items;
        }

        static async Task<IList<NetworkFolder>> PlatformGetFolders(INetworkDirectory directory)
        {
            var items = new List<NetworkFolder>();
            var networkDirectory = await StorageFolder.GetFolderFromPathAsync(directory.Path);
            var networkFolders = await networkDirectory.GetFoldersAsync();

            foreach (var item in networkFolders)
            {
                var i = await StorageService.GetFolder(StorageType.Network, item);
                if (i is NetworkFolder f)
                {
                    items.Add(f);
                }
            }
            return items;
        }

        static async Task<IList<NetworkFile>> PlatformGetFiles(INetworkDirectory directory)
        {
            var items = new List<NetworkFile>();
            var networkDirectory = await StorageFolder.GetFolderFromPathAsync(directory.Path);
            var networkFiles = await networkDirectory.GetFilesAsync();

            foreach (var item in networkFiles)
            {
                var i = await StorageService.GetFile(StorageType.Network, item);
                if (i is NetworkFile f)
                {
                    items.Add(f);
                }
            }
            return items;
        }

        static async Task<NetworkDrive> PlatformGetDrive(string path)
        {
            try
            {
                var item = await StorageFolder.GetFolderFromPathAsync(path);
                if (item != null)
                {
                    var drive = await StorageService.GetDrive(StorageType.Network, item);
                    return drive as NetworkDrive;
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

        static async Task<NetworkFolder> PlatformGetFolder(string path)
        {
            try
            {
                var folder = await StorageFolder.GetFolderFromPathAsync(path);
                return await StorageService.GetFolder(StorageType.Network, folder) as NetworkFolder;
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

        static async Task<NetworkFile> PlatformGetFile(string path)
        {
            try
            {
                var file = await StorageFile.GetFileFromPathAsync(path);
                return await StorageService.GetFile(StorageType.Network, file) as NetworkFile;
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
