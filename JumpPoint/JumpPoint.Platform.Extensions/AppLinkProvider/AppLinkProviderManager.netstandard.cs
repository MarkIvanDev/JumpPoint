﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Models;

namespace JumpPoint.Platform.Extensions
{
    public static partial class AppLinkProviderManager
    {

        static Task<IList<AppLinkProvider>> PlatformGetProviders()
            => throw new NotImplementedException();

        static Task<AppLinkInfo> PlatformPick(AppLinkProvider picker)
            => throw new NotImplementedException();

        static Task<IList<AppLinkPayload>> PlatformGetPayloads(string service, string packageId)
            => throw new NotImplementedException();

    }
}