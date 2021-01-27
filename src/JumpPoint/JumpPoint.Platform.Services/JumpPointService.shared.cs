using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using NittyGritty.Models;
using NittyGritty.Services;
using NGStorage = NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class JumpPointService
    {
#if BETA
        private static readonly string SCHEME = "jumppoint-beta";
#else
        private static readonly string SCHEME = "jumppoint";
#endif
        private static readonly LauncherService launcherService;

        static JumpPointService()
        {
            launcherService = new LauncherService();
            DataFolder = Directory.CreateDirectory(Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Data")).FullName;
        }

        public static string DataFolder { get; }

        public static async Task Initialize()
        {
            await DashboardService.Initialize();
            await FolderTemplateService.Initialize();
            await WorkspaceService.Initialize();
            await AppLinkService.Initialize();
            
        }

        public static async Task Load(JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File when item is FileBase file:
                    await StorageService.Load(file);
                    break;

                case JumpPointItemType.Folder when item is FolderBase folder:
                    await StorageService.Load(folder);
                    break;

                case JumpPointItemType.Drive when item is DriveBase drive:
                    await StorageService.Load(drive);
                    break;

                case JumpPointItemType.Workspace when item is Workspace workspace:
                    await WorkspaceService.Load(workspace);
                    break;

                case JumpPointItemType.SettingLink when item is SettingLink settingLink:
                    await SettingLinkService.Load(settingLink);
                    break;

                case JumpPointItemType.AppLink when item is AppLink appLink:
                    await AppLinkService.Load(appLink);
                    break;

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }
    
        public static async Task<JumpPointItem> Create(object info)
        {
            switch (info)
            {


                case WorkspaceInfo workspaceInfo:
                    return await WorkspaceService.Create(workspaceInfo);

                case AppLinkInfo appLinkInfo:
                    return await AppLinkService.Create(appLinkInfo);

                default:
                    return null;
            }
        }

        public static async Task Rename(JumpPointItem item, string name, RenameCollisionOption option)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                case JumpPointItemType.Folder:
                case JumpPointItemType.Drive:
                    await StorageService.Rename((StorageItemBase)item, name, option);
                    break;

                case JumpPointItemType.Workspace:
                    await WorkspaceService.Rename((Workspace)item, name, option);
                    break;

                case JumpPointItemType.AppLink:
                    await AppLinkService.Rename((AppLink)item, name, option);
                    break;

                case JumpPointItemType.Library:
                case JumpPointItemType.SettingLink:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }

        public static async Task Rename(IList<JumpPointItem> items, string name, RenameCollisionOption option)
        {
            foreach (var item in items)
            {
                await Rename(item, name, option);
            }
        }

        public static async Task Delete(JumpPointItem item, bool deletePermanently)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                case JumpPointItemType.Folder:
                case JumpPointItemType.Drive:
                    await StorageService.Delete(new List<StorageItemBase> { (StorageItemBase)item }, deletePermanently);
                    break;

                case JumpPointItemType.Workspace:
                    await WorkspaceService.Delete((Workspace)item, deletePermanently);
                    break;

                case JumpPointItemType.AppLink:
                    await AppLinkService.Delete((AppLink)item);
                    break;

                case JumpPointItemType.Library:
                case JumpPointItemType.SettingLink:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }
    
        public static async Task Delete(IList<JumpPointItem> items, bool deletePermanently)
        {
            var storageItems = items.OfType<StorageItemBase>().ToList();
            if (storageItems.Count > 0)
            {
                await StorageService.Delete(storageItems, deletePermanently);
            }

            var workspaces = items.OfType<Workspace>().ToList();
            if (workspaces.Count > 0)
            {
                await WorkspaceService.Delete(workspaces, deletePermanently);
            }

            var appLinks = items.OfType<AppLink>().ToList();
            if (appLinks.Count > 0)
            {
                await AppLinkService.Delete(appLinks);
            }
        }

        public static Task<NGStorage.IStorageItem> Convert(StorageItemBase item)
            => PlatformConvert(item);

        public static Task<IList<NGStorage.IStorageItem>> Convert(IEnumerable<JumpPointItem> items)
            => PlatformConvert(items);

        public static Uri GetAppUri(PathType pathType, string path)
        {
            var uriBuilder = new UriBuilder
            {
                Scheme = SCHEME,
                Host = pathType.ToString().ToLower()
            };
            switch (pathType)
            {
                case PathType.Drive:
                case PathType.Folder:
                case PathType.Workspace:
                    uriBuilder.Query = new QueryString()
                    {
                        { "path", path }
                    }.ToString();
                    return uriBuilder.Uri;

                case PathType.Dashboard:
                case PathType.Settings:
                case PathType.Favorites:
                case PathType.Drives:
                case PathType.Workspaces:
                case PathType.AppLinks:
                case PathType.SettingLinks:
                    return uriBuilder.Uri;

                case PathType.Properties:
                case PathType.Libraries:
                case PathType.Library:
                case PathType.Unknown:
                case PathType.File:
                default:
                    return null;
            }
        }

        public static async Task OpenNewWindow(PathType pathType, string path)
        {
            var uri = GetAppUri(pathType, path);
            if (uri != null)
            {
                await launcherService.LaunchUri(uri);
            }
        }

        public static async Task<bool> OpenFile(FileBase file, bool useDefaultHandler)
            => await PlatformOpenFile(file, useDefaultHandler);

        public static async Task OpenInFileExplorer(string path, IEnumerable<StorageItemBase> itemsToSelect = null)
            => await PlatformOpenInFileExplorer(path, itemsToSelect);

        public static async Task OpenProperties(Collection<Seed> seeds)
            => await PlatformOpenProperties(seeds);

    }
}
