using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JumpPoint.Extensions;
using JumpPoint.Extensions.Tools;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using Nito.AsyncEx;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.System;

namespace JumpPoint.Platform.Extensions
{
    public static partial class ToolManager
    {
        private const string EXTENSION_CONTRACT =
#if BETA
            "com.jumppointbeta.ext.tool";
#else
            "com.jumppoint.ext.tool";
#endif

        private static readonly AsyncLock mutex;
        private static readonly AsyncLazy<Task> lazyInitialize;
        private static readonly List<Tool> tools;
        private static readonly AppExtensionCatalog catalog;

        static ToolManager()
        {
            mutex = new AsyncLock();
            lazyInitialize = new AsyncLazy<Task>(Initialize);
            tools = new List<Tool>();
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
                    var tool = await ToTool(item);
                    tools.Add(tool);
                }
            }
            return Task.CompletedTask;
        }

        static void AddTool(Tool tool)
        {
            var index = tools.FindIndex(i => i.Identifier == tool.Identifier);
            if (index == -1)
            {
                tools.Add(tool);
            }
            else
            {
                tools[index] = tool;
            }
        }

        static void RemoveTools(string packageId)
        {
            var packageExts = tools.Where(i => i.PackageId == packageId).ToList();
            foreach (var item in packageExts)
            {
                tools.Remove(item);
            }
        }

        static void UpdateStatus(string packageId, bool isAvailable)
        {
            var packageExts = tools.Where(i => i.PackageId == packageId);
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
                    var p = await ToTool(item);
                    AddTool(p);
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
                    var p = await ToTool(item);
                    AddTool(p);
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
                RemoveTools(args.Package.Id.FamilyName);
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
                        RemoveTools(args.Package.Id.FamilyName);
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

        static async Task<Tool> ToTool(AppExtension extension)
        {
            var tool = await ExtensionBase.Extract<Tool>(extension);
            
            if (await extension.GetExtensionPropertiesAsync() is PropertySet properties)
            {
                tool.Link = properties.TryGetValue(nameof(Tool.Link), out var l) && l is PropertySet lProp ?
                    lProp["#text"].ToString() : null;
                tool.Service = properties.TryGetValue(nameof(Tool.Service), out var srv) && srv is PropertySet srvProp ?
                    srvProp["#text"].ToString() : null;
                tool.Group = properties.TryGetValue(nameof(Tool.Group), out var g) && g is PropertySet gProp ?
                    gProp["#text"].ToString() : null;
                var fileTypes = properties.TryGetValue(nameof(Tool.FileTypes), out var ft) && ft is PropertySet ftProp ?
                    ftProp["#text"].ToString().Split(';', StringSplitOptions.RemoveEmptyEntries) : null;
                tool.FileTypes = new HashSet<string>(fileTypes, StringComparer.OrdinalIgnoreCase);
                tool.IncludeFileTokens = properties.TryGetValue(nameof(Tool.IncludeFileTokens), out var ift) && ift is PropertySet iftProp && bool.TryParse(iftProp["#text"].ToString(), out var includeFileTokens) ?
                    includeFileTokens : false;
                tool.SupportsDirectories = properties.TryGetValue(nameof(Tool.SupportsDirectories), out var sd) && sd is PropertySet sdProp && bool.TryParse(sdProp["#text"].ToString(), out var supportsDirectories) ?
                    supportsDirectories : false;
            }

            return tool;
        }

        static async Task<ToolPayload> ToToolPayload(JumpPointItem item, bool includeFileTokens)
        {
            var payload = new ToolPayload();
            payload.ItemType = item.Type.ToToolPayloadType();
            payload.Path = item.Path;
            if (item is FileBase file)
            {
                if (includeFileTokens || item.Path.GetPathKind() == PathKind.Unmounted)
                {
                    var storageFile = await StorageService.GetStorageFile(file);
                    if (storageFile != null)
                    {
                        var token = SharedStorageAccessManager.AddFile(storageFile);
                        payload.Token = token;
                    }
                }
            }
            return payload;
        }

        static async Task<IList<Tool>> PlatformGetTools()
        {
            await lazyInitialize;

            return tools;
        }

        static async Task<IList<ToolResultPayload>> PlatformRun(Tool tool, IList<JumpPointItem> items)
        {
            var results = new List<ToolResultPayload>();
            if (items.Count == 0 || !tool.IsSupported(items)) return results;

            var payloads = new List<ToolPayload>();
            foreach (var item in items)
            {
                var payload = await ToToolPayload(item, tool.IncludeFileTokens);
                payloads.Add(payload);
            }
            var inputData = await ToolHelper.GetData(payloads);

            if (!string.IsNullOrEmpty(tool.Link))
            {
                var response = await Launcher.LaunchUriForResultsAsync(
                    new Uri(tool.Link),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = tool.PackageId
                    },
                    inputData.ToValueSet());
                if (response.Status == LaunchUriStatus.Success && response.Result is ValueSet responseResult)
                {
                    results.AddRange(await ToolHelper.GetResults(responseResult));
                }
            }
            else if (!string.IsNullOrEmpty(tool.Service))
            {
                var appService = new AppServiceConnection();
                appService.AppServiceName = tool.Service;
                appService.PackageFamilyName = tool.PackageId;

                var status = await appService.OpenAsync();
                if (status == AppServiceConnectionStatus.Success)
                {
                    var response = await appService.SendMessageAsync(inputData.ToValueSet());
                    if (response.Status == AppServiceResponseStatus.Success && response.Message is ValueSet responseResult)
                    {
                        results.AddRange(await ToolHelper.GetResults(responseResult));
                    }
                }
            }

            // Clean up generated tokens
            foreach (var payload in payloads)
            {
                if (!string.IsNullOrEmpty(payload.Token))
                {
                    SharedStorageAccessManager.RemoveFile(payload.Token);
                }
            }

            if (results.Count == 0)
            {
                results.AddRange(payloads.Select(i => new ToolResultPayload
                {
                    Result = ToolResult.Unknown,
                    Path = i.Path
                }));
            }
            
            return results;
        }

    }
}
