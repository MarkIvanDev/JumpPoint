using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Interop.Enums
{
    public enum FILE_SHARE_MODE : uint
    {
        NONE = 0,
        FILE_SHARE_READ = 1,
        FILE_SHARE_WRITE = 2,
        FILE_SHARE_DELETE = 4
    }
}
