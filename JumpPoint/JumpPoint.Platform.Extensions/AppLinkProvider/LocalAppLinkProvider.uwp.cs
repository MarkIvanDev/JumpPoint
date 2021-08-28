using Humanizer;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Items;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;

namespace JumpPoint.Platform.Extensions
{
    public static class LocalAppLinkProvider
    {
        public static IList<AppLinkPayload> GetPayloads()
        {
            var appName = Package.Current.DisplayName;
            var appId = Package.Current.Id.FamilyName;

            return new List<AppLinkPayload>()
            {
                GetPayload(AppPath.Dashboard),
                GetPayload(AppPath.Settings),
                GetPayload(AppPath.Favorites),
                GetPayload(AppPath.Drives),
                GetPayload(AppPath.CloudDrives),
                GetPayload(AppPath.Workspaces),
                GetPayload(AppPath.AppLinks)
            };

            AppLinkPayload GetPayload(AppPath pathType)
            {
                return new AppLinkPayload
                {
                    Link = $@"{Prefix.MAIN_SCHEME}://{pathType.ToString().ToLower()}",
                    Name = $"{pathType.Humanize()} - {appName}",
                    Description = $"{appName} {pathType.Humanize()}",
                    AppName = appName,
                    AppId = appId,
                    LaunchTypes = (int)AppLinkLaunchTypes.Uri,
                    LogoUri = new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png")
                };
            }
        }
    }
}
