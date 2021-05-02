using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum ExposureProgram : uint
    {
        Unknown = 0,
        Manual = 1,
        Normal = 2,
        [Description("Aperture Priority")]
        Aperture = 3,
        [Description("Shutter Priority")]
        Shutter = 4,
        [Description("Creative Program (biased toward depth of field)")]
        Creative = 5,
        [Description("Action Program (biased toward shutter speed)")]
        Action = 6,
        [Description("Portrait Mode")]
        Portrait = 7,
        [Description("Landscape Mode")]
        Landscape = 8
    }
}
