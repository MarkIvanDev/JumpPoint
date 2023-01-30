using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using JumpPoint.Platform;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Commands;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class CloudDrivesViewModel : ShellContextViewModelBase
    {
        private readonly CommandHelper commandHelper;

        public CloudDrivesViewModel(IShortcutService shortcutService, CommandHelper commandHelper, AppSettings appSettings) : base(shortcutService, appSettings)
        {
            this.commandHelper = commandHelper;
        }

        public override bool HasCustomGrouping => true;

        private AsyncRelayCommand<CloudStorageProvider?> _AddAccount;
        public AsyncRelayCommand<CloudStorageProvider?> AddAccountCommand => _AddAccount ?? (_AddAccount = new AsyncRelayCommand<CloudStorageProvider?>(
            async (provider) =>
            {
                if (provider.HasValue)
                {
                    var account = await commandHelper.AddCloudAccount(provider.Value);
                    if (account != null)
                    {
                        await RefreshCommand.TryExecute();
                        Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                                new SidebarMessage(CollectionChangedAction.Reset, null), nameof(AppPath.CloudDrives)),
                                MessengerTokens.SidebarManagement);
                    }
                }
            }));

        protected override async Task Refresh(CancellationToken token)
        {
            var drives = await CloudStorageService.GetDrives();
            Items.AddRange(drives);
            token.ThrowIfCancellationRequested();

            for (int i = 0; i < drives.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(drives[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            PathInfo.Place(AppPath.CloudDrives.Humanize(), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
