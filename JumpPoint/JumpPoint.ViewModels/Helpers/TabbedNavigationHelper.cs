using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Models;
using NittyGritty.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.ViewModels.Helpers
{
    public class TabbedNavigationHelper
    {
        public string Key { get; }

        public INavigationService NavigationService { get; }

        public TabbedNavigationHelper(string key, INavigationService navigationService)
        {
            Key = key;
            NavigationService = navigationService;
        }

        public void ToKey(string key, object parameter = null)
        {
            NavigationService.NavigateTo(key, GetTabParameter(parameter));
        }

        public void ToDirectory(AppPath pathType, StorageType storageType, string path)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    NavigationService.NavigateTo(ViewModelKeys.Folder, GetTabParameter(
                        new QueryString
                        {
                            { nameof(PathInfo.Type), AppPath.Folder.ToString() },
                            { nameof(FolderBase.StorageType), storageType.ToString() },
                            { nameof(PathInfo.Path), path }
                        }.ToString()
                    ));
                    break;

                case AppPath.Drive:
                    NavigationService.NavigateTo(ViewModelKeys.Drive, GetTabParameter(
                        new QueryString
                        {
                            { nameof(PathInfo.Type), AppPath.Drive.ToString() },
                            { nameof(DriveBase.StorageType), storageType.ToString() },
                            { nameof(PathInfo.Path), path }
                        }.ToString()
                    ));
                    break;

                default:
                    break;
            }
        }

        public void ToDrive(DriveBase drive)
        {
            NavigationService.NavigateTo(ViewModelKeys.Drive, GetTabParameter(GetParameter(drive)));
        }

        public void ToFolder(FolderBase folder)
        {
            NavigationService.NavigateTo(ViewModelKeys.Folder, GetTabParameter(GetParameter(folder)));
        }

        public void ToWorkspace(Workspace workspace)
        {
            NavigationService.NavigateTo(ViewModelKeys.Workspace, GetTabParameter(GetParameter(workspace)));
        }

        public void ToCloudStorage(CloudStorageProvider provider)
        {
            NavigationService.NavigateTo(ViewModelKeys.Cloud, GetTabParameter(GetParameter(provider)));
        }

        public void ToLibrary(Library library)
        {
            NavigationService.NavigateTo(ViewModelKeys.Library, GetTabParameter(GetParameter(library)));
        }

        public void ToPathType(AppPath pathType, object parameter = null)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    NavigationService.NavigateTo(ViewModelKeys.Folder, GetTabParameter(parameter));
                    break;

                case AppPath.Drive:
                    NavigationService.NavigateTo(ViewModelKeys.Drive, GetTabParameter(parameter));
                    break;

                case AppPath.Workspace:
                    NavigationService.NavigateTo(ViewModelKeys.Workspace, GetTabParameter(parameter));
                    break;

                case AppPath.Cloud:
                    NavigationService.NavigateTo(ViewModelKeys.Cloud, GetTabParameter(parameter));
                    break;

                case AppPath.Favorites:
                    NavigationService.NavigateTo(ViewModelKeys.Favorites, GetTabParameter());
                    break;

                case AppPath.Drives:
                    NavigationService.NavigateTo(ViewModelKeys.Drives, GetTabParameter());
                    break;

                case AppPath.Workspaces:
                    NavigationService.NavigateTo(ViewModelKeys.Workspaces, GetTabParameter());
                    break;

                case AppPath.AppLinks:
                    NavigationService.NavigateTo(ViewModelKeys.AppLinks, GetTabParameter());
                    break;

                case AppPath.Settings:
                    NavigationService.NavigateTo(ViewModelKeys.Settings, GetTabParameter());
                    break;

                case AppPath.CloudDrives:
                    NavigationService.NavigateTo(ViewModelKeys.CloudDrives, GetTabParameter());
                    break;

                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                case AppPath.Unknown:
                default:
                    NavigationService.NavigateTo(ViewModelKeys.Dashboard, GetTabParameter());
                    break;
            }
        }

        public void ToBreadcrumb(Breadcrumb breadcrumb, JumpPointItem item)
        {
            ToPathType(breadcrumb.AppPath, GetParameter(breadcrumb.AppPath, breadcrumb.Path, item));
        }

        public void GoBack()
        {
            NavigationService.GoBack();
        }

        public void GoForward()
        {
            NavigationService.GoForward();
        }

        public void GoUp(AppPath pathType)
        {
            switch (pathType)
            {
                case AppPath.Drive:
                    NavigationService.NavigateTo(ViewModelKeys.Drives, GetTabParameter());
                    break;

                case AppPath.Workspace:
                    NavigationService.NavigateTo(ViewModelKeys.Workspaces, GetTabParameter());
                    break;

                case AppPath.Cloud:
                    NavigationService.NavigateTo(ViewModelKeys.CloudDrives, GetTabParameter());
                    break;

                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.Settings:
                case AppPath.CloudDrives:
                    NavigationService.NavigateTo(ViewModelKeys.Dashboard, GetTabParameter());
                    break;

                case AppPath.Folder:
                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                case AppPath.Unknown:
                default:
                    break;
            }
        }

        public TabParameter GetTabParameter(object parameter = null)
        {
            return new TabParameter
            {
                TabKey = Key,
                Parameter = parameter
            };
        }

        public static string GetParameter(AppPath pathType, string path, JumpPointItem item)
        {
            var queryString = new QueryString()
            {
                { nameof(PathInfo.Type), pathType.ToString() },
                { nameof(PathInfo.Path), path }
            };
            if (item is StorageItemBase storageItem)
            {
                queryString.Add(nameof(storageItem.StorageType), storageItem.StorageType.ToString());
            }
            return queryString.ToString();
        }

        public static object GetParameter(JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.Drive when item is DriveBase drive:
                    return GetParameter(drive);

                case JumpPointItemType.Folder when item is FolderBase folder:
                    return GetParameter(folder);

                case JumpPointItemType.Workspace when item is Workspace workspace:
                    return GetParameter(workspace);

                case JumpPointItemType.File:
                case JumpPointItemType.AppLink:
                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    return null;
            }
        }

        private static object GetParameter(DriveBase drive)
        {
            var pathKind = drive.Path.GetPathKind();
            switch (pathKind)
            {
                case PathKind.Mounted:
                case PathKind.Network:
                    return new QueryString
                    {
                        { nameof(PathInfo.Type), AppPath.Drive.ToString() },
                        { nameof(DriveBase.StorageType), drive.StorageType.ToString() },
                        { nameof(PathInfo.Path), drive.Path }
                    }.ToString();

                case PathKind.Unmounted:
                case PathKind.Cloud:
                    return drive;

                case PathKind.Unknown:
                case PathKind.Local:
                case PathKind.Workspace:
                default:
                    return null;
            }
        }

        private static object GetParameter(FolderBase folder)
        {
            var pathKind = folder.Path.GetPathKind();
            switch (pathKind)
            {
                case PathKind.Mounted:
                case PathKind.Network:
                    return new QueryString
                    {
                        { nameof(PathInfo.Type), AppPath.Folder.ToString() },
                        { nameof(FolderBase.StorageType), folder.StorageType.ToString() },
                        { nameof(PathInfo.Path), folder.Path }
                    }.ToString();

                case PathKind.Unmounted:
                case PathKind.Cloud:
                    return folder;

                case PathKind.Unknown:
                case PathKind.Local:
                case PathKind.Workspace:
                default:
                    return null;
            }
        }

        private static object GetParameter(Workspace workspace)
        {
            return new QueryString
            {
                { nameof(PathInfo.Type), AppPath.Workspace.ToString() },
                { nameof(PathInfo.Path), workspace.Path }
            }.ToString();
        }

        public static object GetParameter(CloudStorageProvider provider)
        {
            return new QueryString
            {
                { nameof(PathInfo.Path), $@"cloud:\{provider}" }
            }.ToString();
        }

        public static object GetParameter(CloudAccount account)
        {
            return new QueryString
            {
                { nameof(PathInfo.Type), AppPath.Drive.ToString() },
                { nameof(DriveBase.StorageType), StorageType.Cloud.ToString() },
                { nameof(PathInfo.Path), $@"cloud:\{account.Provider}\{account.Name}" }
            }.ToString();
        }
    }

}
