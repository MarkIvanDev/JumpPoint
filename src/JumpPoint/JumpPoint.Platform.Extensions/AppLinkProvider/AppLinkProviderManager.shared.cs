using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Models;
using NittyGritty.Models;

namespace JumpPoint.Platform.Extensions
{
    public static partial class AppLinkProviderManager
    {
        public static event EventHandler ExtensionCollectionChanged;

        public static async Task<IList<AppLinkProvider>> GetProviders()
            => await PlatformGetProviders();

        public static async Task<AppLinkInfo> Pick(AppLinkProvider provider)
            => await PlatformPick(provider);

        public static async Task<IList<AppLinkPayload>> GetPayloads(string service, string packageId)
            => await PlatformGetPayloads(service, packageId);

        public static async Task<string> GetPayloadsToken(IList<AppLinkPayload> payloads)
            => await PlatformGetPayloadsToken(payloads);

        public static async Task<byte[]> GetLogo(Uri logoUri)
            => await PlatformGetLogo(logoUri);

    }

}
