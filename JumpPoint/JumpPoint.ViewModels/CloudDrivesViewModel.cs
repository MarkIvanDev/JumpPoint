using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class CloudDrivesViewModel : ShellContextViewModelBase
    {

        public CloudDrivesViewModel(IShortcutService shortcutService) : base(shortcutService)
        {

        }

        public override bool HasCustomGrouping => true;

        protected override async Task Refresh(CancellationToken token)
        {
            var drives = await CloudStorageService.GetDrives();
            Items.ReplaceRange(drives);

            for (int i = 0; i < drives.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(drives[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            base.LoadState(parameter, state);
            PathInfo.Place(nameof(AppPath.CloudDrives), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
