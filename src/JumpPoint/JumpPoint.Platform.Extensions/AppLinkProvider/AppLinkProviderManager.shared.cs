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

        public static event ExtensionInstalledEventHandler<AppLinkProvider> ExtensionInstalled;
        public static event ExtensionUpdatedEventHandler<AppLinkProvider> ExtensionUpdated;
        public static event ExtensionUpdatingEventHandler ExtensionUpdating;
        public static event ExtensionUninstallingEventHandler ExtensionUninstalling;
        public static event ExtensionStatusChangedEventHandler ExtensionStatusChanged;

        public static void Start()
            => PlatformStart();

        public static void Stop()
            => PlatformStop();

        public static async Task<IList<AppLinkProvider>> GetProviders()
            => await PlatformGetProviders();

        public static async Task<AppLinkInfo> Pick(AppLinkProvider provider)
            => await PlatformPick(provider);

        public static async Task<IList<AppLinkPayload>> GetLocalAppLinks()
            => await PlatformGetLocalAppLinks();

    }

}
