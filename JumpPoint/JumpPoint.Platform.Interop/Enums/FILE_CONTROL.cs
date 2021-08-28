using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Interop.Enums
{
    [Flags]
    public enum FILE_CONTROL : uint
    {
        OPEN_NO_RECALL =        0x00100000,
        OPEN_REPARSE_POINT =    0x00200000,
        SESSION_AWARE =         0x00800000,
        POSIX_SEMANTICS =       0x01000000,
        BACKUP_SEMANTICS =      0x02000000,
        DELETE_ON_CLOSE =       0x04000000,
        SEQUENTIAL_SCAN =       0x08000000,
        RANDOM_ACCESS =         0x10000000,
        NO_BUFFERING =          0x20000000,
        OVERLAPPED =            0x40000000,
        WRITE_THROUGH =         0x80000000
    }
}
