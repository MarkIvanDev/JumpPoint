using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Video
{
    public enum Orientation : uint
    {
        //
        // Summary:
        //     No rotation needed. The video can be displayed using its current orientation.
        Normal = 0,
        //
        // Summary:
        //     Rotate the video 90 degrees.
        Rotate90 = 90,
        //
        // Summary:
        //     Rotate the video counter-clockwise 180 degrees.
        Rotate180 = 180,
        //
        // Summary:
        //     Rotate the video counter-clockwise 270 degrees.
        Rotate270 = 270
    }
}
