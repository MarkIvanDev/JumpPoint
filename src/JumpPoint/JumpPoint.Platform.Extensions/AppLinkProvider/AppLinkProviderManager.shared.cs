using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;

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

    }

    public static class AppLinkPayloadExtensions
    {
        public static AppLinkInfo ToAppLinkInfo(this AppLinkPayload payload)
        {
            return string.IsNullOrWhiteSpace(payload.Link) ? null :
                new AppLinkInfo
                {
                    Link = payload.Link,
                    Name = payload.Name,
                    Description = payload.Description,
                    AppName = payload.AppName,
                    AppId = payload.AppId,
                    Logo = payload.Logo,
                    Background = payload.Background,
                    QueryKeys = payload.QueryKeys,
                    InputKeys = payload.InputKeys.ToInputKeys(),
                    LaunchTypes = (AppLinkLaunchTypes)payload.LaunchTypes,
                };
        }
    }
}
