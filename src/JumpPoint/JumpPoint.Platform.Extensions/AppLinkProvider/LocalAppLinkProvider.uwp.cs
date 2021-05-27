using Humanizer;
using JumpPoint.Platform.Items;
using NittyGritty.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace JumpPoint.Platform.Extensions
{
    public static class LocalAppLinkProvider
    {
        public static async Task<IList<AppLinkPayload>> GetPayloads()
        {
            var appName = Package.Current.DisplayName;
            var appId = Package.Current.Id.FamilyName;

            return new List<AppLinkPayload>()
            {
                await GetPayload(AppPath.Dashboard),
                await GetPayload(AppPath.Settings),
                await GetPayload(AppPath.Favorites),
                await GetPayload(AppPath.Drives),
                await GetPayload(AppPath.CloudDrives),
                await GetPayload(AppPath.Workspaces),
                await GetPayload(AppPath.AppLinks)
            };

            async Task<AppLinkPayload> GetPayload(AppPath pathType)
            {
                return new AppLinkPayload
                {
                    Link = $@"{Prefix.MAIN_SCHEME}://{pathType.ToString().ToLower()}",
                    Name = $"{pathType.Humanize()} - {appName}",
                    Description = $"{appName} {pathType.Humanize()}",
                    AppName = appName,
                    AppId = appId,
                    LaunchTypes = (int)AppLinkLaunchTypes.Uri,
                    Logo = await GetLogo(pathType)
                };
            }

            async Task<byte[]> GetLogo(AppPath pathType)
            {
                var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png"));
                var logoStream = (await logoFile.OpenReadAsync()).AsStream();
                return logoStream.ToByteArray();
            }
        }
    }
}
