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
                await GetPayload(PathType.Dashboard),
                await GetPayload(PathType.Settings),
                await GetPayload(PathType.Favorites),
                await GetPayload(PathType.Drives),
                await GetPayload(PathType.CloudStorages),
                await GetPayload(PathType.Workspaces),
                await GetPayload(PathType.AppLinks)
            };

            async Task<AppLinkPayload> GetPayload(PathType pathType)
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

            async Task<byte[]> GetLogo(PathType pathType)
            {
                var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($@"ms-appx:///Assets/Icons/Path/{pathType}.png"));
                var logoStream = (await logoFile.OpenReadAsync()).AsStream();
                return logoStream.ToByteArray();
            }
        }
    }
}
