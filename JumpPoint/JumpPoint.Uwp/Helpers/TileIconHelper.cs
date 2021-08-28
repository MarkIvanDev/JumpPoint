using JumpPoint.Platform;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Uwp.Converters;
using JumpPoint.ViewModels.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Uwp.Helpers
{
    public class TileIconHelper : ITileIconHelper
    {
        public Uri GetIconUri(AppPath appPath, TileSize tileSize = TileSize.Medium)
        {
            return new Uri($@"ms-appx:///Assets/Tiles/Path/{appPath}-{tileSize}.png");
        }

        public Uri GetIconUri(FolderTemplate folderTemplate, TileSize tileSize = TileSize.Medium)
        {
            return new Uri($@"ms-appx:///Assets/Tiles/Folders/{folderTemplate}-{tileSize}.png");
        }

        public Uri GetIconUri(DriveTemplate driveTemplate, TileSize tileSize = TileSize.Medium)
        {
            switch (driveTemplate)
            {
                case DriveTemplate.Local:
                    return new Uri($@"ms-appx:///Assets/Tiles/Drives/HDD-{tileSize}.png");

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
                    return new Uri($@"ms-appx:///Assets/Tiles/Drives/{driveTemplate}-{tileSize}.png");

                case DriveTemplate.CD:
                case DriveTemplate.CDR:
                case DriveTemplate.CDRW:
                    return new Uri($@"ms-appx:///Assets/Tiles/Drives/CD-{tileSize}.png");

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
                    return new Uri($@"ms-appx:///Assets/Tiles/Drives/DVD-{tileSize}.png");

                case DriveTemplate.BD:
                case DriveTemplate.BDR:
                case DriveTemplate.BDRE:
                    return new Uri($@"ms-appx:///Assets/Tiles/Drives/BD-{tileSize}.png");

                case DriveTemplate.Cloud:
                    return GetIconUri(CloudStorageProvider.Unknown, tileSize);
                case DriveTemplate.OneDrive:
                    return GetIconUri(CloudStorageProvider.OneDrive, tileSize);

                case DriveTemplate.Unknown:
                default:
                    return null;
            }
        }

        public Uri GetIconUri(WorkspaceTemplate workspaceTemplate, TileSize tileSize = TileSize.Medium)
        {
            return new Uri($@"ms-appx:///Assets/Tiles/Workspaces/{workspaceTemplate}-{tileSize}.png");
        }

        public Uri GetIconUri(CloudStorageProvider cloudStorageProvider, TileSize tileSize = TileSize.Medium)
        {
            return new Uri($@"ms-appx:///Assets/Tiles/Cloud/{cloudStorageProvider}-{tileSize}.png");
        }

        public Uri GetIconUri(AppPath pathType, object tag, TileSize tileSize = TileSize.Medium)
        {
            switch (pathType)
            {
                case AppPath.Drive:
                    return tag is DriveTemplate driveTemplate ? GetIconUri(driveTemplate, tileSize) : GetIconUri(DriveTemplate.HDD, tileSize);

                case AppPath.Folder:
                    return tag is FolderTemplate folderTemplate ? GetIconUri(folderTemplate, tileSize) : GetIconUri(FolderTemplate.General, tileSize);

                case AppPath.Workspace:
                    return tag is WorkspaceTemplate workspaceTemplate ? GetIconUri(workspaceTemplate, tileSize) : GetIconUri(WorkspaceTemplate.Briefcase, tileSize);

                case AppPath.Cloud:
                    return tag is CloudStorageProvider cloudStorageProvider ? GetIconUri(cloudStorageProvider, tileSize) : GetIconUri(CloudStorageProvider.Unknown, tileSize);

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
                    return GetIconUri(pathType, tileSize);

                case AppPath.Unknown:
                default:
                    return null;
            }
        }
    }
}
