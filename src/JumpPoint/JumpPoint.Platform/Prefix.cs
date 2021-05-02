using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public static class Prefix
    {
        public const string MAIN_SCHEME =
#if BETA
            @"jumpppoint-beta";
#else
            @"jumppoint";
#endif

        public const string PICKER_SCHEME =
#if BETA
            @"jumpppoint-beta-picker";
#else
            @"jumppoint-picker";
#endif

        public const string UNMOUNTED = @"\\?\";
        public const string NETWORK = @"\\";
        public const string CLOUD = @"cloud:\";
        public const string WORKSPACE = @"workspace:\";
        public const string APPLINK = @"applink:\";
    }
}
