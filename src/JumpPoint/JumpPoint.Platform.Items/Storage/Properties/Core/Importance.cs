using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Core
{
    public enum Importance : int
    {
        [Description("Low")]
        Low0 = 0,
        [Description("Low")]
        Low1 = 1,
        [Description("Normal")]
        Normal2 = 2,
        [Description("Normal")]
        Normal3 = 3,
        [Description("Normal")]
        Normal4 = 4,
        [Description("High")]
        High5 = 5
    }
}
