using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using NittyGritty.Platform.Launcher;
using NittyGritty.Platform.Payloads;

namespace JumpPoint.Platform.Services
{
    public static partial class AppLinkService
    {

        static Task<AppLink> PlatformGetAppLink(IShareTargetPayload payload)
            => throw new NotImplementedException();

        static Task<AppLinkLaunchTypes> PlatformGetLaunchTypes(Uri uri, string identifier)
            => throw new NotImplementedException();

        static Task PlatformOpenUri(AppLink appLink)
            => throw new NotImplementedException();

        static Task<Collection<ValueInfo>> PlatformOpenUriForResults(AppLink appLink)
            => throw new NotImplementedException();

        static Task<IList<IAppInfo>> PlatformFindAppHandlers(string link)
            => throw new NotImplementedException();

    }
}
