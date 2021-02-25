using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Services
{
    public static partial class DesktopService
    {
        public static Task Open(IList<string> paths)
            => PlatformOpen(paths);

        public static Task Paste(string destination)
            => PlatformPaste(destination);

        public static Task Delete(IList<string> paths, bool deletePermanently)
            => PlatformDelete(paths, deletePermanently);

        public static Task OpenCleanManager(char? driveLetter = null)
            => PlatformOpenCleanManager(driveLetter);

        public static Task OpenInCommandPrompt(IList<string> paths)
            => PlatformOpenInCommandPrompt(paths);

        public static Task OpenInPowershell(IList<string> paths)
            => PlatformOpenInPowershell(paths);

        public static Task OpenSystemApp(string app, string arguments = null)
            => PlatformOpenSystemApp(app, arguments);

    }
}
