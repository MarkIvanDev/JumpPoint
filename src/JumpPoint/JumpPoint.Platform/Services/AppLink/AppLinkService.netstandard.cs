using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
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

        static Task PlatformOpenUri(AppLink appLink, IList<ValueInfo> inputValues)
            => throw new NotImplementedException();

        static Task<Collection<ValueInfo>> PlatformOpenUriForResults(AppLink appLink, IList<ValueInfo> inputValues)
            => throw new NotImplementedException();

        static Task<ReadOnlyCollection<IAppInfo>> PlatformFindAppHandlers(string path)
            => throw new NotImplementedException();

    }
}
