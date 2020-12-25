using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum Flash : byte
    {
        NoFlash = 0,
        Flash = 1,

        [Description("Flash, no strobe return")]
        FlashNoReturnLight = 5,

        [Description("Flash, strobe return")]
        FlashReturnLight = 7,

        [Description("Flash, compulsory")]
        FlashCompulsory = 9,

        [Description("Flash, compulsory, no strobe return")]
        FlashCompulsoryNoReturnLight = 13,

        [Description("Flash, compulsory, strobe return")]
        FlashCompulsoryReturnLight = 15,

        [Description("No flash, compulsory")]
        NoFlashCompulsory = 16,

        [Description("No flash, auto")]
        NoFlashAuto = 24,

        [Description("Flash, auto")]
        FlashAuto = 25,

        [Description("Flash, auto, no strobe return")]
        FlashAutoNoReturnLight = 29,

        [Description("Flash, auto, strobe return")]
        FlashAutoReturnLight = 31,

        NoFlashFunction = 32,

        [Description("Flash, red-eye")]
        FlashRedEye = 65,

        [Description("Flash, red-eye, no strobe return")]
        FlashRedEyeNoReturnLight = 69,

        [Description("Flash, red-eye, strobe return")]
        FlashRedEyeReturnLight = 71,

        [Description("Flash, compulsory, red-eye")]
        FlashCompulsoryRedEye = 73,

        [Description("Flash, compulsory, red-eye, no strobe return")]
        FlashCompulsoryRedEyeNoReturnLight = 77,

        [Description("Flash, compulsory, red-eye, strobe return")]
        FlashCompulsoryRedEyeReturnLight = 79,

        [Description("Flash, auto, red-eye")]
        FlashAutoRedEye = 89,

        [Description("Flash, auto, no strobe return, red-eye")]
        FlashAutoRedEyeNoReturnLight = 93,

        [Description("Flash, auto, strobe return, red-eye")]
        FlashAutoRedEyeReturnLight = 95
    }
}
