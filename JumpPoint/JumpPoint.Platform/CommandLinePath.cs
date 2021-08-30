using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum CommandLinePath
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
    }

    public static class CommandLinePathExtensions
    {
        public static AppPath ToAppPath(this CommandLinePath path)
        {
            switch (path)
            {
                case CommandLinePath.Dashboard:
                    return AppPath.Dashboard;

                case CommandLinePath.Settings:
                    return AppPath.Settings;

                case CommandLinePath.Favorites:
                    return AppPath.Favorites;

                case CommandLinePath.Drives:
                    return AppPath.Drives;

                case CommandLinePath.CloudDrives:
                    return AppPath.CloudDrives;

                case CommandLinePath.Workspaces:
                    return AppPath.Workspaces;

                case CommandLinePath.AppLinks:
                    return AppPath.AppLinks;

                case CommandLinePath.Drive:
                    return AppPath.Drive;

                case CommandLinePath.Folder:
                    return AppPath.Folder;

                case CommandLinePath.Workspace:
                    return AppPath.Workspace;

                case CommandLinePath.Cloud:
                    return AppPath.Cloud;

                case CommandLinePath.Properties:
                    return AppPath.Properties;

                case CommandLinePath.Open:
                case CommandLinePath.Unknown:
                default:
                    return AppPath.Unknown;
            }
        }
    }
}
