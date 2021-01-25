using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.LocalStorage;
using JumpPoint.Platform.Items.NetworkStorage;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Storage.Properties;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Utilities;
using NittyGritty.Extensions;
using NittyGritty.Platform.Storage;
using NittyGritty.Services;

namespace JumpPoint.Platform.Services
{
    public static partial class StorageService
    {
        private static readonly FileService fileService;

        static StorageService()
        {
            fileService = new FileService();
        }

        public static async Task<StorageType?> GetPathStorageType(string path)
        {
            var kind = PathUtilities.GetPathKind(path);
            switch (kind)
            {
                case PathKind.Mounted:
                {
                    var driveRoot = Path.GetPathRoot(path);
                    var drive = new DriveInfo(driveRoot);
                    switch (drive.DriveType)
                    {
                        case DriveType.CDRom:
                        case DriveType.Fixed:
                            if (Path.GetPathRoot(Environment.SystemDirectory).NormalizeDirectory() == driveRoot.NormalizeDirectory())
                            {
                                return StorageType.Local;
                            }
                            else
                            {
                                var removableDrivePaths = await PortableStorageService.GetDrivePaths();
                                return removableDrivePaths.Contains(Path.GetPathRoot(path)) ?
                                    StorageType.Portable : StorageType.Local;
                            }

                        case DriveType.Network:
                            return StorageType.Network;

                        case DriveType.Removable:
                            return StorageType.Portable;

                        case DriveType.NoRootDirectory:
                        case DriveType.Ram:
                        case DriveType.Unknown:
                        default:
                            return null;
                    }
                }

                case PathKind.Unmounted:
                    return StorageType.Portable;

                case PathKind.Network:
                    return StorageType.Network;

                case PathKind.Unknown:
                default:
                    return null;
            }
        }

