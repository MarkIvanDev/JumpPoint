using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum ProtocolPath
    {
        Unknown = 0,

        Dashboard = 1,
        Settings = 2,
        Favorites = 3,
        Drives = 4,
        CloudDrives = 5,
        Workspaces = 6,
        AppLinks = 7,

        Open = 21,
        Drive = 22,
        Folder = 23,
        Workspace = 24,
        Cloud = 25,

        Properties = 41,
        Chat = 42,
    }

    public static class ProtocolPathExtensions
    {
        public static AppPath ToAppPath(this ProtocolPath path)
        {
            switch (path)
            {
                case ProtocolPath.Dashboard:
                    return AppPath.Dashboard;

                case ProtocolPath.Settings:
                    return AppPath.Settings;

                case ProtocolPath.Favorites:
                    return AppPath.Favorites;

                case ProtocolPath.Drives:
                    return AppPath.Drives;

                case ProtocolPath.CloudDrives:
                    return AppPath.CloudDrives;

                case ProtocolPath.Workspaces:
                    return AppPath.Workspaces;

                case ProtocolPath.AppLinks:
                    return AppPath.AppLinks;

                case ProtocolPath.Drive:
                    return AppPath.Drive;

                case ProtocolPath.Folder:
                    return AppPath.Folder;

                case ProtocolPath.Workspace:
                    return AppPath.Workspace;

                case ProtocolPath.Cloud:
                    return AppPath.Cloud;

                case ProtocolPath.Properties:
                    return AppPath.Properties;

                case ProtocolPath.Chat:
                    return AppPath.Chat;

                case ProtocolPath.Open:
                case ProtocolPath.Unknown:
                default:
                    return AppPath.Unknown;
            }
        }
    }
}
