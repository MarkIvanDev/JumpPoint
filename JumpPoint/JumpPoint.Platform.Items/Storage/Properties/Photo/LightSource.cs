using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum LightSource : uint
    {
        Unknown = 0,
        Daylight = 1,
        Fluorescent = 2,
        Tungsten = 3,
        [Description("Standard Illuminant A")]
        StandardA = 17,
        [Description("Standard Illuminant B")]
        StandardB = 18,
        [Description("Standard Illuminant C")]
        StandardC = 19,
        D55 = 20,
        D65 = 21,
        D75 = 22
    }
}
