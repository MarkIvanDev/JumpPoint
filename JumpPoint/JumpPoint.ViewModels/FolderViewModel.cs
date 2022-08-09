using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.FullTrust.Core.ChangeNotifier;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Models;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class FolderViewModel : ShellContextViewModelBase
    {

        public FolderViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {
        }

        private StorageType? _storageType;

        public StorageType? StorageType
        {
            get { return _storageType; }
            set { Set(ref _storageType, value); }
        }

        protected override async Task Refresh(CancellationToken token)
        {
            Item = PathInfo.Parameter is TabParameter tab && tab.Parameter is FolderBase fb ?
                fb : await StorageService.GetFolder(PathInfo.Path, StorageType);

            if (!(Item is FolderBase folder))
            {
                throw new OperationCanceledException();
            }
            await JumpPointService.Load(folder);
            PathInfo.Tag = folder.FolderTemplate;
            token.ThrowIfCancellationRequested();

            var items = await StorageService.GetItems(folder);
            Items.AddRange(items);
            token.ThrowIfCancellationRequested();

            for (int i = 0; i < items.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(items[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        protected override async Task Initialize(object parameter, Dictionary<string, object> state)
        {
            string path = null;
            if (parameter is TabParameter tab)
            {
                if (tab.Parameter is string queryString)
                {
                    var args = QueryString.Parse(queryString);
                    path = args[nameof(PathInfo.Path)];
                    StorageType = args.TryGetValue(nameof(DriveBase.StorageType), out var st) &&
                        Enum.TryParse<StorageType>(st, true, out var storageType) ?
                        storageType : (StorageType?)null;
                }
                else if (tab.Parameter is FolderBase folder)
                {
                    path = folder.Path;
                    StorageType = folder.StorageType;
                }
            }
            else if (state != null)
            {
                path = RestoreStateItem<string>(state, nameof(PathInfo.Path));
                StorageType = RestoreStateItem<StorageType?>(state, nameof(StorageType), null);
            }

            PathInfo.Place(path, parameter);
            await RefreshCommand.TryExecute();
            if (StorageType == Platform.Items.Storage.StorageType.Local)
            {
                ChangeNotifierService.Start(PathInfo.Path, ItemsChanged);
            }
        }

        private async void ItemsChanged(NotifyChange change)
        {
            await Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(async () =>
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Created:
                        StorageItemBase itemToCreate = change.IsDirectory ?
                            (StorageItemBase)(await StorageService.GetFolder(change.FullPath, StorageType)) :
                            await StorageService.GetFile(change.FullPath, StorageType);
                        if (itemToCreate != null)
                        {
                            Items.Add(itemToCreate);
                            await JumpPointService.Load(itemToCreate);
                        }
                        break;

                    case ChangeType.Deleted:
                        var itemToDelete = Items.FirstOrDefault(i => i.Name.Equals(change.Name, StringComparison.OrdinalIgnoreCase));
                        if (itemToDelete != null)
                        {
                            Items.Remove(itemToDelete);
                        }
                        break;

                    case ChangeType.Changed:
                        var itemToLoad = (StorageItemBase)Items.FirstOrDefault(i => i.Name.Equals(change.Name, StringComparison.OrdinalIgnoreCase));
                        StorageItemBase refreshedItem = null;
                        if (itemToLoad is FolderBase folder)
                        {
                            refreshedItem = await StorageService.GetFolder(folder.Path, StorageType);
                        }
                        else if (itemToLoad is FileBase file)
                        {
                            refreshedItem = await StorageService.GetFile(file.Path, StorageType);
                        }

                        if (refreshedItem != null)
                        {
                            itemToLoad.Refresh(refreshedItem);
                            await JumpPointService.Load(itemToLoad);
                        }
                        break;

                    case ChangeType.Renamed:
                        var itemToRename = (StorageItemBase)Items.FirstOrDefault(i => i.Name.Equals(change.OldName, StringComparison.OrdinalIgnoreCase));
                        if (itemToRename != null)
                        {
                            itemToRename.Path = change.FullPath;
                        }
                        break;

                    case ChangeType.Unknown:
                    default:
                        await RefreshCommand.TryExecute();
                        break;
                }
            });
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
            if (StorageType == Platform.Items.Storage.StorageType.Local)
            {
                ChangeNotifierService.Stop(PathInfo.Path, ItemsChanged); 
            }
            if (PathInfo.Type == AppPath.Folder)
            {
                state[nameof(PathInfo.Path)] = PathInfo.Path;
            }
            if (StorageType.HasValue)
            {
                state[nameof(StorageType)] = StorageType.Value;
                StorageType = null;
            }
        }
    }
}
