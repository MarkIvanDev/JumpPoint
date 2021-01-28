using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage.Properties.Audio
{
    public enum Bitrate : uint
    {
        [Description("Voice and AM Broadcast (0 - 32 Kbps)")]
        AMBroadcast = 0,

        [Description("FM Broadcast (32 - 64 Kbps)")]
        FMBroadcast = 32769,

        [Description("High Quality (64 - 128 Kbps)")]
        HighQuality = 65537,

        [Description("Near CD Quality (over 128 Kbps)")]
        NearCDQuality = 131073
    }
}
