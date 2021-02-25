using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum PathKind
    {
        Unknown = 0,
        Local = 1,
        Mounted = 2,
        Unmounted = 3,
        Network = 4,
        Cloud = 5,
        Workspace = 6,
    }
}
