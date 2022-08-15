using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Models;

namespace JumpPoint.Platform.Services
{
    public static partial class AppLinkProviderService
    {
        static Task<IList<AppLinkProvider>> PlatformGetProviders()
            => throw new NotImplementedException();

        static Task<AppLinkInfo> PlatformPick(AppLinkProvider picker)
            => throw new NotImplementedException();

        static Task<IList<AppLinkPayload>> PlatformGetPayloads(string service, string packageId)
            => throw new NotImplementedException();
    }
}
