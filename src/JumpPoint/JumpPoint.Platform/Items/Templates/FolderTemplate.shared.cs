using System;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace JumpPoint.Platform.Items.Templates
{
    [StoreAsText]
    public enum FolderTemplate
    {
        [Description("File")]
        General = 0,

        User = 1,
        [Description("3D Objects")]
        Objects3D = 2,
        CameraRoll = 3,
        Contacts = 4,
        Cookies = 5,
        Desktop = 6,
        Documents = 7,
        Downloads = 8,
        Favorites = 9,
        Links = 10,
        History = 11,
        InternetCache = 12,
        LocalAppData = 13,
        LocalAppDataLow = 14,
        Music = 15,
        Pictures = 16,
        Playlists = 17,
        Recent = 18,
        RoamingAppData = 19,
        SavedGames = 20,
        SavedPictures = 21,
        Screenshots = 22,
        Searches = 23,
        Templates = 24,
        Videos = 25,

        Users = 51,
        Burn = 52,
        Fonts = 53,
        ProgramData = 54,
        ProgramFiles = 55,
        [Description("Program Files x86")]
        ProgramFilesX86 = 56,
        Public = 57,
        PublicDesktop = 58,
        PublicDocuments = 59,
        PublicDownloads = 60,
        PublicMusic = 61,
        PublicPictures = 62,
        PublicVideos = 63,
        System = 64,
        Windows = 65,

        [Description("OneDrive")]
        OneDrive = 101
    }

}
