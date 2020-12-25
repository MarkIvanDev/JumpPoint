using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Interop.Enums
{
    [Flags]
    public enum FIND_FIRST_EX_ADDITIONAL_FLAGS : uint
    {
        FIND_FIRST_EX_CASE_SENSITIVE = 1,
        FIND_FIRST_EX_LARGE_FETCH = 2,
        FIND_FIRST_EX_ON_DISK_ENTRIES_ONLY = 4
    }
}
