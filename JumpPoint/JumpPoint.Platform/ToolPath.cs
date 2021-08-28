using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform
{
    public enum ToolPath
    {
        Unknown = 0,
        Hash = 1,
    }

    public enum HashFunction
    {
        Unknown = 0,

        [Description("MD5")]
        MD5 = 1,

        [Description("SHA-1")]
        SHA1 = 2,

        [Description("SHA-256")]
        SHA256 = 3,

        [Description("SHA-384")]
        SHA384 = 4,

        [Description("SHA-512")]
        SHA512 = 5,
    }
}
