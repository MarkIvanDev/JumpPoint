using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Image
{
    public enum Compression : ushort
    {
        Uncompressed = 1,

        [Description("CCITT T.3")]
        CCITTT3 = 2,

        [Description("CCITT T.4")]
        CCITTT4 = 3,

        [Description("CCITT T.6")]
        CCITTT6 = 4,
        
        LZW = 5,
        JPEG = 6,
        PackBits = 32773
    }
}
