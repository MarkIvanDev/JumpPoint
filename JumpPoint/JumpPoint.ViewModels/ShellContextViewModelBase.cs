using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty;
using NittyGritty.Collections;
using NittyGritty.Commands;
using NittyGritty.Models;
using NittyGritty.Services.Core;
using NittyGritty.ViewModels;
using Xamarin.Essentials;

namespace JumpPoint.ViewModels
{
    public abstract class ShellContextViewModelBase : ViewModelBase
    {
        private readonly SemaphoreSlim initializeSemaphore;
        private readonly SemaphoreSlim refreshSemaphore;
        private readonly object filterLock;
        private readonly IShortcutService shortcutService;
        private readonly AppSettings appSettings;

        public ShellContextViewModelBase(IShortcutService shortcutService, AppSettings appSettings)
        {
            initializeSemaphore = new SemaphoreSlim(1, 1);
            refreshSemaphore = new SemaphoreSlim(1, 1);
            filterLock = new object();
            this.shortcutService = shortcutService;
            this.appSettings = appSettings;
            HasCustomGrouping = false;
            ProgressInfo = new ProgressInfo();
            IsPinned = false;
            PathInfo = new PathInfo();
            Items = new DynamicCollection<JumpPointItem>();
            SelectedItems = new ObservableCollection<JumpPointItem>();
            ItemStats = new ItemStats(Items);
            SelectedItemStats = new ItemStats(SelectedItems);
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

        private PathSettings _settings;

        public PathSettings Settings
        {
            get { return _settings; }
            set { Set(ref _settings, value); }
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

        private bool _isSearching;

        public bool IsSearching
        {
            get { return _isSearching; }
            set
            {
                Set(ref _isSearching, value);
                if (!IsSearching)
                {
                    Search = null;
                }
            }
        }

        private string _search;

        public string Search
        {
            get { return _search ?? (_search = string.Empty); }
            set
            {
                if (Set(ref _search, value))
                {
                    Filter();
                }
            }
        }

        public ItemStats ItemStats { get; }

        public ItemStats SelectedItemStats { get; }

        public DynamicCollection<JumpPointItem> Items { get; }

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
                        Items.Clear();
                        ProgressInfo.Start();

                        PathHash = HashTool.Sha256Hash(PathInfo.Path.ToUpperInvariant());
                        Settings = new PathSettings(PathHash);
                        IsPinned = shortcutService.Exists(PathHash);

                        Filter();

                        await Refresh(token);
                    }
                    catch (OperationCanceledException)
                    {
                        Items.Clear();
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

        public sealed override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            await initializeSemaphore.WaitAsync();
            try
            {
                if (parameter is TabParameter tabParameter)
                {
                    TabKey = tabParameter.TabKey;
                }
                PathInfo.PropertyChanged += PathInfo_PropertyChanged;
                Items.CollectionChanged += Items_CollectionChanged;
                SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
                appSettings.PropertyChanged += AppSettings_PropertyChanged;

                await Initialize(parameter, state);
            }
            catch (Exception ex)
            {
                Messenger.Default.Send(new NotificationMessage<Exception>(ex, ex.Message), MessengerTokens.ExceptionManagement);
            }
            finally
            {
                initializeSemaphore.Release();
            }
        }

        protected abstract Task Initialize(object parameter, Dictionary<string, object> state);

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
            ItemStats.Refresh();
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SelectedItemStats.Refresh();
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
            lock (filterLock)
            {
                Items.Filter = i =>
                {
                    var include = true;

                    if (!appSettings.ShowHiddenItems && i is StorageItemBase item && item.Attributes.HasValue)
                    {
                        include &= (item.Attributes.Value & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden;
                    }

                    if (!string.IsNullOrWhiteSpace(Search))
                    {
                        include &= i.Name.IndexOf(Search.Trim(), StringComparison.OrdinalIgnoreCase) >= 0;
                    }

                    return include;
                };
            }
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            CancelAll();
            Item = null;
            SelectedItems.Clear();
            IsSearching = false;
            PathInfo.PropertyChanged -= PathInfo_PropertyChanged;
            Items.CollectionChanged -= Items_CollectionChanged;
            SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            appSettings.PropertyChanged -= AppSettings_PropertyChanged;
        }

        public void CloseSearch() => IsSearching = false;
    }

    public class PathSettings : ObservableObject
    {
        private const string DRIVES_HASH = "77e9d94720614a9d8dbc5450dda97fd344f15bec5c7a786986fc4984eb4a9473";
        private const string CLOUDDRIVES_HASH = "3ed8ca857698335e4f5ff193a2afed9130b68478aceeb809748e124f227fcc48";

        private readonly string pathHash;

        public PathSettings(string pathHash)
        {
            this.pathHash = pathHash;
        }

        #region Layout

        public string Layout
        {
            get { return Preferences.Get(nameof(Layout), GetDefaultLayout(pathHash), pathHash); }
            set
            {
                if (Layout != value)
                {
                    Preferences.Set(nameof(Layout), value, pathHash);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGridLayout));
                    RaisePropertyChanged(nameof(IsDetailsLayout));
                    RaisePropertyChanged(nameof(IsTilesLayout));
                    RaisePropertyChanged(nameof(IsListLayout));
                }
            }
        }

