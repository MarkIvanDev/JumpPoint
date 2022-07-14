using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Extensions;
using JumpPoint.Extensions.NewItems;
using JumpPoint.Platform.Interop;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using Nito.AsyncEx;
using NittyGritty.Extensions;
using NittyGritty.Utilities;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.System;

namespace JumpPoint.Platform.Extensions
{
    public static partial class NewItemManager
    {
        private const string EXTENSION_CONTRACT =
#if BETA
            "com.jumppointbeta.ext.newitem";
#else
            "com.jumppoint.ext.newitem";
#endif

        private static readonly AsyncLock mutex;
        private static readonly AsyncLazy<Task> lazyInitialize;
        private static readonly List<NewItem> newItems;
        private static readonly AppExtensionCatalog catalog;

        static NewItemManager()
        {
            mutex = new AsyncLock();
            lazyInitialize = new AsyncLazy<Task>(Initialize);
            newItems = new List<NewItem>();
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
                    var tool = await ToNewItem(item);
                    newItems.Add(tool);
                }
            }
            return Task.CompletedTask;
        }

        static void AddNewItem(NewItem newItem)
        {
            var index = newItems.FindIndex(i => i.Identifier == newItem.Identifier);
            if (index == -1)
            {
                newItems.Add(newItem);
            }
            else
            {
                newItems[index] = newItem;
            }
        }

        static void RemoveNewItems(string packageId)
        {
            var packageExts = newItems.Where(i => i.PackageId == packageId).ToList();
            foreach (var item in packageExts)
            {
                newItems.Remove(item);
            }
        }

        static void UpdateStatus(string packageId, bool isAvailable)
        {
            var packageExts = newItems.Where(i => i.PackageId == packageId);
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
                    var p = await ToNewItem(item);
                    AddNewItem(p);
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
                    var p = await ToNewItem(item);
                    AddNewItem(p);
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
                RemoveNewItems(args.Package.Id.FamilyName);
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
                        RemoveNewItems(args.Package.Id.FamilyName);
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

        static async Task<NewItem> ToNewItem(AppExtension extension)
        {
            var newItem = await ExtensionBase.Extract<NewItem>(extension);

            if (await extension.GetExtensionPropertiesAsync() is PropertySet properties)
            {
                newItem.Link = properties.TryGetValue(nameof(NewItem.Link), out var l) && l is PropertySet lProp && lProp.ContainsKey("#text") ?
                    lProp["#text"].ToString() : null;
                newItem.Service = properties.TryGetValue(nameof(NewItem.Service), out var srv) && srv is PropertySet srvProp && srvProp.ContainsKey("#text") ?
                    srvProp["#text"].ToString() : null;
                newItem.FileExtension = properties.TryGetValue(nameof(NewItem.FileExtension), out var ft) && ft is PropertySet ftProp && ftProp.ContainsKey("#text") ?
                    ftProp["#text"].ToString() : null;
            }

            return newItem;
        }

        static async Task<IList<NewItem>> PlatformGetNewItems()
        {
            await lazyInitialize;

            return newItems;
        }

        static async Task PlatformRun(NewItem newItem, DirectoryBase destination)
        {
            var results = new List<NewItemResultPayload>();

            var payload = new NewItemPayload { Destination = destination.Path };
            var inputData = NewItemHelper.GetData(payload);

            if (!string.IsNullOrEmpty(newItem.Link))
            {
                var response = await Launcher.LaunchUriForResultsAsync(
                    new Uri(newItem.Link),
                    new LauncherOptions()
                    {
                        TargetApplicationPackageFamilyName = newItem.PackageId
                    },
                    inputData.ToValueSet());
                if (response.Status == LaunchUriStatus.Success && response.Result is ValueSet responseResult)
                {
                    results.AddRange(await NewItemHelper.GetResults(responseResult));
                }
            }
            else if (!string.IsNullOrEmpty(newItem.Service))
            {
                var appService = new AppServiceConnection();
                appService.AppServiceName = newItem.Service;
                appService.PackageFamilyName = newItem.PackageId;

                var status = await appService.OpenAsync();
                if (status == AppServiceConnectionStatus.Success)
                {
                    var response = await appService.SendMessageAsync(inputData.ToValueSet());
                    if (response.Status == AppServiceResponseStatus.Success && response.Message is ValueSet responseResult)
                    {
                        results.AddRange(await NewItemHelper.GetResults(responseResult));
                    }
                }
            }

            foreach (var item in results)
            {
                var folder = await FileInterop.GetStorageFolder(destination);
                if (folder != null)
                {
                    var fileName = item.FileName
                        .Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)
                        .WithEnding(Path.DirectorySeparatorChar.ToString());
                    var segments = fileName.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < segments.Length; i++)
                    {
                        var segment = segments[i];
                        if (string.IsNullOrWhiteSpace(segment))
                        {
                            continue;
                        }

                        if (folder is null)
                        {
                            continue;
                        }

                        if (i == segments.Length - 1)
                        {
                            var newFile = await CodeHelper.InvokeOrDefault(async () => await folder.CreateFileAsync(segment, Windows.Storage.CreationCollisionOption.GenerateUniqueName));
                            if (newFile != null)
                            {
                                var bytes = string.IsNullOrEmpty(item.ContentToken) ? null : await IOHelper.ReadBytes(item.ContentToken);
                                if (bytes != null)
                                {
                                    await IOHelper.WriteBytes(newFile, bytes);
                                }
                            }
                        }
                        else
                        {
                            folder = await CodeHelper.InvokeOrDefault(async () => await folder.CreateFolderAsync(segment, Windows.Storage.CreationCollisionOption.OpenIfExists));
                        }
                    }
                    
                }
            }
        }
    }
}
