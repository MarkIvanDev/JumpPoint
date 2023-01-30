using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using NittyGritty.Models;

namespace JumpPoint.ViewModels.Commands
{
    public static class Sidebar
    {
        public static bool IsOpenInNewTabEnabled(object content)
        {
            if (content is null) return false;
            if (content is JumpPointItem jpItem)
            {
                switch (jpItem.Type)
                {
                    case JumpPointItemType.Drive:
                    case JumpPointItemType.Folder:
                    case JumpPointItemType.Workspace:
                        return true;

                    case JumpPointItemType.File:
                    case JumpPointItemType.AppLink:
                    case JumpPointItemType.Library:
                    case JumpPointItemType.Unknown:
                    default:
                        return false;
                }
            }
            else if (content is AppPath appPath)
            {
                switch (appPath)
                {
                    case AppPath.Dashboard:
                    case AppPath.Settings:
                    case AppPath.Favorites:
                    case AppPath.Drives:
                    case AppPath.CloudDrives:
                    case AppPath.Workspaces:
                    case AppPath.AppLinks:
                    case AppPath.WSL:
                    case AppPath.Drive:
                    case AppPath.Folder:
                    case AppPath.Workspace:
                    case AppPath.Cloud:
                        return true;

                    case AppPath.Properties:
                    case AppPath.Chat:
                    case AppPath.ClipboardManager:
                    case AppPath.Unknown:
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsOpenInNewWindowEnabled(object content)
        {
            if (content is null) return false;
            if (content is JumpPointItem jpItem)
            {
                switch (jpItem.Type)
                {
                    case JumpPointItemType.Drive:
                    case JumpPointItemType.Folder:
                    case JumpPointItemType.Workspace:
                        return true;

                    case JumpPointItemType.File:
                    case JumpPointItemType.AppLink:
                    case JumpPointItemType.Library:
                    case JumpPointItemType.Unknown:
                    default:
                        return false;
                }
            }
            else if (content is AppPath appPath)
            {
                switch (appPath)
                {
                    case AppPath.Dashboard:
                    case AppPath.Settings:
                    case AppPath.Favorites:
                    case AppPath.Drives:
                    case AppPath.CloudDrives:
                    case AppPath.Workspaces:
                    case AppPath.AppLinks:
                    case AppPath.WSL:
                    case AppPath.Drive:
                    case AppPath.Folder:
                    case AppPath.Workspace:
                    case AppPath.Cloud:
                        return true;

                    case AppPath.Properties:
                    case AppPath.Chat:
                    case AppPath.ClipboardManager:
                    case AppPath.Unknown:
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
