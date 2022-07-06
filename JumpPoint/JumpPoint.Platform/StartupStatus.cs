using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum StartupStatus
    {
        Disabled = 0,
        DisabledByUser = 1,
        Enabled = 2,
        DisabledByPolicy = 3,
        EnabledByPolicy = 4
    }
}
