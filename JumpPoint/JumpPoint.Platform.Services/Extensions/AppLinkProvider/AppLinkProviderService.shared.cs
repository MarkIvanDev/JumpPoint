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
        public static event EventHandler ExtensionCollectionChanged;

        public static async Task<IList<AppLinkProvider>> GetProviders()
            => await PlatformGetProviders();

        public static async Task<AppLinkInfo> Pick(AppLinkProvider provider)
            => await PlatformPick(provider);

        public static async Task<IList<AppLinkPayload>> GetPayloads(string service, string packageId)
            => await PlatformGetPayloads(service, packageId);
    }
}
