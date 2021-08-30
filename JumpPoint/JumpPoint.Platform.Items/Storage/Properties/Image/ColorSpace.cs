using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Image
{
    public enum ColorSpace : ushort
    {
        [Description("sRGB")]
        SRGB = 1,
        Uncalibrated = 0xFFFF
    }
}
