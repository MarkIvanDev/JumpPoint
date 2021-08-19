using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public static class Prefix
    {
        public const string MAIN_SCHEME = @"jumppoint";
        //#if BETA
        //            @"jumppoint-beta";
        //#else
        //            @"jumppoint";
        //#endif

        public const string PICKER_SCHEME = @"jumppoint-picker";
        //#if BETA
        //            @"jumppoint-beta-picker";
        //#else
        //            @"jumppoint-picker";
        //#endif

        public const string TOOL_SCHEME = @"jumppoint-tool";
        //#if BETA
        //            @"jumppoint-beta-tool";
        //#else
        //            @"jumppoint-tool";
        //#endif

        public const string UNMOUNTED = @"\\?\";
        public const string NETWORK = @"\\";
        public const string CLOUD = @"cloud:\";
        public const string WORKSPACE = @"workspace:\";
        public const string APPLINK = @"applink:\";
    }
}
