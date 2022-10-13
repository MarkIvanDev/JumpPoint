using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public static class Prefix
    {
        public const string MAIN_SCHEME =
#if JPBETA
            @"jumppoint-beta";
#else
            @"jumppoint";
#endif

        public const string PICKER_SCHEME =
#if JPBETA
            @"jumppoint-beta-picker";
#else
            @"jumppoint-picker";
#endif

        public const string TOOL_SCHEME =
#if JPBETA
            @"jumppoint-beta-tool";
#else
            @"jumppoint-tool";
#endif

        public const string NEWITEM_SCHEME =
#if JPBETA
            @"jumppoint-beta-newitem";
#else
            @"jumppoint-newitem";
#endif

        public const string UNMOUNTED = @"\\?\";
        public const string WSL = @"\\wsl.localhost\";
        public const string NETWORK = @"\\";
        public const string CLOUD = @"cloud:\";
        public const string WORKSPACE = @"workspace:\";
        public const string APPLINK = @"applink:\";
    }
}
