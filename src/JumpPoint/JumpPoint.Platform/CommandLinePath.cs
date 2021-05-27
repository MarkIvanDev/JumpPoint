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

        Properties = 41
    }
}
