using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using NGStorage = NittyGritty.Platform.Storage;
using Windows.Storage;
using Windows.System;
using Windows.ApplicationModel.DataTransfer;
using NittyGritty.Models;
using System.Collections.ObjectModel;
using JumpPoint.Platform.Models;
using Newtonsoft.Json;

namespace JumpPoint.Platform.Services
{
    public static partial class JumpPointService
    {

        static async Task<NGStorage.IStorageItem> PlatformConvert(StorageItemBase item)
        {
            if (item is FileBase file)
            {
                var storageFile = await StorageService.GetStorageFile(file);
                if (storageFile != null)
                {
                    return new NGStorage.NGFile(storageFile);
                }
            }
            else if (item is DirectoryBase directory)
            {
                var storageFolder = await StorageService.GetStorageFolder(directory);
                if (storageFolder != null)
                {
                    return new NGStorage.NGFolder(storageFolder);
                }
            }
            return null;
        }

        static async Task<IList<NGStorage.IStorageItem>> PlatformConvert(IEnumerable<JumpPointItem> items)
        {
            var list = new List<NGStorage.IStorageItem>();
            foreach (var item in items)
            {
                if (item is StorageItemBase fs)
                {
                    var i = await PlatformConvert(fs);
                    if (i != null)
                    {
                        list.Add(i);
                    }
                }
                else if (item is Workspace ws)
                {
                    var wsFiles = await WorkspaceService.GetItems(ws.Id, JumpPointItemType.File);
                    foreach (var wsf in wsFiles.Cast<StorageItemBase>())
                    {
                        var i = await PlatformConvert(wsf);
                        if (i != null)
                        {
                            list.Add(i);
                        }
                    }
                    var wsFolders = await WorkspaceService.GetItems(ws.Id, JumpPointItemType.Folder);
                    foreach (var wsf in wsFolders.Cast<StorageItemBase>())
                    {
                        var i = await PlatformConvert(wsf);
                        if (i != null)
                        {
                            list.Add(i);
                        }
                    }
                }
                //else if (item is ILibrary lib)
                //{
                //    var libFolders = await PlatformGetLibraryFolders(lib.LibraryTemplate);
                //    foreach (var libf in libFolders)
                //    {
                //        var i = await Convert(libf);
                //        if (i != null)
                //        {
                //            list.Add(i);
                //        }
                //    }
                //}
            }
            return list;
        }

        static async Task<bool> PlatformOpenFile(FileBase file, bool useDefaultHandler)
        {
            var item = await StorageService.GetStorageFile(file);
            return item != null ?
                await Launcher.LaunchFileAsync(item, new LauncherOptions { DisplayApplicationPicker = !useDefaultHandler }) :
                false;
        }

        static async Task PlatformOpenInFileExplorer(string path, IEnumerable<StorageItemBase> itemsToSelect = null)
        {
            if (itemsToSelect == null || !itemsToSelect.Any())
            {
                await Launcher.LaunchFolderPathAsync(path);
            }
            else
            {
                var options = new FolderLauncherOptions();
                foreach (var item in itemsToSelect)
                {
                    if (item is FileBase file)
                    {
                        var storageFile = await StorageService.GetStorageFile(file);
                        if (storageFile != null)
                        {
                            options.ItemsToSelect.Add(storageFile);
                        }
                    }
                    else if (item is DirectoryBase directory)
                    {
                        var storageFolder = await StorageService.GetStorageFolder(directory);
                        if (storageFolder != null)
                        {
                            options.ItemsToSelect.Add(storageFolder);
                        }
                    }
                }
                await Launcher.LaunchFolderPathAsync(path, options);
            }
        }

        static async Task PlatformOpenProperties(Collection<Seed> seeds)
        {
            var folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Properties", CreationCollisionOption.OpenIfExists);
            var file = await folder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
            await CreatePropertiesPayload();
            var seedsToken = SharedStorageAccessManager.AddFile(file);
            var properties = new UriBuilder()
            {
                Scheme = SCHEME,
                Host = "properties",
                Query = new QueryString() { { nameof(seedsToken), seedsToken } }.ToString()
            }.Uri;
            await Launcher.LaunchUriAsync(properties);

            async Task CreatePropertiesPayload()
            {
                int retryAttempts = 3;
                const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
                const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);
                const int ERROR_UNABLE_TO_REMOVE_REPLACED = unchecked((int)0x80070497);

                // Application now has read/write access to the picked file.
                while (retryAttempts > 0)
                {
                    try
                    {
                        retryAttempts--;
                        var contents = JsonConvert.SerializeObject(seeds);
                        await FileIO.WriteTextAsync(file, contents, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                        break;
                    }
                    catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) ||
                                               (ex.HResult == ERROR_SHARING_VIOLATION) ||
                                               (ex.HResult == ERROR_UNABLE_TO_REMOVE_REPLACED))
                    {
                        // This might be recovered by retrying, otherwise let the exception be raised.
                        // The app can decide to wait before retrying.
                    }
                }
            }
        }

    }
}
