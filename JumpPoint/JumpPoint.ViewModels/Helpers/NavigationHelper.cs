using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight.Ioc;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Models;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels.Helpers
{
    public static class NavigationHelper
    {
        private static readonly INavigationService navigationService;

        static NavigationHelper()
        {
            navigationService = null;
        }

        public static void ToKey(string key, object parameter = null)
        {
            navigationService.NavigateTo(key, parameter);
        }

        public static void ToDirectory(AppPath pathType, StorageType storageType, string path)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    navigationService.NavigateTo(ViewModelKeys.Folder, new QueryString
                    {
                        { nameof(PathInfo.Type), AppPath.Folder.ToString() },
                        { nameof(FolderBase.StorageType), storageType.ToString() },
                        { nameof(PathInfo.Path), path }
                    }.ToString());
                    break;

                case AppPath.Drive:
                    navigationService.NavigateTo(ViewModelKeys.Drive, new QueryString
                    {
                        { nameof(PathInfo.Type), AppPath.Drive.ToString() },
                        { nameof(DriveBase.StorageType), storageType.ToString() },
                        { nameof(PathInfo.Path), path }
                    }.ToString());
                    break;

                default:
                    break;
            }
        }

        public static void ToDrive(DriveBase drive)
        {
            navigationService.NavigateTo(ViewModelKeys.Drive, GetParameter(drive));
        }

        public static void ToFolder(FolderBase folder)
        {
            navigationService.NavigateTo(ViewModelKeys.Folder, GetParameter(folder));
        }

        public static void ToWorkspace(Workspace workspace)
        {
            navigationService.NavigateTo(ViewModelKeys.Workspace, GetParameter(workspace));
        }

        public static void ToCloudStorage(CloudStorageProvider provider)
        {
            navigationService.NavigateTo(ViewModelKeys.Cloud, GetParameter(provider));
        }

        public static void ToLibrary(Library library)
        {
            navigationService.NavigateTo(ViewModelKeys.Library, GetParameter(library));
        }

        public static void ToPathType(AppPath pathType, object parameter = null)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    navigationService.NavigateTo(ViewModelKeys.Folder, parameter);
                    break;

                case AppPath.Drive:
                    navigationService.NavigateTo(ViewModelKeys.Drive, parameter);
                    break;

                case AppPath.Workspace:
                    navigationService.NavigateTo(ViewModelKeys.Workspace, parameter);
                    break;

                case AppPath.Cloud:
                    navigationService.NavigateTo(ViewModelKeys.Cloud, parameter);
                    break;

                case AppPath.Favorites:
                    navigationService.NavigateTo(ViewModelKeys.Favorites);
                    break;

                case AppPath.Drives:
                    navigationService.NavigateTo(ViewModelKeys.Drives);
                    break;

                case AppPath.Workspaces:
                    navigationService.NavigateTo(ViewModelKeys.Workspaces);
                    break;

                case AppPath.AppLinks:
                    navigationService.NavigateTo(ViewModelKeys.AppLinks);
                    break;

                case AppPath.Settings:
                    navigationService.NavigateTo(ViewModelKeys.Settings);
                    break;

                case AppPath.CloudDrives:
                    navigationService.NavigateTo(ViewModelKeys.CloudDrives);
                    break;

                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Chat:
                case AppPath.Unknown:
                default:
                    navigationService.NavigateTo(ViewModelKeys.Dashboard);
                    break;
            }
        }

        public static void ToBreadcrumb(Breadcrumb breadcrumb, JumpPointItem item)
        {
            ToPathType(breadcrumb.AppPath, GetParameter(breadcrumb.AppPath, breadcrumb.Path, item));
        }

        public static void GoBack()
        {
            navigationService.GoBack();
        }

        public static void GoForward()
        {
            navigationService.GoForward();
        }

        public static void GoUp(AppPath pathType)
        {
            switch (pathType)
            {
                case AppPath.Drive:
                    navigationService.NavigateTo(ViewModelKeys.Drives);
                    break;

                case AppPath.Workspace:
                    navigationService.NavigateTo(ViewModelKeys.Workspaces);
                    break;

                case AppPath.Cloud:
                    navigationService.NavigateTo(ViewModelKeys.CloudDrives);
                    break;

                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.Settings:
                case AppPath.CloudDrives:
                    navigationService.NavigateTo(ViewModelKeys.Dashboard);
                    break;

                case AppPath.Folder:
                case AppPath.Properties:
                case AppPath.Dashboard:
                case AppPath.Chat:
                case AppPath.Unknown:
                default:
                    break;
            }
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
