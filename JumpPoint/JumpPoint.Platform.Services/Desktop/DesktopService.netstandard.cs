using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Services
{
    public static partial class DesktopService
    {
        static Task PlatformOpen(IList<string> paths)
            => throw new NotImplementedException();

        static Task PlatformPaste(string destination)
            => throw new NotImplementedException();

        static Task PlatformCopyTo(string destination, IList<string> paths)
            => throw new NotImplementedException();

        static Task PlatformMoveTo(string destination, IList<string> paths)
            => throw new NotImplementedException();

        static Task PlatformDelete(IList<string> paths, bool deletePermanently)
            => throw new NotImplementedException();

        static Task PlatformOpenCleanManager(char? driveLetter = null)
            => throw new NotImplementedException();

        static Task PlatformOpenInCommandPrompt(IList<string> paths)
            => throw new NotImplementedException();

        static Task PlatformOpenInPowershell(IList<string> paths)
            => throw new NotImplementedException();

        static Task PlatformOpenSystemApp(string app, string arguments = null)
            => throw new NotImplementedException();
    }
}
