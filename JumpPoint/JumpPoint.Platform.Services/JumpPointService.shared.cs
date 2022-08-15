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

        static JumpPointService()
        {
            DataFolder = Directory.CreateDirectory(Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "Data")).FullName;
        }

        public static string DataFolder { get; }

        public static async Task Initialize()
        {
            await FolderTemplateService.Initialize();
            await AppLinkService.Initialize();
            await WorkspaceService.Initialize();
            await CloudStorageService.Initialize();
            await DashboardService.Initialize();
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

        public static async Task<string> Rename(JumpPointItem item, string name, RenameOption option)
        {
            switch (item.Type)
            {
                case JumpPointItemType.File:
                case JumpPointItemType.Folder:
                case JumpPointItemType.Drive:
                    return await StorageService.Rename((StorageItemBase)item, name, option);

                case JumpPointItemType.Workspace:
                    return await WorkspaceService.Rename((Workspace)item, name, option);

                case JumpPointItemType.AppLink:
                    return await AppLinkService.Rename((AppLink)item, name, option);

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    return string.Empty;
            }
        }

        public static async Task Rename(IList<JumpPointItem> items, string name, RenameOption option)
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

        public static Uri GetAppUri(AppPath pathType, string path)
        {
            var protocolPath = pathType.ToProtocolPath();
            var uriBuilder = new UriBuilder
            {
                Scheme = Prefix.MAIN_SCHEME,
                Host = protocolPath.ToString().ToLower()
            };
            switch (protocolPath)
            {
                case ProtocolPath.Dashboard:
                case ProtocolPath.Settings:
                case ProtocolPath.Favorites:
                case ProtocolPath.Drives:
                case ProtocolPath.CloudDrives:
                case ProtocolPath.Workspaces:
                case ProtocolPath.AppLinks:
                case ProtocolPath.Chat:
                case ProtocolPath.Clipboard:
                    return uriBuilder.Uri;

                case ProtocolPath.Open:
                case ProtocolPath.Drive:
                case ProtocolPath.Folder:
                case ProtocolPath.Workspace:
                    uriBuilder.Query = new QueryString()
                    {
                        { "path", path }
                    }.ToString();
                    return uriBuilder.Uri;

                case ProtocolPath.Cloud:
                    uriBuilder.Query = new QueryString()
                    {
                        { "provider", CloudStorageService.GetProvider(path).ToString() }
                    }.ToString();
                    return uriBuilder.Uri;
                case ProtocolPath.Properties:
                case ProtocolPath.Unknown:
                default:
                    return null;
            }
        }

        public static async Task OpenNewWindow(AppPath pathType, string path)
        {
            var uri = GetAppUri(pathType, path);
            if (uri != null)
            {
                await Xamarin.Essentials.Launcher.OpenAsync(uri);
            }
        }

        public static async Task<bool> OpenFile(FileBase file, bool useDefaultHandler)
            => await PlatformOpenFile(file, useDefaultHandler);

        public static async Task OpenInFileExplorer(string path, IEnumerable<StorageItemBase> itemsToSelect = null)
            => await PlatformOpenInFileExplorer(path, itemsToSelect);

        public static async Task OpenProperties(Collection<Seed> seeds)
            => await PlatformOpenProperties(seeds);

        public static async Task<bool> Rate()
            => await PlatformRate();

    }
}
