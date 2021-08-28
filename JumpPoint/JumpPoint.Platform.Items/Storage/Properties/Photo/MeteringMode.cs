using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum MeteringMode : ushort
    {
        Unknown = 0,
        Average = 1,
        Center = 2,
        Spot = 3,
        MultiSpot = 4,
        Pattern = 5,
        Partial = 6
    }
}
