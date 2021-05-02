using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Models;

namespace JumpPoint.Platform.Extensions
{
    public static partial class AppLinkProviderManager
    {

        static void PlatformStart()
            => throw new NotImplementedException();

        static void PlatformStop()
            => throw new NotImplementedException();

        static Task<IList<AppLinkProvider>> PlatformGetProviders()
            => throw new NotImplementedException();

        static Task<AppLinkInfo> PlatformPick(AppLinkProvider picker)
            => throw new NotImplementedException();

        static Task<IList<AppLinkPayload>> PlatformGetLocalAppLinks()
            => throw new NotImplementedException();

    }
}
