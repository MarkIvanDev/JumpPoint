using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public static class Prefix
    {
        public const string UNMOUNTED = @"\\?\";
        public const string NETWORK = @"\\";
        public const string CLOUD = @"cloud:\";
        public const string WORKSPACE = @"workspace:\";
    }
}
