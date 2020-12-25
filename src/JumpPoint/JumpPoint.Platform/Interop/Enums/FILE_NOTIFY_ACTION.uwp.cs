using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Interop.Enums
{
    public enum FILE_NOTIFY_ACTION : uint
    {
        FILE_ACTION_ADDED = 0x00000001,
        FILE_ACTION_REMOVED = 0x00000002,
        FILE_ACTION_MODIFIED = 0x00000003,
        FILE_ACTION_RENAMED_OLD_NAME = 0x00000004,
        FILE_ACTION_RENAMED_NEW_NAME = 0x00000005
    }
}
