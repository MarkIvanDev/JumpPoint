using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum PathType
    {
        Unknown = 0,
        Drive = 1,
        Folder = 2,
        File = 3,
        Workspace = 4,
        SettingLink = 5,
        AppLink = 6,
        Library = 7,
        CloudStorage = 8,

        Favorites = 101,
        Drives = 102,
        Workspaces = 103,
        Libraries = 104,
        AppLinks = 105,
        SettingLinks = 106,
        CloudStorages = 107,

        Dashboard = 201,
        Settings = 202,
        Properties = 203,
    }
}
