using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using Newtonsoft.Json;
using NittyGritty.Extensions;
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


        private static readonly AppExtensionCatalog catalog;

        static ToolManager()
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
            var exts = new List<Tool>();
            foreach (var item in args.Extensions)
            {
                exts.Add(await ToTool(item));
            }
            ExtensionInstalled?.Invoke(null, new ExtensionInstalledEventArgs<Tool>(exts));
        }

        static async void Catalog_PackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            var exts = new List<Tool>();
            foreach (var item in args.Extensions)
            {
                exts.Add(await ToTool(item));
            }
            ExtensionUpdated?.Invoke(null, new ExtensionUpdatedEventArgs<Tool>(exts));
        }

        static void Catalog_PackageUpdating(AppExtensionCatalog sender, AppExtensionPackageUpdatingEventArgs args)
        {
            ExtensionUpdating?.Invoke(null, new ExtensionUpdatingEventArgs(args.Package.Id.FamilyName));
        }

        static void Catalog_PackageUninstalling(AppExtensionCatalog sender, AppExtensionPackageUninstallingEventArgs args)
        {
            ExtensionUninstalling?.Invoke(null, new ExtensionUninstallingEventArgs(args.Package.Id.FamilyName));
        }

        static void Catalog_PackageStatusChanged(AppExtensionCatalog sender, AppExtensionPackageStatusChangedEventArgs args)
        {
            if (!args.Package.Status.VerifyIsOK())
            {
                if (args.Package.Status.Servicing || args.Package.Status.DeploymentInProgress)
                {
                    ExtensionStatusChanged?.Invoke(null, new ExtensionStatusChangedEventArgs(args.Package.Id.FamilyName, null));
                }
                else
                {
                    ExtensionStatusChanged?.Invoke(null, new ExtensionStatusChangedEventArgs(args.Package.Id.FamilyName, false));
                }
            }
            else
            {
                ExtensionStatusChanged?.Invoke(null, new ExtensionStatusChangedEventArgs(args.Package.Id.FamilyName, true));
            }
        }

        static void PlatformStop()
        {
            catalog.PackageInstalled -= Catalog_PackageInstalled;
            catalog.PackageUpdated -= Catalog_PackageUpdated;
            catalog.PackageUninstalling -= Catalog_PackageUninstalling;
            catalog.PackageUpdating -= Catalog_PackageUpdating;
            catalog.PackageStatusChanged -= Catalog_PackageStatusChanged;
        }

        #endregion

        static async Task<Tool> ToTool(AppExtension extension)
        {
            var tool = await ExtensionBase.Extract<Tool>(extension);
            
            if (await extension.GetExtensionPropertiesAsync() is PropertySet properties)
            {
                tool.Link = properties.TryGetValue(nameof(Tool.Link), out var l) && l is PropertySet linkProperty ?
                    linkProperty["#text"].ToString() : null;
                tool.Service = properties.TryGetValue(nameof(Tool.Service), out var srv) && srv is PropertySet serviceProperty ?
                    serviceProperty["#text"].ToString() : null;
                tool.Group = properties.TryGetValue(nameof(Tool.Group), out var g) && g is PropertySet groupProperty ?
                    groupProperty["#text"].ToString() : null;
                var fileTypes = properties.TryGetValue(nameof(Tool.FileTypes), out var ft) && ft is PropertySet fileTypesProperty ?
                    fileTypesProperty["#text"].ToString().Split(';', StringSplitOptions.RemoveEmptyEntries) : null;
                tool.FileTypes = new HashSet<string>(fileTypes, StringComparer.OrdinalIgnoreCase);
                tool.IncludeFileTokens = properties.TryGetValue(nameof(Tool.IncludeFileTokens), out var ift) && ift is PropertySet includeFileTokensProperty && bool.TryParse(includeFileTokensProperty["#text"].ToString(), out var includeFileTokens) ?
                    includeFileTokens : false;
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
            var tools = new List<Tool>();
            var appExtensions = await catalog.FindAllAsync();
            foreach (var item in appExtensions)
            {
                var tool = await ToTool(item);
                tools.Add(tool);
            }
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
