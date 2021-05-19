using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using Newtonsoft.Json;
using Nito.AsyncEx;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;
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
            var packageExts = tools.Where(i => i.PackageId == packageId);
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
            payload.ItemType = item.Type;
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

        static async Task<ToolResult> PlatformRun(Tool tool, IList<JumpPointItem> items)
        {
            if (items.Count == 0 || !tool.IsSupported(items)) return ToolResult.Nothing;

            var inputData = new ValueSet();
            var payloads = new List<ToolPayload>();
            if (items.Count == 1)
            {
                var payload = await ToToolPayload(items[0], tool.IncludeFileTokens);
                inputData.Add(nameof(ToolPayload.ItemType), payload.ItemType.ToString());
                inputData.Add(nameof(ToolPayload.Path), payload.Path);
                inputData.Add(nameof(ToolPayload.Token), payload.Token);
                payloads.Add(payload);
            }
            else
            {
                foreach (var item in items)
                {
                    var payload = await ToToolPayload(item, tool.IncludeFileTokens);
                    payloads.Add(payload);
                }
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
                var json = JsonConvert.SerializeObject(payloads);
                await file.WriteText(json);
                var token = SharedStorageAccessManager.AddFile(file);
                inputData.Add(nameof(ToolPayload), token);
            }

            var result = ToolResult.Nothing;
            if (!string.IsNullOrEmpty(tool.Link))
            {
                var response = await Launcher.LaunchUriForResultsAsync(
                    new Uri(tool.Link),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = tool.PackageId
                    },
                    inputData);
                if (response.Status == LaunchUriStatus.Success && response.Result is ValueSet responseResult)
                {
                    if (responseResult.TryGetValue(nameof(ToolResult), out var tr) && Enum.TryParse<ToolResult>(tr?.ToString(), true, out var r))
                    {
                        result = r;
                    }
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
                    var response = await appService.SendMessageAsync(inputData);
                    if (response.Status == AppServiceResponseStatus.Success && response.Message is ValueSet responseResult)
                    {
                        if (responseResult.TryGetValue(nameof(ToolResult), out var tr) && Enum.TryParse<ToolResult>(tr?.ToString(), true, out var r))
                        {
                            result = r;
                        }
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

            return result;
        }

        static async Task<IList<ToolPayload>> PlatformExtractPayloads(IReadOnlyDictionary<string, object> data)
        {
            try
            {
                var payloads = new List<ToolPayload>();
                if (data.TryGetValue(nameof(ToolPayload), out var tp))
                {
                    var token = tp?.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                        var text = await FileIO.ReadTextAsync(file);
                        payloads.AddRange(JsonConvert.DeserializeObject<List<ToolPayload>>(text));
                    }
                }
                else if (data.TryGetValue(nameof(ToolPayload.ItemType), out var it) &&
                    data.TryGetValue(nameof(ToolPayload.Path), out var p) &&
                    data.TryGetValue(nameof(ToolPayload.Token), out var t))
                {
                    payloads.Add(new ToolPayload
                    {
                        ItemType = Enum.TryParse<JumpPointItemType>(it?.ToString(), true, out var itemType) ? itemType : JumpPointItemType.Unknown,
                        Path = p?.ToString(),
                        Token = t?.ToString()
                    });
                }
                return payloads;
            }
            catch (Exception)
            {
                return new List<ToolPayload>();
            }
        }

        public static async Task<StorageFile> GetFile(ToolPayload payload)
        {
            try
            {
                var file = !string.IsNullOrEmpty(payload.Token) ?
                    await SharedStorageAccessManager.RedeemTokenForFileAsync(payload.Token) :
                    await StorageFile.GetFileFromPathAsync(payload.Path);
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
