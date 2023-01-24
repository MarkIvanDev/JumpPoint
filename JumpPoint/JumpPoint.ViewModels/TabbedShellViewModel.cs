using GalaSoft.MvvmLight.Messaging;
using Humanizer;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Commands;
using JumpPoint.ViewModels.Dialogs;
using JumpPoint.ViewModels.Helpers;
using NittyGritty;
using NittyGritty.Commands;
using NittyGritty.Extensions;
using NittyGritty.Models;
using NittyGritty.Platform.Store;
using NittyGritty.Services.Core;
using NittyGritty.Utilities;
using NittyGritty.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace JumpPoint.ViewModels
{
    public class TabbedShellViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly IAddOnService addOnService;
        private readonly IShareService shareService;
        private readonly AppSettings appSettings;

        public TabbedShellViewModel(IDialogService dialogService,
                                    IAddOnService addOnService,
                                    IShareService shareService,
                                    AppSettings appSettings,
                                    ShellItems shellItems,
                                    CommandHelper commandHelper,
                                    BreadcrumbChildrenViewModel breadcrumbChildren)
        {
            this.dialogService = dialogService;
            this.addOnService = addOnService;
            this.shareService = shareService;
            this.appSettings = appSettings;
            ShellItems = shellItems;
            CommandHelper = commandHelper;
            BreadcrumbChildren = breadcrumbChildren;
            Tabs = new ObservableCollection<TabViewModel>();
        }

        public CommandHelper CommandHelper { get; }

        public BreadcrumbChildrenViewModel BreadcrumbChildren { get; }

        public ObservableCollection<TabViewModel> Tabs { get; }

        private TabViewModel _CurrentTab;

        public TabViewModel CurrentTab
        {
            get { return _CurrentTab; }
            set
            {
                //if (_CurrentTab != null) _CurrentTab.PropertyChanged -= CurrentTab_PropertyChanged;
                //if (value != null) value.PropertyChanged += CurrentTab_PropertyChanged;
                Set(ref _CurrentTab, value);
                //Messenger.Default.Send(new NotificationMessage(nameof(CurrentTab)), MessengerTokens.CommandManagement);
            }
        }

        private RelayCommand<TabViewModel> _NewTab;
        public RelayCommand<TabViewModel> NewTabCommand => _NewTab ?? (_NewTab = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                NewTab(tab is null ? -1 : Tabs.IndexOf(tab) + 1, AppPath.Dashboard);
            }));

        private RelayCommand<TabViewModel> _DuplicateTab;
        public RelayCommand<TabViewModel> DuplicateTabCommand => _DuplicateTab ?? (_DuplicateTab = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                if (tab?.Context is null) return;

                if (tab.Context.PathInfo.Parameter is TabParameter tabParameter)
                {
                    NewTab(Tabs.IndexOf(tab) + 1, tab.Context.PathInfo.Type, tabParameter.Parameter);
                }
            }));

        private AsyncRelayCommand<TabViewModel> _MoveTabToNewWindow;
        public AsyncRelayCommand<TabViewModel> MoveTabToNewWindowCommand => _MoveTabToNewWindow ?? (_MoveTabToNewWindow = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                if (tab?.Context is null) return;

                CloseTabCommand.TryExecute(tab);
                await CommandHelper.OpenPathInNewWindowCommand.TryExecute(tab.Context);
            }));

        private RelayCommand<TabViewModel> _CloseOtherTabs;
        public RelayCommand<TabViewModel> CloseOtherTabsCommand => _CloseOtherTabs ?? (_CloseOtherTabs = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                if (tab is null) return;

                var currentIndex = Tabs.IndexOf(tab);
                if (currentIndex == -1) return;

                for (int i = Tabs.Count - 1; i >= 0; i--)
                {
                    if (i != currentIndex)
                    {
                        var key = Tabs[i].Key;
                        Tabs.RemoveAt(i);
                        ViewModelLocator.Instance.DisposeTab(key);
                    }
                }
            }));

        private AsyncRelayCommand<ShellItem> _OpenShellItemInNewTab;
        public AsyncRelayCommand<ShellItem> OpenShellItemInNewTabCommand => _OpenShellItemInNewTab ?? (_OpenShellItemInNewTab = new AsyncRelayCommand<ShellItem>(
            async (item) =>
            {
                if (item?.Content is null) return;
                var currentIndex = Tabs.IndexOf(CurrentTab);
                if (item.Content is JumpPointItem jpItem)
                {
                    OpenItemInNewTab(currentIndex, jpItem);
                }
                else if (item.Content is AppPath appPath)
                {
                    NewTab(currentIndex != -1 ? currentIndex += 1 : currentIndex, appPath, TabbedNavigationHelper.GetParameter(appPath, appPath.Humanize(), null));
                }
                await Task.CompletedTask;
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenPathInNewTab;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenPathInNewTabCommand => _OpenPathInNewTab ?? (_OpenPathInNewTab = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.PathInfo is null) return;
                var currentIndex = Tabs.IndexOf(CurrentTab);
                NewTab(currentIndex != -1 ? currentIndex += 1 : currentIndex, context.PathInfo.Type, context.PathInfo.Parameter.Parameter);
                await Task.CompletedTask;
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInNewTab;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInNewTabCommand => _OpenItemsInNewTab ?? (_OpenItemsInNewTab = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;
                var currentIndex = Tabs.IndexOf(CurrentTab);

                foreach (var item in context.SelectedItems)
                {
                    OpenItemInNewTab(currentIndex, item);
                }
                await Task.CompletedTask;
            }));

        private void OpenItemInNewTab(int currentIndex, JumpPointItem item)
        {
            switch (item?.Type)
            {
                case JumpPointItemType.Folder:
                    NewTab(currentIndex != -1 ? currentIndex += 1 : currentIndex, AppPath.Folder, TabbedNavigationHelper.GetParameter(item));
                    break;

                case JumpPointItemType.Drive:
                    NewTab(currentIndex != -1 ? currentIndex += 1 : currentIndex, AppPath.Drive, TabbedNavigationHelper.GetParameter(item));
                    break;

                case JumpPointItemType.Workspace:
                    NewTab(currentIndex != -1 ? currentIndex += 1 : currentIndex, AppPath.Workspace, TabbedNavigationHelper.GetParameter(item));
                    break;

                case JumpPointItemType.Unknown:
                case JumpPointItemType.File:
                case JumpPointItemType.AppLink:
                case JumpPointItemType.Library:
                default:
                    break;
            }
        }

        private void NewTab(int index, AppPath appPath, string parameter = null)
        {
            var newTab = ViewModelLocator.Instance.GetNewTab();
            newTab.InitialParameter = new NewTabParameter(appPath, parameter);
            if (index == -1 || index > Tabs.Count)
            {
                Tabs.Add(newTab);
            }
            else
            {
                Tabs.Insert(index, newTab);
            }
            CurrentTab = newTab;
        }

        private RelayCommand<TabViewModel> _CloseTab;
        public RelayCommand<TabViewModel> CloseTabCommand => _CloseTab ?? (_CloseTab = new RelayCommand<TabViewModel>(
            tab =>
            {
                Tabs.Remove(tab);
                ViewModelLocator.Instance.DisposeTab(tab.Key);
            }));

        #region Pane

        public ShellItems ShellItems { get; }

        private AsyncRelayCommand<ShellItem> _ShellItem;
        public AsyncRelayCommand<ShellItem> ShellItemCommand => _ShellItem ?? (_ShellItem = new AsyncRelayCommand<ShellItem>(
            async item =>
            {
                if (item.Key == ViewModelKeys.Chatbot)
                {
                    await JumpPointService.OpenNewWindow(AppPath.Chat, null);
                }
                else if (item.Key == ViewModelKeys.ClipboardManager)
                {
                    await JumpPointService.OpenNewWindow(AppPath.ClipboardManager, null);
                }
                else if (!(item.Key is null))
                {
                    CurrentTab?.NavigationHelper.ToKey(item.Key, item.Parameter?.ToString());
                }
            }));

        #endregion

        #region Breadcrumb Bar

        private AsyncRelayCommand<Breadcrumb> _Breadcrumb;
        public AsyncRelayCommand<Breadcrumb> BreadcrumbCommand => _Breadcrumb ?? (_Breadcrumb = new AsyncRelayCommand<Breadcrumb>(
            async breadcrumb =>
            {
                await Task.CompletedTask;
                CurrentTab?.NavigationHelper.ToBreadcrumb(breadcrumb, CurrentTab?.Context?.Item);
            }));

        private bool _isEditingPath;

        public bool IsEditingPath
        {
            get { return _isEditingPath; }
            set { Set(ref _isEditingPath, value); }
        }

        private string _pathQuery;

        public string PathQuery
        {
            get { return _pathQuery ?? (_pathQuery = string.Empty); }
            set { Set(ref _pathQuery, value); }
        }

        private RelayCommand _EditPath;
        public RelayCommand EditPathCommand => _EditPath ?? (_EditPath = new RelayCommand(
            () =>
            {
                IsEditingPath = true;
                PathQuery = CurrentTab?.Context.PathInfo.Path;
            }));

        private RelayCommand _RollbackPath;
        public RelayCommand RollbackPathCommand => _RollbackPath ?? (_RollbackPath = new RelayCommand(
            () =>
            {
                IsEditingPath = false;
                PathQuery = null;
            }));

        private AsyncRelayCommand _CommitPath;
        public AsyncRelayCommand CommitPathCommand => _CommitPath ?? (_CommitPath = new AsyncRelayCommand(
            async () =>
            {
                var crumbs = PathQuery.GetBreadcrumbs();
                var lastCrumb = crumbs.LastOrDefault();
                if (lastCrumb is null && !string.IsNullOrWhiteSpace(PathQuery))
                {
                    await dialogService.ShowMessage($"\'{PathQuery}\' is an invalid path", "Unknown Path", "OK", appSettings.Theme);
                }
                else if (lastCrumb != null && !lastCrumb.Path.Equals(CurrentTab.Context.PathInfo.Path, StringComparison.OrdinalIgnoreCase))
                {
                    CurrentTab.NavigationHelper.ToBreadcrumb(lastCrumb, null);
                }
                IsEditingPath = false;
                PathQuery = null;
            }));

        #endregion

        #region Support Developer

        private IList<DurableAddOn> _durables;

        public IList<DurableAddOn> Durables
        {
            get { return _durables; }
            set { Set(ref _durables, value); }
        }

        private IList<SubscriptionAddOn> _subscriptions;

        public IList<SubscriptionAddOn> Subscriptions
        {
            get { return _subscriptions; }
            set { Set(ref _subscriptions, value); }
        }

        private AsyncRelayCommand _loadAddOnsCommand;

        public AsyncRelayCommand LoadAddOnsCommand => _loadAddOnsCommand ?? (_loadAddOnsCommand = new AsyncRelayCommand(
            async () =>
            {
                await Run(async (token) =>
                {
                    Durables = await addOnService.GetDurableAddOns(new List<string> {
                        AddOnKeys.Durable1, AddOnKeys.Durable2, AddOnKeys.Durable3, AddOnKeys.Durable4, AddOnKeys.Durable5 });
                    foreach (var item in Durables)
                    {
                        item.IsActive = await addOnService.IsActive(item);
                    }

                    Subscriptions = await addOnService.GetSubscriptionAddOns(new List<string> {
                        AddOnKeys.Monthly1, AddOnKeys.Monthly2, AddOnKeys.Monthly3, AddOnKeys.Monthly4, AddOnKeys.Monthly5 });
                    foreach (var item in Subscriptions)
                    {
                        item.IsActive = await addOnService.IsActive(item);
                    }
                });
            }));

        private AsyncRelayCommand<IActiveAddOn> _PurchaseAddOn;
        public AsyncRelayCommand<IActiveAddOn> PurchaseAddOnCommand => _PurchaseAddOn ?? (_PurchaseAddOn = new AsyncRelayCommand<IActiveAddOn>(
            async item =>
            {
                if (item.IsActive)
                {
                    await dialogService.ShowMessage("Your support for Jump Point is deeply appreciated.", "Thank you!", appSettings.Theme);
                }
                else
                {
                    if (await addOnService.TryPurchase((AddOn)item))
                    {
                        item.IsActive = await addOnService.IsActive(item);
                    }
                }
            }));

        private AsyncRelayCommand _Rate;
        public AsyncRelayCommand RateCommand => _Rate ?? (_Rate = new AsyncRelayCommand(
            async () =>
            {
                //await Browser.OpenAsync("ms-windows-store://review/?ProductId=9PL7B4PZ0HS0");
                var result = await JumpPointService.Rate();
                if (result)
                {
                    await dialogService.ShowMessage("Your review is much appreciated.", "Thank you!", appSettings.Theme);
                }
            }));

        private AsyncRelayCommand _GithubIssue;
        public AsyncRelayCommand GithubIssueCommand => _GithubIssue ?? (_GithubIssue = new AsyncRelayCommand(
            async () =>
            {
                await Browser.OpenAsync("https://github.com/MarkIvanDev/JumpPoint/issues");
            }));

        private AsyncRelayCommand _ManageSubscriptions;
        public AsyncRelayCommand ManageSubscriptionsCommand => _ManageSubscriptions ?? (_ManageSubscriptions = new AsyncRelayCommand(
            async () =>
            {
                await addOnService.ManageSubscriptions();
            }));

        #endregion

        public void ProcessParameter(string parameter)
        {
            var query = QueryString.Parse(parameter);
            var type = EnumHelper<AppPath>.ParseOrDefault(query[nameof(PathInfo.Type)], ignoreCase: true);
            if (type != AppPath.Unknown)
            {
                NewTab(-1, type, parameter);
            }
            else
            {
                NewTab(-1, AppPath.Dashboard, parameter);
            }
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            ChangeNotifierService.Connect();
            Messenger.Default.Register<NotificationMessage>(this, MessengerTokens.CommandManagement, ManageCommands);

            if (parameter != null)
            {
                ProcessParameter(parameter.ToString());
            }
            else if (state != null)
            {
                ProcessParameter(RestoreStateItem<string>(state, nameof(PathInfo)));
            }
            else
            {
                ProcessParameter(new QueryString()
                {
                    { nameof(PathInfo.Type), AppPath.Dashboard.ToString() },
                    { nameof(PathInfo.Path), nameof(AppPath.Dashboard) }
                }.ToString());
            }
            await ShellItems.Refresh();
            //Messenger.Default.Send(new NotificationMessage(nameof(CurrentTab)), MessengerTokens.CommandManagement);

            // Hook Up Listeners
            ShellItems.Start();
            shareService.Start();
            await DesktopService.ChangeNotifier();

            await LoadAddOnsCommand.TryExecute();
        }

        private void TabbedShellViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentTab))
            {
                Messenger.Default.Send(new NotificationMessage(nameof(CurrentTab)), MessengerTokens.CommandManagement);
            }
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            ChangeNotifierService.Disconnect();
            Messenger.Default.Unregister<NotificationMessage>(this, MessengerTokens.CommandManagement, ManageCommands);
            CancelAll();

            if (CurrentTab?.Context != null)
            {
                state[nameof(PathInfo)] = new QueryString()
                {
                    { nameof(PathInfo.Type), CurrentTab.Context.PathInfo.Type.ToString() },
                    { nameof(PathInfo.Path), CurrentTab.Context.PathInfo.Path }
                }.ToString();
            }

            // Unhook listeners
            ShellItems.Stop();
            shareService.Stop();
        }

        private void CurrentTab_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CurrentTab.Context))
            {
                Messenger.Default.Send(new NotificationMessage(nameof(CurrentTab.Context)), MessengerTokens.CommandManagement);
            }
        }

        private void ManageCommands(NotificationMessage message)
        {
            CurrentTab?.RaisePropertyChanged(nameof(CurrentTab.Context));
        }
    }

    public class TabViewModel : ObservableObject
    {
        public TabViewModel(string key, INavigationService navigationService)
        {
            Key = key;
            NavigationHelper = new TabbedNavigationHelper(key, navigationService);
        }

        public string Key { get; }
        
        public TabbedNavigationHelper NavigationHelper { get; }

        private NewTabParameter _initialParameter;

        public NewTabParameter InitialParameter
        {
            get { return _initialParameter; }
            set { Set(ref _initialParameter, value); }
        }

        private ShellContextViewModelBase _context;

        public ShellContextViewModelBase Context
        {
            get { return _context; }
            set { Set(ref _context, value); }
        }

    }

    public class NewTabParameter
    {
        public NewTabParameter(AppPath appPath, string parameter = null)
        {
            AppPath = appPath;
            Parameter = parameter;
        }

        public AppPath AppPath { get; }

        public string Parameter { get; }
    }

    public class BreadcrumbChildrenViewModel : ObservableObject
    {
        private readonly SemaphoreSlim semaphore;
        private readonly AppSettings appSettings;
        private CancellationTokenSource tokenSource;

        public BreadcrumbChildrenViewModel(AppSettings appSettings)
        {
            semaphore = new SemaphoreSlim(1, 1);
            tokenSource = new CancellationTokenSource();
            Children = new ObservableCollection<JumpPointItem>();
            this.appSettings = appSettings;
        }

        public ObservableCollection<JumpPointItem> Children { get; }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private AsyncRelayCommand<Breadcrumb> _Load;
        public AsyncRelayCommand<Breadcrumb> LoadCommand => _Load ?? (_Load = new AsyncRelayCommand<Breadcrumb>(
            async (crumb) =>
            {
                IsLoading = true;
                await semaphore.WaitAsync();
                try
                {
                    Children.Clear();
                    CancelCommand.TryExecute();
                    tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    switch (crumb.AppPath)
                    {
                        case AppPath.Drive:
                        case AppPath.Folder:
                            var directory = await StorageService.GetDirectory(crumb.Path);
                            if (directory != null)
                            {
                                var items = (await StorageService.GetItems(directory)).Where(i =>
                                {
                                    if (i is DirectoryBase)
                                    {
                                        if (!appSettings.ShowHiddenItems && i is StorageItemBase item && item.Attributes.HasValue)
                                        {
                                            return (item.Attributes.Value & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden;
                                        }
                                        else
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }).ToList();
                                Children.AddRange(items);
                                foreach (var item in items)
                                {
                                    token.ThrowIfCancellationRequested();
                                    await JumpPointService.Load(item);
                                }
                            }
                            break;

                        case AppPath.Dashboard:
                            var userFolders = await DashboardService.GetUserFolders(false);
                            Children.AddRange(userFolders);
                            var systemFolders = await DashboardService.GetSystemFolders(false);
                            Children.AddRange(systemFolders);
                            foreach (var item in userFolders)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            foreach (var item in systemFolders)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            break;

                        case AppPath.Favorites:
                            var favorites = (await DashboardService.GetFavorites()).Where(i => i is DirectoryBase || i is Workspace);
                            Children.AddRange(favorites);
                            foreach (var item in favorites)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            break;

                        case AppPath.Drives:
                            var drives = await StorageService.GetDrives();
                            Children.AddRange(drives);
                            foreach (var item in drives)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            break;

                        case AppPath.CloudDrives:
                            var cloudDrives = await CloudStorageService.GetDrives();
                            Children.AddRange(cloudDrives);
                            foreach (var item in cloudDrives)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            break;

                        case AppPath.Workspaces:
                            var workspaces = await WorkspaceService.GetWorkspaces();
                            Children.AddRange(workspaces);
                            foreach (var item in workspaces)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            break;

                        case AppPath.Workspace:
                            var workspace = await WorkspaceService.GetWorkspace(crumb.Path);
                            if (workspace != null)
                            {
                                var items = (await WorkspaceService.GetItems(workspace.Id)).Where(i => i is DirectoryBase);
                                Children.AddRange(items);
                                foreach (var item in items)
                                {
                                    token.ThrowIfCancellationRequested();
                                    await JumpPointService.Load(item);
                                }
                            }
                            break;

                        case AppPath.Cloud:
                            var provider = CloudStorageService.GetProvider(crumb.Path);
                            if (provider != CloudStorageProvider.Unknown)
                            {
                                var clouds = await CloudStorageService.GetDrives(provider);
                                Children.AddRange(clouds);
                                foreach (var item in clouds)
                                {
                                    token.ThrowIfCancellationRequested();
                                    await JumpPointService.Load(item);
                                }
                            }
                            break;

                        case AppPath.WSL:
                            var wslDrives = await WslStorageService.GetDrives();
                            Children.AddRange(wslDrives);
                            foreach (var item in wslDrives)
                            {
                                token.ThrowIfCancellationRequested();
                                await JumpPointService.Load(item);
                            }
                            break;

                        case AppPath.AppLinks:
                        case AppPath.Settings:
                        case AppPath.Properties:
                        case AppPath.Chat:
                        case AppPath.ClipboardManager:
                        case AppPath.Unknown:
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Breadcrumb children loading error: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                    IsLoading = false;
                }
            }));

        private RelayCommand _Cancel;
        public RelayCommand CancelCommand => _Cancel ?? (_Cancel = new RelayCommand(
            () =>
            {
                try
                {
                    tokenSource?.Cancel();
                    tokenSource?.Dispose();
                }
                catch (Exception)
                {
                }
            }));

    }

}
