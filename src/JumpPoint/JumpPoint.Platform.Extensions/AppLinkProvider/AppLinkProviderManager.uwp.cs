using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using Windows.ApplicationModel.AppExtensions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using JumpPoint.Platform.Models.Extensions;
using System.Collections.ObjectModel;
using NittyGritty.Extensions;
using NittyGritty.Platform.Storage;
using Windows.ApplicationModel;
using Humanizer;
using Windows.Storage;

namespace JumpPoint.Platform.Extensions
{
    public static partial class AppLinkProviderManager
    {
        private const string EXTENSION_CONTRACT =
#if BETA
            "com.jumppointbeta.ext.applinkprovider";
#else
            "com.jumppoint.ext.applinkprovider";
#endif


        private static readonly AppExtensionCatalog catalog;

        static AppLinkProviderManager()
        {
            catalog = AppExtensionCatalog.Open(EXTENSION_CONTRACT);
        }

        #region Monitor Changes

        static void PlatformStart()
        {
            catalog.PackageInstalled += Catalog_PackageInstalled;
            catalog.PackageUpdated += Catalog_PackageUpdated;
            catalog.PackageUninstalling += Catalog_PackageUninstalling;
            catalog.PackageUpdating += Catalog_PackageUpdating;
            catalog.PackageStatusChanged += Catalog_PackageStatusChanged;
        }

        static async void Catalog_PackageInstalled(AppExtensionCatalog sender, AppExtensionPackageInstalledEventArgs args)
        {
            var exts = new List<AppLinkProvider>();
            foreach (var item in args.Extensions)
            {
                exts.Add(await ToAppLinkProvider(item));
            }
            ExtensionInstalled?.Invoke(null, new ExtensionInstalledEventArgs(exts));
        }

        static async void Catalog_PackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            var exts = new List<AppLinkProvider>();
            foreach (var item in args.Extensions)
            {
                exts.Add(await ToAppLinkProvider(item));
            }
            ExtensionUpdated?.Invoke(null, new ExtensionUpdatedEventArgs(exts));
        }

        static void Catalog_PackageUpdating(AppExtensionCatalog sender, AppExtensionPackageUpdatingEventArgs args)
        {
            ExtensionUpdating?.Invoke(null, new ExtensionUpdatingEventArgs(args.Package.Id.FullName));
        }

        static void Catalog_PackageUninstalling(AppExtensionCatalog sender, AppExtensionPackageUninstallingEventArgs args)
        {
            ExtensionUninstalling?.Invoke(null, new ExtensionUninstallingEventArgs(args.Package.Id.FullName));
        }

        static void Catalog_PackageStatusChanged(AppExtensionCatalog sender, AppExtensionPackageStatusChangedEventArgs args)
        {
            if (!args.Package.Status.VerifyIsOK())
            {
                if (args.Package.Status.Servicing || args.Package.Status.DeploymentInProgress)
                {
                    ExtensionStatusChanged?.Invoke(null, new ExtensionStatusChangedEventArgs(args.Package.Id.FullName, null));
                }
                else
                {
                    ExtensionStatusChanged?.Invoke(null, new ExtensionStatusChangedEventArgs(args.Package.Id.FullName, false));
                }
            }
            else
            {
                ExtensionStatusChanged?.Invoke(null, new ExtensionStatusChangedEventArgs(args.Package.Id.FullName, true));
            }
        }

        static void PlatformStop()
        {
            catalog.PackageInstalled += Catalog_PackageInstalled;
            catalog.PackageUpdated += Catalog_PackageUpdated;
            catalog.PackageUninstalling += Catalog_PackageUninstalling;
            catalog.PackageUpdating += Catalog_PackageUpdating;
            catalog.PackageStatusChanged += Catalog_PackageStatusChanged;
        }

        #endregion

