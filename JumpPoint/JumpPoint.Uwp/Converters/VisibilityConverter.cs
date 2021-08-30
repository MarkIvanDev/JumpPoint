using System;
using System.Text;
using JumpPoint.Platform;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Converters
{
    public static class VisibilityConverter
    {
        public static Visibility IsToolbarVisible(AppPath pathType)
        {
            switch (pathType)
            {
                case AppPath.Folder:
                case AppPath.Drive:
                case AppPath.Workspace:
                case AppPath.Favorites:
                case AppPath.Drives:
                case AppPath.Workspaces:
                case AppPath.AppLinks:
                case AppPath.Dashboard:
                case AppPath.Cloud:
                case AppPath.CloudDrives:
                    return Visibility.Visible;

                case AppPath.Unknown:
                case AppPath.Settings:
                case AppPath.Properties:
                case AppPath.Chat:
                case AppPath.ClipboardManager:
                default:
                    return Visibility.Collapsed;
            }
        }
    }
}
