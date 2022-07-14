using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation.Collections;
using Windows.System;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Extensions;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Nito.AsyncEx;
using JumpPoint.Extensions.AppLinkProviders;

namespace JumpPoint.Platform.Extensions
{
    public static partial class AppLinkProviderManager
    {
        private const string EXTENSION_CONTRACT =
#if JPBETA
            "com.jumppointbeta.ext.applinkprovider";
#else
            "com.jumppoint.ext.applinkprovider";
#endif

        private static readonly AsyncLock mutex;
        private static readonly AsyncLazy<Task> lazyInitialize;
        private static readonly List<AppLinkProvider> providers;
        private static readonly AppExtensionCatalog catalog;

        static AppLinkProviderManager()
        {
            mutex = new AsyncLock();
            lazyInitialize = new AsyncLazy<Task>(Initialize);
            providers = new List<AppLinkProvider>();
            catalog = AppExtensionCatalog.Open(EXTENSION_CONTRACT);
            catalog.PackageInstalled += Catalog_PackageInstalled;
            catalog.PackageUpdated += Catalog_PackageUpdated;
            catalog.PackageUninstalling += Catalog_PackageUninstalling;
            catalog.PackageUpdating += Catalog_PackageUpdating;
            catalog.PackageStatusChanged += Catalog_PackageStatusChanged;
        }

        #region Monitor Changes

        static async Task<Task> Initialize()
        {
            using (await mutex.LockAsync())
            {
                var appExtensions = await catalog.FindAllAsync();
                foreach (var item in appExtensions)
                {
                    var picker = await ToAppLinkProvider(item);
                    providers.Add(picker);
                }
            }
            return Task.CompletedTask;
        }

        static void AddProvider(AppLinkProvider provider)
        {
            var index = providers.FindIndex(i => i.Identifier == provider.Identifier);
            if (index == -1)
            {
                providers.Add(provider);
            }
            else
            {
                providers[index] = provider;
            }
        }

        static void RemoveProviders(string packageId)
        {
            var packageExts = providers.Where(i => i.PackageId == packageId).ToList();
            foreach (var item in packageExts)
            {
                providers.Remove(item);
            }
        }

        static void UpdateStatus(string packageId , bool isAvailable)
        {
            var packageExts = providers.Where(i => i.PackageId == packageId);
            foreach (var item in packageExts)
            {
                item.IsAvailable = isAvailable;
            }
        }

        static async void Catalog_PackageInstalled(AppExtensionCatalog sender, AppExtensionPackageInstalledEventArgs args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                foreach (var item in args.Extensions)
                {
                    var p = await ToAppLinkProvider(item);
                    AddProvider(p);
                }
            }
            ExtensionCollectionChanged?.Invoke(null, EventArgs.Empty);
        }

