using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform
{
    public enum AppPath
    {
        Unknown = 0,

        Dashboard = 1,
        Settings = 2,
        Favorites = 3,
        Drives = 4,
        [Description("Cloud Drives")]
        CloudDrives = 5,
        Workspaces = 6,
        [Description("App Links")]
        AppLinks = 7,
        WSL = 8,

        Drive = 22,
        Folder = 23,
        Workspace = 24,
        Cloud = 25,

        Properties = 41,
        Chat = 42,
        [Description("Clipboard Manager")]
        ClipboardManager = 43,
    }

    public static class AppPathExtensions
    {
        public static ProtocolPath ToProtocolPath(this AppPath path)
        {
            switch (path)
            {
                case AppPath.Dashboard:
                    return ProtocolPath.Dashboard;

                case AppPath.Settings:
                    return ProtocolPath.Settings;

                case AppPath.Favorites:
                    return ProtocolPath.Favorites;

                case AppPath.Drives:
                    return ProtocolPath.Drives;

                case AppPath.CloudDrives:
                    return ProtocolPath.CloudDrives;

                case AppPath.Workspaces:
                    return ProtocolPath.Workspaces;

                case AppPath.AppLinks:
                    return ProtocolPath.AppLinks;

                case AppPath.WSL:
                    return ProtocolPath.WSL;

                case AppPath.Drive:
                    return ProtocolPath.Drive;

                case AppPath.Folder:
                    return ProtocolPath.Folder;

                case AppPath.Workspace:
                    return ProtocolPath.Workspace;

                case AppPath.Cloud:
                    return ProtocolPath.Cloud;

                case AppPath.Properties:
                    return ProtocolPath.Properties;

                case AppPath.Chat:
                    return ProtocolPath.Chat;

                case AppPath.ClipboardManager:
                    return ProtocolPath.Clipboard;
                
                case AppPath.Unknown:
                default:
                    return ProtocolPath.Unknown;
            }
        }
    }
}
