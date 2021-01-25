using System;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Templates
{
    public enum SystemFolderTemplate
    {
        None = 0,
        Users = 51,
        Public = 57,
        PublicDesktop = 58,
        PublicDocuments = 59,
        PublicDownloads = 60,
        PublicMusic = 61,
        PublicPictures = 62,
        PublicVideos = 63,

        Fonts = 53,
        ProgramData = 54,
        ProgramFiles = 55,
        [Description("Program Files x86")]
        ProgramFilesX86 = 56,

        System = 64,
        Windows = 65,

        Burn = 52
    }
}