        static async void Catalog_PackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                foreach (var item in args.Extensions)
                {
                    var p = await ToAppLinkProvider(item);
                    AddProvider(p);
                }
            }
            ExtensionCollectionChanged?.Invoke(null, EventArgs.Empty);
        }

        static async void Catalog_PackageUpdating(AppExtensionCatalog sender, AppExtensionPackageUpdatingEventArgs args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                UpdateStatus(args.Package.Id.FamilyName, args.Package.Status.VerifyIsOK());
            }
            ExtensionCollectionChanged?.Invoke(null, EventArgs.Empty);
        }

        static async void Catalog_PackageUninstalling(AppExtensionCatalog sender, AppExtensionPackageUninstallingEventArgs args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                RemoveProviders(args.Package.Id.FamilyName);
            }
            ExtensionCollectionChanged?.Invoke(null, EventArgs.Empty);
        }

        static async void Catalog_PackageStatusChanged(AppExtensionCatalog sender, AppExtensionPackageStatusChangedEventArgs args)
        {
            await lazyInitialize;

            using (await mutex.LockAsync())
            {
                if (!args.Package.Status.VerifyIsOK())
                {
                    if (args.Package.Status.PackageOffline)
                    {
                        UpdateStatus(args.Package.Id.FamilyName, args.Package.Status.VerifyIsOK());
                    }
                    else if (args.Package.Status.Servicing || args.Package.Status.DeploymentInProgress)
                    {
                        // if the package is being serviced or deployed, ignore the status events
                    }
                    else
                    {
                        RemoveProviders(args.Package.Id.FamilyName);
                    }
                }
                else
                {
                    UpdateStatus(args.Package.Id.FamilyName, args.Package.Status.VerifyIsOK());
                }
            }
            ExtensionCollectionChanged?.Invoke(null, EventArgs.Empty);
        }

        #endregion

        static async Task<AppLinkProvider> ToAppLinkProvider(AppExtension extension)
        {
            var provider = await ExtensionBase.Extract<AppLinkProvider>(extension);

            if (await extension.GetExtensionPropertiesAsync() is PropertySet properties)
            {
                provider.Link = properties.TryGetValue(nameof(AppLinkProvider.Link), out var l) && l is PropertySet lProp && lProp.ContainsKey("#text") ?
                    lProp["#text"].ToString() : null;
                provider.Service = properties.TryGetValue(nameof(AppLinkProvider.Service), out var srv) && srv is PropertySet srvProp && srvProp.ContainsKey("#text") ?
                    srvProp["#text"].ToString() : null;
            }

            return provider;
        }

        static async Task<IList<AppLinkProvider>> PlatformGetProviders()
        {
            await lazyInitialize;

            return providers;
        }

        static async Task<AppLinkInfo> PlatformPick(AppLinkProvider provider)
        {
            LaunchUriResult response = null;

            if (!string.IsNullOrEmpty(provider.Link))
            {
                response = await Launcher.LaunchUriForResultsAsync(
                    new Uri(provider.Link),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = provider.PackageId
                    });
            }
            else if (!string.IsNullOrEmpty(provider.Service))
            {
                response = await Launcher.LaunchUriForResultsAsync(
                    new UriBuilder(Prefix.PICKER_SCHEME, PickerPath.AppLinkProvider.ToString()).Uri,
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = Package.Current.Id.FamilyName
                    },
                    new ValueSet()
                    {
                        { nameof(AppLinkProvider.Name), provider.Name },
                        { nameof(AppLinkProvider.Service), provider.Service },
                        { nameof(AppLinkProvider.PackageId), provider.PackageId }
                    });
            }

            if (response != null && response.Status == LaunchUriStatus.Success && response.Result is ValueSet values)
            {
                var payload = new AppLinkPayload();
                payload.Link = values.TryGetValue(nameof(AppLinkPayload.Link), out var l) && l is string link ?
                    link : null;
                payload.Name = values.TryGetValue(nameof(AppLinkPayload.Name), out var dn) && dn is string displayName && !string.IsNullOrWhiteSpace(displayName) ?
                    displayName : provider.Package;
                payload.Description = values.TryGetValue(nameof(AppLinkPayload.Description), out var ds) && ds is string description ?
                    description : string.Empty;
                payload.AppName = values.TryGetValue(nameof(AppLinkPayload.AppName), out var an) && an is string appName && !string.IsNullOrWhiteSpace(appName) ?
                    appName : null;
                payload.AppId = values.TryGetValue(nameof(AppLinkPayload.AppId), out var id) && id is string identifier && !string.IsNullOrWhiteSpace(identifier) ?
                    identifier : null;
                payload.Logo = values.TryGetValue(nameof(AppLinkPayload.Logo), out var lg) && lg is byte[] logo ?
                    logo : provider.Logo.ToByteArray();
                payload.Background = values.TryGetValue(nameof(AppLinkPayload.Background), out var bg) && bg is string background && background.IsHexColor() ?
                    background : "#00FFFFFF";
                payload.LaunchTypes = values.TryGetValue(nameof(AppLinkPayload.LaunchTypes), out var lt) && lt is int launchTypes && launchTypes.IsLaunchTypeDefined() ?
                    launchTypes : (int)AppLinkLaunchTypes.Uri;
                payload.QueryKeys = values.TryGetValue(nameof(AppLinkPayload.QueryKeys), out var qk) && qk is string[] queryKeys ?
                    queryKeys.RemoveEmptyEntries() : Array.Empty<string>();
                payload.InputKeys = values.TryGetValue(nameof(AppLinkPayload.InputKeys), out var ik) && ik is string[] inputKeys ?
                    inputKeys : Array.Empty<string>();
                return payload.ToAppLinkInfo();
            }

            return null;
        }

        static async Task<IList<AppLinkPayload>> PlatformGetPayloads(string service, string packageId)
        {
            var payloads = new List<AppLinkPayload>();

            var appService = new AppServiceConnection();
            appService.AppServiceName = service;
            appService.PackageFamilyName = packageId;

            var status = await appService.OpenAsync();
            if (status == AppServiceConnectionStatus.Success)
            {
                var response = await appService.SendMessageAsync(new ValueSet() { { "Action", "GetPayloads" } });
                if (response.Status == AppServiceResponseStatus.Success && response.Message is ValueSet values)
                {
                    return await AppLinkProviderHelper.GetPayloads(values);
                }
            }

            return payloads;
        }

    }
}
