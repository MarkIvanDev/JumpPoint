using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Items.PortableStorage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using Nito.AsyncEx;
using NittyGritty.Services.Core;
using MainThread = Xamarin.Essentials.MainThread;

namespace JumpPoint.ViewModels
{
    public class DrivesViewModel : ShellContextViewModelBase
    {
        private readonly AsyncLock mutexDrives;

        public DrivesViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {
            mutexDrives = new AsyncLock();
        }

        public override bool HasCustomGrouping => true;

        protected override async Task Refresh(CancellationToken token)
        {
            using (await mutexDrives.LockAsync())
            {
                var drives = await StorageService.GetDrives();
                Items.AddRange(drives);
                token.ThrowIfCancellationRequested();

                for (int i = 0; i < drives.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    await JumpPointService.Load(drives[i]);
                    ProgressInfo.Update(Items.Count, i + 1, string.Empty);
                }
            }
        }

        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            PathInfo.Place(nameof(AppPath.Drives), parameter);
            await RefreshCommand.TryExecute();
            PortableStorageService.PortableDriveCollectionChanged += PortableStorageService_PortableDriveCollectionChanged;
        }

        private async void PortableStorageService_PortableDriveCollectionChanged(object sender, PortableDriveCollectionChangedEventArgs e)
        {
            using (await mutexDrives.LockAsync())
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    case NotifyCollectionChangedAction.Replace:
                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            var item = Items.OfType<PortableDrive>().FirstOrDefault(i => i.DeviceId == e.DeviceId);
                            await JumpPointService.Load(e.Drive);
                            if (item != null)
                            {
                                var index = Items.IndexOf(item);
                                Items[index] = e.Drive;
                            }
                            else
                            {
                                Items.Add(e.Drive);
                            }
                        });
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        await MainThread.InvokeOnMainThreadAsync(() =>
                        {
                            var item = Items.OfType<PortableDrive>().FirstOrDefault(i => i.DeviceId == e.DeviceId);
                            if (item != null)
                            {
                                Items.Remove(item);
                            }
                        });
                        break;

                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Reset:
                    default:
                        break;
                }
            }
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
            PortableStorageService.PortableDriveCollectionChanged -= PortableStorageService_PortableDriveCollectionChanged;
        }

    }
}
