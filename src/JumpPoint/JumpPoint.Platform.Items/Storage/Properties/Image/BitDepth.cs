using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Image
{
    public enum BitDepth : uint
    {
        Default = 0,
        [Description("1-Bit")]
        Bit1 = 1,

        [Description("2-Bits")]
        Bits2 = 2,

        [Description("4-Bits")]
        Bits4 = 4,

        [Description("8-Bits")]
        Bits8 = 8,

        [Description("16-Bits")]
        Bits16 = 16,

        [Description("24-Bits")]
        Bits24 = 24,

        [Description("32-Bits")]
        Bits32 = 32,

        [Description("48-Bits")]
        Bits48 = 48,

        [Description("64-Bits")]
        Bits64 = 64,

        [Description("96-Bits")]
        Bits96 = 96,

        [Description("128-Bits")]
        Bits128 = 128
    }
}
