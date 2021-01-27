using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Items;
using NittyGritty.Platform.Launcher;
using NittyGritty.Platform.Payloads;
using Windows.Foundation.Collections;
using Windows.System;

namespace JumpPoint.Platform.Services
{
    public static partial class AppLinkService
    {

        static async Task<AppLink> PlatformGetAppLink(IShareTargetPayload payload)
        {
            if (payload is ShareTargetPayload shareTargetPayload)
            {
                var appUri = await shareTargetPayload.Operation.Data.GetApplicationLinkAsync();
                var displayName = shareTargetPayload.Operation.Data.Properties.Title;
                var appName = shareTargetPayload.Operation.Data.Properties.ApplicationName;
                var identifier = shareTargetPayload.Operation.Data.Properties.PackageFamilyName;
                var logo = shareTargetPayload.Operation.Data.Properties.Square30x30Logo != null ?
                    (await shareTargetPayload.Operation.Data.Properties.Square30x30Logo.OpenReadAsync()).AsStream() : null;
                var background = Color.FromArgb(
                    shareTargetPayload.Operation.Data.Properties.LogoBackgroundColor.A,
                    shareTargetPayload.Operation.Data.Properties.LogoBackgroundColor.R,
                    shareTargetPayload.Operation.Data.Properties.LogoBackgroundColor.G,
                    shareTargetPayload.Operation.Data.Properties.LogoBackgroundColor.B);
                var launchTypes = await PlatformGetLaunchTypes(appUri, identifier);
                return new AppLink(displayName, appUri.AbsoluteUri, appName, identifier, logo, background, new Collection<ValueInfo>(), launchTypes);
            }
            return null;
        }

        static async Task<AppLinkLaunchTypes> PlatformGetLaunchTypes(Uri uri, string identifier)
        {
            var launchUriSupport = identifier != null ?
                    await Launcher.QueryUriSupportAsync(uri, LaunchQuerySupportType.Uri, identifier) :
                    await Launcher.QueryUriSupportAsync(uri, LaunchQuerySupportType.Uri);
            var launchUriForResultsSupport = identifier != null ?
                await Launcher.QueryUriSupportAsync(uri, LaunchQuerySupportType.UriForResults, identifier) :
                await Launcher.QueryUriSupportAsync(uri, LaunchQuerySupportType.UriForResults);
            var launchTypes = AppLinkLaunchTypes.None;
            launchTypes |= launchUriSupport == LaunchQuerySupportStatus.Available ?
                AppLinkLaunchTypes.Uri : AppLinkLaunchTypes.None;
            launchTypes |= launchUriForResultsSupport == LaunchQuerySupportStatus.Available ?
                AppLinkLaunchTypes.UriForResults : AppLinkLaunchTypes.None;
            return launchTypes;
        }

        static async Task PlatformOpenUri(AppLink appLink, IList<ValueInfo> inputValues)
        {
            var valueSet = new ValueSet();
            foreach (var input in inputValues ?? Enumerable.Empty<ValueInfo>())
            {
                valueSet.Add(input.Key, input.Value);
            }
            _ = valueSet.Count > 0 ?
                await Launcher.LaunchUriAsync(
                    new Uri(appLink.Path),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = appLink.Identifier
                    },
                    valueSet) :
                await Launcher.LaunchUriAsync(
                    new Uri(appLink.Path),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = appLink.Identifier
                    });
        }

        static async Task<Collection<ValueInfo>> PlatformOpenUriForResults(AppLink appLink, IList<ValueInfo> inputValues)
        {
            var valueSet = new ValueSet();
            foreach (var input in inputValues ?? Enumerable.Empty<ValueInfo>())
            {
                valueSet.Add(input.Key, input.Value);
            }
            var response = valueSet.Count > 0 ?
                await Launcher.LaunchUriForResultsAsync(
                    new Uri(appLink.Path),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = appLink.Identifier
                    },
                    valueSet) :
                await Launcher.LaunchUriForResultsAsync(
                    new Uri(appLink.Path),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = appLink.Identifier
                    });

            var results = new Collection<ValueInfo>();
            foreach (var item in response.Result ?? Enumerable.Empty<KeyValuePair<string, object>>())
            {
                results.Add(new ValueInfo()
                {
                    Key = item.Key,
                    Value = item.Value
                });
            }
            return results;
        }

        static async Task<ReadOnlyCollection<IAppInfo>> PlatformFindAppHandlers(string path)
        {
            try
            {
                var uri = new Uri(path);
                var apps = await Launcher.FindUriSchemeHandlersAsync(uri.Scheme);
                var appInfos = new List<IAppInfo>();
                foreach (var item in apps)
                {
                    var appInfo = new NGAppInfo(item);
                    appInfo.Logo = await appInfo.GetLogo(new Size(100, 100));
                    appInfos.Add(appInfo);
                }
                return new ReadOnlyCollection<IAppInfo>(appInfos);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                return new ReadOnlyCollection<IAppInfo>(new List<IAppInfo>());
            }
        }

    }

}
