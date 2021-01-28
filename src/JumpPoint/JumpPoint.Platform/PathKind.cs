using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum PathKind
    {
        Unknown = 0,
        Mounted = 1,
        Unmounted = 2,
        Network = 3,
        Cloud = 4
    }
}
