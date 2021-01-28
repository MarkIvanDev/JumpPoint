using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Interop.Enums
{
    public enum FILE_NOTIFY_FILTER : uint
    {
        CHANGE_FILE_NAME = 1,
        CHANGE_DIR_NAME = 2,
        CHANGE_ATTRIBUTES = 4,
        CHANGE_SIZE = 8,
        CHANGE_LAST_WRITE = 16,
        CHANGE_LAST_ACCESS = 32,
        CHANGE_CREATION = 64,
        CHANGE_SECURITY = 256
    }
}