        public static async Task Rename(StorageItemBase item, string name, RenameCollisionOption option)
        {
            try
            {
                switch (item.StorageType)
                {
                    case StorageType.Local:
                    case StorageType.Portable:
                    case StorageType.Network:
                        await PlatformRename(item, name, option);
                        break;

                    case StorageType.Cloud:
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
        }

        public static async Task Delete(IList<string> paths, bool deletePermanently)
        {
            var groups = paths.GroupBy(p => p.StartsWith(@"\\?\")).ToDictionary(g => g.Key, g => g.ToList());

            if (groups.TryGetValue(false, out var mountedPaths))
            {
                await DesktopService.Delete(mountedPaths, deletePermanently);
            }

            if (groups.TryGetValue(true, out var unmountedPaths))
            {
                var unmountedItems = new List<StorageItemBase>();
                foreach (var path in unmountedPaths)
                {
                    var item = await GetFile(path);
                    if (item != null)
                    {
                        unmountedItems.Add(item);
                    }
                }
                await Delete(unmountedItems, deletePermanently);
            }
        }

        public static async Task Delete(IList<StorageItemBase> items, bool deletePermanently)
        {
            var groups = items.GroupBy(i => i.StorageType).ToDictionary(g => g.Key, g => g.ToList());

            var desktopDelete = new List<StorageItemBase>();
            var ngDelete = new List<IStorageItem>();
            if (groups.TryGetValue(StorageType.Local, out var localItems))
            {
                desktopDelete.AddRange(localItems);
            }
            if (groups.TryGetValue(StorageType.Network, out var networkItems))
            {
                desktopDelete.AddRange(networkItems);
            }
            if (groups.TryGetValue(StorageType.Portable, out var portableItems))
            {
                foreach (var item in portableItems)
                {
                    if (item.Path.StartsWith(@"\\?\"))
                    {
                        if (item is PortableFile pf && pf.Context != null)
                        {
                            ngDelete.Add(pf.Context);
                        }
                        else if (item is IPortableDirectory pd && pd.Context != null)
                        {
                            ngDelete.Add(pd.Context);
                        }
                    }
                    else
                    {
                        desktopDelete.Add(item);
                    }
                }
            }

            if (desktopDelete.Count > 0)
            {
                await DesktopService.Delete(desktopDelete.Select(i => i.Path).ToList(), deletePermanently);
            }

            if (ngDelete.Count > 0)
            {
                foreach (var item in ngDelete)
                {
                    await fileService.Delete(item, deletePermanently);
                }
            }

            if (groups.TryGetValue(StorageType.Cloud, out var cloudItems))
            {

            }
        }

        #region Directory

        public static async Task<DirectoryBase> GetDirectory(string path)
        {
            var crumbs = PathUtilities.GetCrumbs(path);
            if (crumbs.Count == 1 && crumbs[0].PathType == PathType.Drive)
            {
                return await GetDrive(path);
            }
            else if (crumbs.Count > 1)
            {
                return await GetFolder(path);
            }
            return null;
        }

        public static async Task<IList<FolderBase>> GetFolders(DirectoryBase directory)
        {
            var folders = new List<FolderBase>();

            switch (directory.StorageType)
            {
                case StorageType.Local when directory is ILocalDirectory localDirectory:
                    folders.AddRange(await LocalStorageService.GetFolders(localDirectory));
                    break;

                case StorageType.Portable when directory is IPortableDirectory portableDirectory:
                    folders.AddRange(await PortableStorageService.GetFolders(portableDirectory));
                    break;

                case StorageType.Network when directory is INetworkDirectory networkDirectory:
                    folders.AddRange(await NetworkStorageService.GetFolders(networkDirectory));
                    break;

                case StorageType.Cloud:
                default:
                    break;
            }

            return folders;
        }

        public static async Task<IList<FileBase>> GetFiles(DirectoryBase directory)
        {
            var files = new List<FileBase>();

            switch (directory.StorageType)
            {
                case StorageType.Local when directory is ILocalDirectory localDirectory:
                    files.AddRange(await LocalStorageService.GetFiles(localDirectory));
                    break;

                case StorageType.Portable when directory is IPortableDirectory portableDirectory:
                    files.AddRange(await PortableStorageService.GetFiles(portableDirectory));
                    break;

                case StorageType.Network when directory is INetworkDirectory networkDirectory:
                    files.AddRange(await NetworkStorageService.GetFiles(networkDirectory));
                    break;

                case StorageType.Cloud:
                default:
                    break;
            }

            return files;
        }

        public static async Task<FolderBase> CreateFolder(DirectoryBase directory, string name)
        {
            try
            {
                switch (directory.StorageType)
                {
                    case StorageType.Local:
                    case StorageType.Portable:
                    case StorageType.Network:
                        return await PlatformCreateFolder(directory, name);

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

        public static async Task<FileBase> CreateFile(DirectoryBase directory, string name)
        {
            try
            {
                switch (directory.StorageType)
                {
                    case StorageType.Local:
                    case StorageType.Portable:
                    case StorageType.Network:
                        return await PlatformCreateFile(directory, name);

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

        public static async Task Paste(DirectoryBase directory)
        {
            await DesktopService.Paste(directory.Path);
        }

        #endregion

        #region Drive

        public static async Task<IList<DriveBase>> GetDrives()
        {
            var drives = new List<DriveBase>();
            drives.AddRange(await LocalStorageService.GetDrives());
            drives.AddRange(await PortableStorageService.GetDrives());
            drives.AddRange(await NetworkStorageService.GetDrives());
            return drives;
        }

        public static async Task Load(DriveBase drive)
        {
            drive.IsFavorite = await DashboardService.GetStatus(drive);
        }

        public static async Task<DriveBase> GetDrive(string path, StorageType? storageType = null)
        {
            switch (storageType ?? await GetPathStorageType(path))
            {
                case StorageType.Local:
                    return await LocalStorageService.GetDrive(path);

                case StorageType.Portable:
                    return await PortableStorageService.GetDrive(path);

                case StorageType.Network:
                    return await NetworkStorageService.GetDrive(path);

                case StorageType.Cloud:
                default:
                    return null;
            }
        }

        public static DriveTemplate GetDriveTemplate(string path)
        {
            try
            {
                if (path.StartsWith(@"\\?\"))
                {
                    return DriveTemplate.Removable;
                }

                if (path.StartsWith(@"\\"))
                {
                    return DriveTemplate.Network;
                }

                var driveInfo = new DriveInfo(path);
                var driveRoot = driveInfo.RootDirectory.FullName.NormalizeDirectory();
                if (driveRoot == path.NormalizeDirectory())
                {
                    switch (driveInfo.DriveType)
                    {
                        case DriveType.CDRom:
                            return DriveTemplate.Optical;

                        case DriveType.Fixed:
                            return Path.GetPathRoot(Environment.SystemDirectory).NormalizeDirectory() == driveRoot ?
                                DriveTemplate.System :
                                DriveTemplate.Local;

                        case DriveType.Network:
                            return DriveTemplate.Network;

                        case DriveType.Removable:
                            return DriveTemplate.Removable;

                        case DriveType.NoRootDirectory:
                        case DriveType.Ram:
                        case DriveType.Unknown:
                        default:
                            return DriveTemplate.Unknown;
                    }
                }
                else
                {
                    return DriveTemplate.Unknown;
                }
            }
            catch (Exception)
            {
                return DriveTemplate.Unknown;
            }
        }

        #endregion

        #region Folder

        public static async Task Load(FolderBase folder)
        {
            folder.FolderTemplate = await FolderTemplateService.GetFolderTemplate(folder);
            folder.IsFavorite = await DashboardService.GetStatus(folder);
        }

        public static async Task<FolderBase> GetFolder(string path, StorageType? storageType = null)
        {
            switch (storageType ?? await GetPathStorageType(path))
            {
                case StorageType.Local:
                    return await LocalStorageService.GetFolder(path);

                case StorageType.Portable:
                    return await PortableStorageService.GetFolder(path);

                case StorageType.Network:
                    return await NetworkStorageService.GetFolder(path);

                case StorageType.Cloud:
                default:
                    return null;
            }
        }

        public static async Task<FolderBase> GetUserFolder(UserFolderTemplate template)
        {
            if (FolderTemplateService.TryGetPath(template, out var path))
            {
                var uf = await GetFolder(path);
                if (!(uf is null))
                {
                    uf.FolderType = FolderType.User;
                    uf.FolderTemplate = (FolderTemplate)template;
                    return uf;
                }
            }
            return null;
        }

        public static async Task<FolderBase> GetSystemFolder(SystemFolderTemplate template)
        {
            if (FolderTemplateService.TryGetPath(template, out var path))
            {
                var sf = await GetFolder(path);
                if (!(sf is null))
                {
                    sf.FolderTemplate = (FolderTemplate)template;
                    return sf;
                }
            }
            return null;
        }

        public static async Task<ulong?> GetFolderSize(FolderBase folder)
        {
            try
            {
                ulong size = 0;
                var cfsFiles = await GetFiles(folder);
                foreach (var fi in cfsFiles)
                {
                    size += fi.Size ?? 0;
                }

                var cfsFolders = await GetFolders(folder);
                foreach (var fo in cfsFolders)
                {
                    size += await GetFolderSize(fo) ?? 0;
                }
                return size;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region File

        public static async Task Load(FileBase file)
        {
            file.IsFavorite = await DashboardService.GetStatus(file);

            switch (file.StorageType)
            {
                case StorageType.Local:
                case StorageType.Portable:
                case StorageType.Network:
                    await PlatformLoad(file);
                    break;

                case StorageType.Cloud:
                default:
                    break;
            }
        }

        public static async Task LoadProperties(FileBase file, FileBaseProperties properties)
        {
            if (properties == FileBaseProperties.None)
                return;

            switch (file.StorageType)
            {
                case StorageType.Local:
                case StorageType.Portable:
                case StorageType.Network:
                    await PlatformLoadProperties(file, properties);
                    break;

                case StorageType.Cloud:
                default:
                    break;
            }
        }

        public static async Task<FileBase> GetFile(string path, StorageType? storageType = null)
        {
            switch (storageType ?? await GetPathStorageType(path))
            {
                case StorageType.Local:
                    return await LocalStorageService.GetFile(path);

                case StorageType.Portable:
                    return await PortableStorageService.GetFile(path);

                case StorageType.Network:
                    return await NetworkStorageService.GetFile(path);

                case StorageType.Cloud:
                default:
                    return null;
            }
        }

        #endregion
    }
}
