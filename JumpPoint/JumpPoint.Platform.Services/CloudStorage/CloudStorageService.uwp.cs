using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JumpPoint.Platform.Services
{
    public static partial class CloudStorageService
    {
        static object PlatformGetWindowHandle()
        {
            dynamic corewin = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            var interop = (ICoreWindowInterop)corewin;
            return interop.WindowHandle;
        }
    }

    [ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICoreWindowInterop
    {
        IntPtr WindowHandle { get; }
        bool MessageHandled { set; }
    }
}