        private static string GetDefaultLayout(string pathHash)
        {
            if (pathHash == DRIVES_HASH || pathHash == CLOUDDRIVES_HASH)
            {
                return LayoutModes.Tiles;
            }
            else
            {
                return LayoutModes.Grid;
            }
        }

        public bool IsGridLayout
        {
            get { return Layout == LayoutModes.Grid; }
            set
            {
                if (Layout != LayoutModes.Grid && value)
                {
                    Layout = LayoutModes.Grid;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsDetailsLayout
        {
            get { return Layout == LayoutModes.Details; }
            set
            {
                if (Layout != LayoutModes.Details && value)
                {
                    Layout = LayoutModes.Details;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsTilesLayout
        {
            get { return Layout == LayoutModes.Tiles; }
            set
            {
                if (Layout != LayoutModes.Tiles && value)
                {
                    Layout = LayoutModes.Tiles;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsListLayout
        {
            get { return Layout == LayoutModes.List; }
            set
            {
                if (Layout != LayoutModes.List && value)
                {
                    Layout = LayoutModes.List;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region SortBy

        public SortBy SortBy
        {
            get { return (SortBy)Preferences.Get(nameof(SortBy), (int)SortBy.Name, pathHash); }
            set
            {
                if (SortBy != value)
                {
                    Preferences.Set(nameof(SortBy), (int)value, pathHash);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSortByName));
                    RaisePropertyChanged(nameof(IsSortByDateModified));
                    RaisePropertyChanged(nameof(IsSortByDisplayType));
                    RaisePropertyChanged(nameof(IsSortBySize));
                }
            }
        }

        public bool IsSortByName
        {
            get { return SortBy == SortBy.Name; }
            set
            {
                if (SortBy != SortBy.Name && value)
                {
                    SortBy = SortBy.Name;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortByDateModified
        {
            get { return SortBy == SortBy.DateModified; }
            set
            {
                if (SortBy != SortBy.DateModified && value)
                {
                    SortBy = SortBy.DateModified;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortByDisplayType
        {
            get { return SortBy == SortBy.DisplayType; }
            set
            {
                if (SortBy != SortBy.DisplayType && value)
                {
                    SortBy = SortBy.DisplayType;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortBySize
        {
            get { return SortBy == SortBy.Size; }
            set
            {
                if (SortBy != SortBy.Size && value)
                {
                    SortBy = SortBy.Size;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSortAscending
        {
            get { return Preferences.Get(nameof(IsSortAscending), true, pathHash); }
            set
            {
                if (IsSortAscending != value)
                {
                    Preferences.Set(nameof(IsSortAscending), value, pathHash);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSortDescending));
                }
            }
        }

        public bool IsSortDescending
        {
            get { return !IsSortAscending; }
            set
            {
                if (IsSortDescending != value)
                {
                    IsSortAscending = !value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsSortAscending));
                }
            }
        }

        #endregion

        #region GroupBy

        public GroupBy GroupBy
        {
            get { return (GroupBy)Preferences.Get(nameof(GroupBy), (int)GroupBy.None, pathHash); }
            set
            {
                if (GroupBy != value)
                {
                    Preferences.Set(nameof(GroupBy), (int)value, pathHash);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGroupByNone));
                    RaisePropertyChanged(nameof(IsGroupByName));
                    RaisePropertyChanged(nameof(IsGroupByDateModified));
                    RaisePropertyChanged(nameof(IsGroupByDisplayType));
                    RaisePropertyChanged(nameof(IsGroupBySize));
                    RaisePropertyChanged(nameof(IsGroupByItemType));
                }
            }
        }

        public bool IsGroupByNone
        {
            get { return GroupBy == GroupBy.None; }
            set
            {
                if (GroupBy != GroupBy.None && value)
                {
                    GroupBy = GroupBy.None;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByName
        {
            get { return GroupBy == GroupBy.Name; }
            set
            {
                if (GroupBy != GroupBy.Name && value)
                {
                    GroupBy = GroupBy.Name;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByDateModified
        {
            get { return GroupBy == GroupBy.DateModified; }
            set
            {
                if (GroupBy != GroupBy.DateModified && value)
                {
                    GroupBy = GroupBy.DateModified;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByDisplayType
        {
            get { return GroupBy == GroupBy.DisplayType; }
            set
            {
                if (GroupBy != GroupBy.DisplayType && value)
                {
                    GroupBy = GroupBy.DisplayType;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupBySize
        {
            get { return GroupBy == GroupBy.Size; }
            set
            {
                if (GroupBy != GroupBy.Size && value)
                {
                    GroupBy = GroupBy.Size;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupByItemType
        {
            get { return GroupBy == GroupBy.ItemType; }
            set
            {
                if (GroupBy != GroupBy.ItemType && value)
                {
                    GroupBy = GroupBy.ItemType;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsGroupAscending
        {
            get { return Preferences.Get(nameof(IsGroupAscending), true, pathHash); }
            set
            {
                if (IsGroupAscending != value)
                {
                    Preferences.Set(nameof(IsGroupAscending), value, pathHash);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGroupDescending));
                }
            }
        }

        public bool IsGroupDescending
        {
            get { return !IsGroupAscending; }
            set
            {
                if (IsGroupDescending != value)
                {
                    IsGroupAscending = !value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(IsGroupAscending));
                }
            }
        }

        #endregion

    }
}
