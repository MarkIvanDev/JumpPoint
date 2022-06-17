using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public class DriveViewModel : ShellContextViewModelBase
    {

        public DriveViewModel(IShortcutService shortcutService) : base(shortcutService)
        {
        }

        private StorageType? _storageType;

        public StorageType? StorageType
        {
            get { return _storageType; }
            set { Set(ref _storageType, value); }
        }

        #region Shell Integration

        protected override async Task Refresh(CancellationToken token)
        {
            Item = PathInfo.Parameter is TabParameter tab && tab.Parameter is DriveBase db ?
                    db : await StorageService.GetDrive(PathInfo.Path, StorageType);

            if (!(Item is DriveBase drive))
            {
                throw new OperationCanceledException();
            }

            await JumpPointService.Load(drive);
            PathInfo.Tag = drive.DriveTemplate;

            token.ThrowIfCancellationRequested();
            var items = await StorageService.GetItems(drive);
            Items.ReplaceRange(items);

            for (int i = 0; i < items.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(items[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        #endregion

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            base.LoadState(parameter, state);
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
                else if (tab.Parameter is DriveBase drive)
                {
                    path = drive.Path;
                    StorageType = drive.StorageType;
                }
            }
            else if (state != null)
            {
                path = RestoreStateItem<string>(state, nameof(PathInfo.Path));
                StorageType = RestoreStateItem<StorageType?>(state, nameof(StorageType), null);
            }

            PathInfo.Place(path, parameter);
            await RefreshCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
            if (PathInfo.Type == AppPath.Drive)
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
