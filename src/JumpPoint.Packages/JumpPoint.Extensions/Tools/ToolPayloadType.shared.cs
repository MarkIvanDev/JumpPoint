using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Extensions.Tools
{
    public enum ToolPayloadType
    {
        Unknown = 0,
        Drive = 1,
        Folder = 2,
        File = 3,
        Workspace = 4,
        AppLink = 6,
        Library = 7,
    }
}
