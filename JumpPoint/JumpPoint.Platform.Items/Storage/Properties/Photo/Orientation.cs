using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum Orientation : ushort
    {
        Normal = 1,
        FlipHorizontal = 2,
        Rotate180 = 3,
        FlipVertical = 4,
        Transpose = 5,
        Rotate270 = 6,
        Transverse = 7,
        Rotate90 = 8
    }
}
