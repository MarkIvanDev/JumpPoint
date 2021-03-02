using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Photo
{
    public enum ProgramMode : uint
    {
        NotDefined = 0,
        Manual = 1,
        [Description("Normal program")]
        Normal = 2,
        [Description("Aperture priority")]
        Aperture = 3,
        [Description("Shutter priority")]
        Shutter = 4,
        [Description("Creative program")]
        Creative = 5,
        [Description("Action program")]
        Action = 6,
        Portrait = 7,
        Landscape = 8
    }
}