        static async Task<AppLinkProvider> ToAppLinkProvider(AppExtension extension)
        {
            var provider = new AppLinkProvider();
            provider.Name = extension.DisplayName;
            provider.Description = extension.Description;
            provider.PackageId = extension.Package.Id.FamilyName;
            provider.Package = extension.Package.DisplayName;
            provider.ExtensionId = extension.Id;
            provider.Publisher = extension.Package.PublisherDisplayName;
            provider.Version = $"{extension.Package.Id.Version.Major}.{extension.Package.Id.Version.Minor}.{extension.Package.Id.Version.Build}.{extension.Package.Id.Version.Revision}";
            provider.IsAvailable = extension.Package.Status.VerifyIsOK();
            provider.Signature = (ExtensionSignature)extension.Package.SignatureKind;

            var folder = await extension.GetPublicFolderAsync();
            provider.Folder = folder is null ? null : new NGFolder(folder);

            var logo = folder != null && await folder.TryGetItemAsync("Logo.png") is StorageFile logoFile ?
                await logoFile.OpenReadAsync() : 
                await extension.AppInfo.DisplayInfo.GetLogo(new Size(1, 1)).OpenReadAsync();
            provider.Logo = logo.AsStream();

            if (await extension.GetExtensionPropertiesAsync() is PropertySet properties)
            {
                provider.Link = properties.ContainsKey(nameof(AppLinkProvider.Link)) && properties[nameof(AppLinkProvider.Link)] is PropertySet property ?
                    property["#text"].ToString() : null;
            }

            return provider;
        }

        static async Task<IList<AppLinkProvider>> PlatformGetProviders()
        {
            var pickers = new List<AppLinkProvider>();
            var appExtensions = await catalog.FindAllAsync();
            foreach (var item in appExtensions)
            {
                var picker = await ToAppLinkProvider(item);
                pickers.Add(picker);
            }
            return pickers;
        }

        static async Task<AppLinkInfo> PlatformPick(AppLinkProvider provider)
        {
            var response = await Launcher.LaunchUriForResultsAsync(
                new Uri(provider.Link),
                new LauncherOptions()
                {
                    TargetApplicationPackageFamilyName = provider.PackageId
                });
            if (response.Status == LaunchUriStatus.Success && response.Result is ValueSet values)
            {
                var payload = new AppLinkPayload();
                payload.Link = values.TryGetValue(nameof(AppLinkPayload.Link), out var l) && l is string link ?
                    link : null;
                payload.Name = values.TryGetValue(nameof(AppLinkPayload.Name), out var dn) && dn is string displayName && !string.IsNullOrWhiteSpace(displayName) ?
                    displayName : provider.Package;
                payload.Description = values.TryGetValue(nameof(AppLinkPayload.Description), out var ds) && ds is string description ?
                    description : string.Empty;
                payload.AppName = values.TryGetValue(nameof(AppLinkPayload.AppName), out var an) && an is string appName && !string.IsNullOrWhiteSpace(appName) ?
                    appName : provider.Package;
                payload.AppId = values.TryGetValue(nameof(AppLinkPayload.AppId), out var id) && id is string identifier && !string.IsNullOrWhiteSpace(identifier) ?
                    identifier : provider.PackageId;
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

        static async Task<IList<AppLinkPayload>> PlatformGetLocalAppLinks()
        {
            var appName = Package.Current.DisplayName;
            var appId = Package.Current.Id.FamilyName;
            var logoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(@"ms-appx:///Assets/Logo.png"));
            var logoStream = (await logoFile.OpenReadAsync()).AsStream();
            var logo = logoStream.ToByteArray();

            return new List<AppLinkPayload>()
            {
                GetPayload(PathType.Dashboard),
                GetPayload(PathType.Settings),
                GetPayload(PathType.Favorites),
                GetPayload(PathType.Drives),
                GetPayload(PathType.CloudStorages),
                GetPayload(PathType.Workspaces),
                GetPayload(PathType.AppLinks)
            };

            AppLinkPayload GetPayload(PathType pathType)
            {
                return new AppLinkPayload
                {
                    Link = $@"{Prefix.MAIN_SCHEME}://{pathType.ToString().ToLower()}",
                    Name = $"{appName} {pathType.Humanize()}",
                    Description = $"{appName} {pathType.Humanize()}",
                    AppName = appName,
                    AppId = appId,
                    LaunchTypes = (int)AppLinkLaunchTypes.Uri,
                    Logo = logo
                };
            }
        }

    }
}
