using System;
using System.IO;
using System.Text;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.ViewModels;
using NittyGritty.Extensions;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace JumpPoint.Uwp.Converters
{
    public static class IconConverter
    {
        public static Uri GetItemIconUri(JumpPointItemType itemType)
        {
            return new Uri($@"ms-appx:///Assets/Icons/Items/{itemType}.png");
        }

        public static Uri GetDriveIconUri(DriveTemplate driveTemplate)
        {
            switch (driveTemplate)
            {
                case DriveTemplate.Local:
                    return new Uri($@"ms-appx:///Assets/Icons/Drives/HDD.png");

                case DriveTemplate.Optical:
                case DriveTemplate.Network:
                case DriveTemplate.Removable:
                case DriveTemplate.System:
                case DriveTemplate.HDD:
                case DriveTemplate.SSD:
                case DriveTemplate.USB:
                case DriveTemplate.SD:
                case DriveTemplate.MicroSD:
                case DriveTemplate.Phone:
                case DriveTemplate.Camera:
                    return new Uri($@"ms-appx:///Assets/Icons/Drives/{driveTemplate}.png");

                case DriveTemplate.CD:
                case DriveTemplate.CDR:
                case DriveTemplate.CDRW:
                    return new Uri($@"ms-appx:///Assets/Icons/Drives/CD.png");

                case DriveTemplate.DVD:
                case DriveTemplate.DVDRAM:
                case DriveTemplate.DVDPLUSR:
                case DriveTemplate.DVDPLUSRDL:
                case DriveTemplate.DVDPLUSRW:
                case DriveTemplate.DVDPLUSRWDL:
                case DriveTemplate.DVDDASHR:
                case DriveTemplate.DVDDASHRDL:
                case DriveTemplate.DVDDASHRW:
                case DriveTemplate.DVDDASHRWDL:
                case DriveTemplate.HDDVD:
                case DriveTemplate.HDDVDRAM:
                case DriveTemplate.HDDVDR:
                    return new Uri($@"ms-appx:///Assets/Icons/Drives/DVD.png");

                case DriveTemplate.BD:
                case DriveTemplate.BDR:
                case DriveTemplate.BDRE:
                    return new Uri($@"ms-appx:///Assets/Icons/Drives/BD.png");

                case DriveTemplate.Cloud:
                    return GetCloudStorageIconUri(CloudStorageProvider.Unknown);
                case DriveTemplate.OneDrive:
                    return GetCloudStorageIconUri(CloudStorageProvider.OneDrive);
                case DriveTemplate.Storj:
                    return GetCloudStorageIconUri(CloudStorageProvider.Storj);

                case DriveTemplate.Unknown:
                default:
                    return null;
            }
        }

        public static Uri GetFolderIconUri(FolderTemplate folderTemplate)
        {
            return new Uri($@"ms-appx:///Assets/Icons/Folders/{folderTemplate}.png");
        }

        public static string GetWorkspaceGlyph(WorkspaceTemplate workspaceTemplate)
        {
            return char.ConvertFromUtf32((int)workspaceTemplate);
        }

        public static Uri GetWorkspaceIconUri(WorkspaceTemplate workspaceTemplate)
        {
            return new Uri($@"ms-appx:///Assets/Icons/Workspaces/{workspaceTemplate}.png");
        }

        public static Uri GetCloudStorageIconUri(CloudStorageProvider cloudStorageProvider)
        {
            return new Uri($@"ms-appx:///Assets/Icons/Cloud/{cloudStorageProvider}.png");
        }

        public static Uri GetDashboardItemIconUri(Enum name)
        {
            if (name is UserFolderTemplate ufName)
            {
                return GetFolderIconUri((FolderTemplate)ufName);
            }
            else if (name is SystemFolderTemplate sfName)
            {
                return GetFolderIconUri((FolderTemplate)sfName);
            }
            return null;
        }

        public static Uri GetPathTypeIconUri(AppPath pathType)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    return GetItemIconUri(JumpPointItemType.Folder);

                case AppPath.Drive:
                    return GetItemIconUri(JumpPointItemType.Drive);

                case AppPath.Workspace:
                    return GetItemIconUri(JumpPointItemType.Workspace);

                case AppPath.Cloud:
                    return GetCloudStorageIconUri(CloudStorageProvider.Unknown);

                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.CloudDrives:
                case AppPath.Dashboard:
                case AppPath.Settings:
                case AppPath.Properties:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                    return new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png");
                
                case AppPath.Unknown:
                default:
                    return null;
            }
        }

        public static Uri GetPathTypeIconUri(AppPath pathType, object tag)
        {
            switch (pathType)
            {
                case AppPath.Drive:
                    return tag is DriveTemplate driveTemplate ? GetDriveIconUri(driveTemplate) : GetDriveIconUri(DriveTemplate.HDD);

                case AppPath.Folder:
                    return tag is FolderTemplate folderTemplate ? GetFolderIconUri(folderTemplate) : GetFolderIconUri(FolderTemplate.General);

                case AppPath.Workspace:
                    return tag is WorkspaceTemplate workspaceTemplate ? GetWorkspaceIconUri(workspaceTemplate) : GetWorkspaceIconUri(WorkspaceTemplate.Briefcase);

                case AppPath.Cloud:
                    return tag is CloudStorageProvider cloudStorageProvider ? GetCloudStorageIconUri(cloudStorageProvider) : GetCloudStorageIconUri(CloudStorageProvider.Unknown);

                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.CloudDrives:
                case AppPath.Dashboard:
                case AppPath.Settings:
                case AppPath.Properties:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                    return new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png");
            
                case AppPath.Unknown:
                default:
                    return null;
            }
        }

        public static IconElement GetPathTypeIconElement(AppPath pathType, object tag)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    return new BitmapIcon()
                    {
                        UriSource = tag is FolderTemplate folderTemplate ? GetFolderIconUri(folderTemplate) : GetFolderIconUri(FolderTemplate.General),
                        ShowAsMonochrome = false
                    };
                case AppPath.Drive:
                    return new BitmapIcon()
                    {
                        UriSource = tag is DriveTemplate driveTemplate ? GetDriveIconUri(driveTemplate) : GetDriveIconUri(DriveTemplate.HDD),
                        ShowAsMonochrome = false
                    };
                case AppPath.Workspace:
                    return new FontIcon()
                    {
                        Glyph = tag is WorkspaceTemplate workspaceTemplate ? GetWorkspaceGlyph(workspaceTemplate) : string.Empty,
                        FontFamily = new FontFamily("Segoe UI Emoji")
                    };
                case AppPath.Cloud:
                    return new BitmapIcon()
                    {
                        UriSource = tag is CloudStorageProvider cloudStorageProvider ? GetCloudStorageIconUri(cloudStorageProvider) : GetCloudStorageIconUri(CloudStorageProvider.Unknown),
                        ShowAsMonochrome = false
                    };
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.CloudDrives:
                case AppPath.Dashboard:
                case AppPath.Favorites:
                case AppPath.Settings:
                case AppPath.Properties:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                    return new BitmapIcon()
                    {
                        UriSource = new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png"),
                        ShowAsMonochrome = false
                    };
                case AppPath.Unknown:
                default:
                    return null;
            }
        }

        public static IconSource GetPathTypeIconSource(object pathType, object tag)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                    return new BitmapIconSource()
                    {
                        UriSource = tag is FolderTemplate folderTemplate ? GetFolderIconUri(folderTemplate) : GetFolderIconUri(FolderTemplate.General),
                        ShowAsMonochrome = false
                    };
                case AppPath.Drive:
                    return new BitmapIconSource()
                    {
                        UriSource = tag is DriveTemplate driveTemplate ? GetDriveIconUri(driveTemplate) : GetDriveIconUri(DriveTemplate.HDD),
                        ShowAsMonochrome = false
                    };
                case AppPath.Workspace:
                    return new FontIconSource()
                    {
                        Glyph = tag is WorkspaceTemplate workspaceTemplate ? GetWorkspaceGlyph(workspaceTemplate) : string.Empty,
                        FontFamily = new FontFamily("Segoe UI Emoji")
                    };
                case AppPath.Cloud:
                    return new BitmapIconSource()
                    {
                        UriSource = tag is CloudStorageProvider cloudStorageProvider ? GetCloudStorageIconUri(cloudStorageProvider) : GetCloudStorageIconUri(CloudStorageProvider.Unknown),
                        ShowAsMonochrome = false
                    };
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.CloudDrives:
                case AppPath.Dashboard:
                case AppPath.Favorites:
                case AppPath.Settings:
                case AppPath.Properties:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                    return new BitmapIconSource()
                    {
                        UriSource = new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png"),
                        ShowAsMonochrome = false
                    };
                case AppPath.Unknown:
                case null:
                default:
                    return new FontIconSource()
                    {
                        Glyph = string.Empty
                    };
            }
        }

        public static IconElement GetShellItemIcon(string key, object tag)
        {
            if (key == ViewModelKeys.Drive)
            {
                return GetPathTypeIconElement(AppPath.Drive, tag);
            }
            else if (key == ViewModelKeys.Folder)
            {
                return GetPathTypeIconElement(AppPath.Folder, tag);
            }
            else if (key == ViewModelKeys.Workspace)
            {
                return GetPathTypeIconElement(AppPath.Workspace, tag);
            }
            else if (key == ViewModelKeys.Cloud)
            {
                return GetPathTypeIconElement(AppPath.Cloud, tag);
            }
            else if (key == ViewModelKeys.Dashboard ||
                key == ViewModelKeys.Favorites ||
                key == ViewModelKeys.Settings ||
                key == ViewModelKeys.Properties ||
                key == ViewModelKeys.Drives ||
                key == ViewModelKeys.Workspaces ||
                key == ViewModelKeys.AppLinks ||
                key == ViewModelKeys.CloudDrives ||
                key == ViewModelKeys.Chatbot ||
                key == ViewModelKeys.ClipboardManager)
            {
                return tag is AppPath pathType ?
                    GetPathTypeIconElement(pathType, null) :
                    null;
            }
            return null;
        }

        public static ImageSource GetImageSourceFromStream(Stream stream, int height, int width)
        {
            var bitmap = new BitmapImage();
            bitmap.DecodePixelType = DecodePixelType.Logical;
            if (height != 0)
            {
                bitmap.DecodePixelHeight = height;
            }
            if (width != 0)
            {
                bitmap.DecodePixelWidth = width;
            }

            if (stream != null)
            {
                stream.Position = 0;
                bitmap.SetSource(stream.AsRandomAccessStream());
            }

            return bitmap;
        }

        public static ImageSource GetImageSource(Uri uri, Stream stream, byte[] bytes, int height, int width)
        {
            var bitmap = new BitmapImage();
            bitmap.DecodePixelType = DecodePixelType.Logical;
            if (height != 0)
            {
                bitmap.DecodePixelHeight = height;
            }
            if (width != 0)
            {
                bitmap.DecodePixelWidth = width;
            }

            if (uri != null)
            {
                bitmap.UriSource = uri;
                return bitmap;
            }
            else if (stream != null)
            {
                stream.Position = 0;
                bitmap.SetSource(stream.AsRandomAccessStream());
                return bitmap;
            }
            else if (bytes != null)
            {
                var ms = bytes.ToMemoryStream();
                ms.Position = 0;
                bitmap.SetSource(ms.AsRandomAccessStream());
                return bitmap;
            }
            else
            {
                return null;
            }
        }

        public static ImageSource GetImageSource(JumpPointItem item)
        {
            switch (item.Type)
            {
                case JumpPointItemType.Drive when item is DriveBase drive:
                    return GetImageSource(GetDriveIconUri(drive.DriveTemplate), null, null, 36, 36);

                case JumpPointItemType.Folder when item is FolderBase folder:
                    return GetImageSource(GetFolderIconUri(folder.FolderTemplate), null, null, 36, 36);

                case JumpPointItemType.File when item is FileBase file:
                    return FileThumbnailConverter.Convert(file.FileType, file.Thumbnail, 36, 36);

                case JumpPointItemType.Workspace when item is Workspace workspace:
                    return GetImageSource(GetWorkspaceIconUri(workspace.Template), null, null, 36, 36);

                case JumpPointItemType.AppLink when item is AppLink appLink:
                    return GetImageSource(null, appLink.Logo, null, 36, 36);

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    return null;
            }
        }

    }

}
