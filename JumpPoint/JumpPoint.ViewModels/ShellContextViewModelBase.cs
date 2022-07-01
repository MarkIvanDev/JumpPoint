using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Commands;
using NittyGritty.Models;
using NittyGritty.Services.Core;
using NittyGritty.ViewModels;

namespace JumpPoint.ViewModels
{
    public abstract class ShellContextViewModelBase : ViewModelBase
    {
        private readonly SemaphoreSlim refreshSemaphore;
        private readonly IShortcutService shortcutService;
        private readonly AppSettings appSettings;

        public ShellContextViewModelBase(IShortcutService shortcutService, AppSettings appSettings)
        {
            refreshSemaphore = new SemaphoreSlim(1, 1);
            this.shortcutService = shortcutService;
            this.appSettings = appSettings;
            HasCustomGrouping = false;
            ProgressInfo = new ProgressInfo();
            IsPinned = false;
            PathInfo = new PathInfo();
            ItemStats = new ItemStats();
            SelectedItemStats = new ItemStats();
            Items = new ObservableRangeCollection<JumpPointItem>();
            SelectedItems = new ObservableCollection<JumpPointItem>();
        }

        public virtual bool HasCustomGrouping { get; }

        public ProgressInfo ProgressInfo { get; }

        public PathInfo PathInfo { get; }

        private string _pathHash;

        public string PathHash
        {
            get { return _pathHash; }
            set { Set(ref _pathHash, value); }
        }

        private bool _isPinned;

        public bool IsPinned
        {
            get { return _isPinned; }
            set { Set(ref _isPinned, value); }
        }

        private string _tabKey;

        public string TabKey
        {
            get { return _tabKey; }
            set { Set(ref _tabKey, value); }
        }

        public ItemStats ItemStats { get; }

        public ItemStats SelectedItemStats { get; }

        public ObservableRangeCollection<JumpPointItem> Items { get; }

        public ObservableCollection<JumpPointItem> SelectedItems { get; }

        private JumpPointItem _item;

        public JumpPointItem Item
        {
            get { return _item; }
            set
            {
                if (_item != null) _item.PropertyChanged -= Item_PropertyChanged;
                if (value != null) value.PropertyChanged += Item_PropertyChanged;
                Set(ref _item, value);
                Messenger.Default.Send(new NotificationMessage(nameof(Item)), MessengerTokens.CommandManagement);
            }
        }

        private AsyncRelayCommand _Refresh;
        public AsyncRelayCommand RefreshCommand => _Refresh ?? (_Refresh = new AsyncRelayCommand(
            async () =>
            {
                await Run(async (token) =>
                {
                    await refreshSemaphore.WaitAsync();
                    try
                    {
                        ProgressInfo.Start();
                        ItemStats.Reset();

                        PathHash = HashTool.Sha256Hash(PathInfo.Path.ToUpperInvariant());
                        IsPinned = shortcutService.Exists(PathHash);

                        Filter();

                        await Refresh(token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
                    }
                    finally
                    {
                        ProgressInfo.Stop();
                        refreshSemaphore.Release();
                    }
                });
            }));

        protected abstract Task Refresh(CancellationToken token);

        public override void LoadState(object parameter, Dictionary<string, object> state)
        {
            if (parameter is TabParameter tabParameter)
            {
                TabKey = tabParameter.TabKey;
            }
            PathInfo.PropertyChanged += PathInfo_PropertyChanged;
            Items.CollectionChanged += Items_CollectionChanged;
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            appSettings.PropertyChanged += AppSettings_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(nameof(Item)), MessengerTokens.CommandManagement);
        }

        private void PathInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage(nameof(PathInfo)), MessengerTokens.CommandManagement);
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ItemStats.Update(
                drives: Items.Count(i => i.Type == JumpPointItemType.Drive),
                folders: Items.Count(i => i.Type == JumpPointItemType.Folder),
                files: Items.Count(i => i.Type == JumpPointItemType.File),
                workspaces: Items.Count(i => i.Type == JumpPointItemType.Workspace),
                appLinks: Items.Count(i => i.Type == JumpPointItemType.AppLink)
            );
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedItemStats.Update(
                drives: SelectedItems.Count(i => i.Type == JumpPointItemType.Drive),
                folders: SelectedItems.Count(i => i.Type == JumpPointItemType.Folder),
                files: SelectedItems.Count(i => i.Type == JumpPointItemType.File),
                workspaces: SelectedItems.Count(i => i.Type == JumpPointItemType.Workspace),
                appLinks: SelectedItems.Count(i => i.Type == JumpPointItemType.AppLink)
            );
            Messenger.Default.Send(new NotificationMessage(nameof(SelectedItems)), MessengerTokens.CommandManagement);
        }

        private void AppSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.ShowHiddenItems))
            {
                Filter();
            }
        }

        private void Filter()
        {
            
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            CancelAll();
            Item = null;
            Items.Clear();
            SelectedItems.Clear();
            PathInfo.PropertyChanged -= PathInfo_PropertyChanged;
            Items.CollectionChanged -= Items_CollectionChanged;
            SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            appSettings.PropertyChanged -= AppSettings_PropertyChanged;
        }
    }
}
