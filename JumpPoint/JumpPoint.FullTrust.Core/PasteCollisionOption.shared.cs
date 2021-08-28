using System;
using System.ComponentModel;

namespace JumpPoint.FullTrust.Core
{
    public enum PasteCollisionOption
    {
        [Description("Generate a unique name")]
        GenerateUniqueName = 0,

        [Description("Replace existing")]
        ReplaceExisiting = 1,

        [Description("Do nothing (Skip)")]
        DoNothing = 3,

        [Description("Keep newer file")]
        KeepNewer = 4,

        [Description("Let me decide")]
        LetMeDecide = 9
    }
}
