using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum PhotometricInterpretation : ushort
    {
        RGB = 2,
        [Description("YCbCr")]
        YCbCr = 6
    }
}
