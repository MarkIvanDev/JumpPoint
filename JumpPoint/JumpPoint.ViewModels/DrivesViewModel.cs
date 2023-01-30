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
        private readonly SemaphoreSlim itemsSemaphore;

        public DrivesViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {
            itemsSemaphore = new SemaphoreSlim(1, 1);
        }

        public override bool HasCustomGrouping => true;

        protected override async Task Refresh(CancellationToken token)
        {
            await itemsSemaphore.WaitAsync();
            try
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
            finally
            {
                itemsSemaphore.Release();
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
            await Run(async (token) =>
            {
                await itemsSemaphore.WaitAsync();
                try
                {
                    var item = Items.OfType<PortableDrive>().FirstOrDefault(i => i.DeviceId == e.DeviceId);
                    token.ThrowIfCancellationRequested();
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                        case NotifyCollectionChangedAction.Replace:
                            var drive = await PortableStorageService.GetDriveFromId(e.DeviceId);
                            token.ThrowIfCancellationRequested();
                            if (drive != null)
                            {
                                await JumpPointService.Load(drive);
                                token.ThrowIfCancellationRequested();
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    if (item != null)
                                    {
                                        var index = Items.IndexOf(item);
                                        Items[index] = drive;
                                    }
                                    else
                                    {
                                        Items.Add(drive);
                                    }
                                });
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            await MainThread.InvokeOnMainThreadAsync(() =>
                            {
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
                catch { }
                finally
                {
                    itemsSemaphore.Release();
                }
            });
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
            PortableStorageService.PortableDriveCollectionChanged -= PortableStorageService_PortableDriveCollectionChanged;
        }

    }
}
