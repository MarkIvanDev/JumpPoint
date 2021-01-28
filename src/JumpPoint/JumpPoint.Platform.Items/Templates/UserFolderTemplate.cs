using System;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Templates
{
    public enum UserFolderTemplate
    {
        None = 0,
        User = 1,
        [Description("3D Objects")]
        Objects3D = 2,
        CameraRoll = 3,
        Desktop = 6,
        Documents = 7,
        Downloads = 8,
        Favorites = 9,
        Music = 15,
        OneDrive = 101,
        Pictures = 16,
        Playlists = 17,
        SavedPictures = 21,
        Screenshots = 22,
        Videos = 25,

        LocalAppData = 13,
        LocalAppDataLow = 14,
        RoamingAppData = 19,

        Cookies = 5,
        History = 11,
        InternetCache = 12,
        Recent = 18,
        Templates = 24,

        Contacts = 4,
        Links = 10,
        SavedGames = 20,
        Searches = 23
    }
}
