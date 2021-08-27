using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Commands;
using JumpPoint.ViewModels.Dialogs;
using JumpPoint.ViewModels.Helpers;
using NittyGritty;
using NittyGritty.Commands;
using NittyGritty.Models;
using NittyGritty.Platform.Store;
using NittyGritty.Services;
using NittyGritty.Utilities;
using NittyGritty.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace JumpPoint.ViewModels
{
    public class TabbedShellViewModel : ViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly IAddOnService addOnService;
        private readonly IShareService shareService;
        private readonly CommandHelper commandHelper;

        public TabbedShellViewModel(IDialogService dialogService,
                                    IAddOnService addOnService,
                                    IShareService shareService,
                                    ShellItems shellItems,
                                    CommandHelper commandHelper)
        {
            this.dialogService = dialogService;
            this.addOnService = addOnService;
            this.shareService = shareService;
            ShellItems = shellItems;
            this.commandHelper = commandHelper;
            Tabs = new ObservableCollection<TabViewModel>();
        }

        public ObservableCollection<TabViewModel> Tabs { get; }

        private TabViewModel _CurrentTab;

        public TabViewModel CurrentTab
        {
            get { return _CurrentTab; }
            set
            {
                if (_CurrentTab != null) _CurrentTab.PropertyChanged -= CurrentTab_PropertyChanged;
                if (value != null) value.PropertyChanged += CurrentTab_PropertyChanged;
                Set(ref _CurrentTab, value);
                Messenger.Default.Send(new NotificationMessage(nameof(CurrentTab)), MessengerTokens.CommandManagement);
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
                await commandHelper.OpenPathInNewWindowCommand.TryExecute(tab);
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

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInNewTab;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInNewTabCommand => _OpenItemsInNewTab ?? (_OpenItemsInNewTab = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;
                var currentIndex = Tabs.IndexOf(CurrentTab);

                foreach (var item in context.SelectedItems)
                {
                    switch (item.Type)
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
                await Task.CompletedTask;
            },
            (context) => Toolbar.IsOpenInNewTabEnabled(context)));

        private void NewTab(int index, AppPath appPath, object parameter = null)
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
                    CurrentTab?.NavigationHelper.ToKey(item.Key, item.Parameter);
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
                    await dialogService.ShowMessage($"\'{PathQuery}\' is an invalid path", "Unknown Path", "OK");
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

        private ReadOnlyCollection<DurableAddOn> _durables;

        public ReadOnlyCollection<DurableAddOn> Durables
        {
            get { return _durables; }
            set { Set(ref _durables, value); }
        }

        private ReadOnlyCollection<SubscriptionAddOn> _subscriptions;

        public ReadOnlyCollection<SubscriptionAddOn> Subscriptions
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
                    Durables = await addOnService.GetDurableAddOns(
                        AddOnKeys.Durable1, AddOnKeys.Durable2, AddOnKeys.Durable3, AddOnKeys.Durable4, AddOnKeys.Durable5);
                    foreach (var item in Durables)
                    {
                        item.IsActive = await addOnService.IsActive(item);
                    }

                    Subscriptions = await addOnService.GetSubscriptionAddOns(
                        AddOnKeys.Monthly1, AddOnKeys.Monthly2, AddOnKeys.Monthly3, AddOnKeys.Monthly4, AddOnKeys.Monthly5);
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
                    await dialogService.ShowMessage("Your support for Jump Point is deeply appreciated.", "Thank you!");
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
                    await dialogService.ShowMessage("Your review is much appreciated.", "Thank you!");
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
            var type = EnumHelper<AppPath>.ParseOrDefault(query[nameof(PathInfo.Type)]);
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
            Messenger.Default.Send(new NotificationMessage(nameof(CurrentTab)), MessengerTokens.CommandManagement);

            // Hook Up Listeners
            PropertyChanged += TabbedShellViewModel_PropertyChanged;
            ShellItems.Start();
            shareService.Start();

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

            // Unhook Listeners
            PropertyChanged -= TabbedShellViewModel_PropertyChanged;
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
            switch (message.Notification)
            {
                case nameof(CurrentTab):
                case nameof(CurrentTab.Context):
                    commandHelper.NotifyNaviagtionBarCommands();
                    commandHelper.NotifyToolbarCommands();
                    break;

                case nameof(CurrentTab.Context.Item):
                    commandHelper.NotifyNaviagtionBarCommands();
                    //commandHelper.OpenPathInFileExplorerCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenPathInCommandPromptCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenPathInPowershellCommand.RaiseCanExecuteChanged();

                    //commandHelper.AddPathToWorkspaceCommand.RaiseCanExecuteChanged();
                    //commandHelper.AddPathToFavoritesCommand.RaiseCanExecuteChanged();
                    //commandHelper.RemovePathFromFavoritesCommand.RaiseCanExecuteChanged();

                    //commandHelper.SetPathWorkspaceTemplateCommand.RaiseCanExecuteChanged();
                    //commandHelper.SetPathFolderTemplateCommand.RaiseCanExecuteChanged();
                    break;

                case nameof(CurrentTab.Context.PathInfo):
                    // Navigation Bar
                    commandHelper.UpCommand.RaiseCanExecuteChanged();
                    commandHelper.PathPropertiesCommand.RaiseCanExecuteChanged();

                    // Toolbar
                    commandHelper.CopyCommand.RaiseCanExecuteChanged();
                    commandHelper.CutCommand.RaiseCanExecuteChanged();
                    commandHelper.RenameCommand.RaiseCanExecuteChanged();
                    commandHelper.DeleteCommand.RaiseCanExecuteChanged();
                    commandHelper.DeletePermanentlyCommand.RaiseCanExecuteChanged();
                    break;

                case nameof(CurrentTab.Context.SelectedItems):
                    // Toolbar
                    OpenItemsInNewTabCommand.RaiseCanExecuteChanged();
                    commandHelper.NotifyToolbarCommands();
                    //commandHelper.CopyCommand.RaiseCanExecuteChanged();
                    //commandHelper.CutCommand.RaiseCanExecuteChanged();
                    //commandHelper.RenameCommand.RaiseCanExecuteChanged();
                    //commandHelper.DeleteCommand.RaiseCanExecuteChanged();
                    //commandHelper.DeletePermanentlyCommand.RaiseCanExecuteChanged();

                    //commandHelper.OpenCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenWithCommand.RaiseCanExecuteChanged();
                    //OpenItemsInNewTabCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenItemsInNewWindowCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenItemsInFileExplorerCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenItemsInCommandPromptCommand.RaiseCanExecuteChanged();
                    //commandHelper.OpenItemsInPowershellCommand.RaiseCanExecuteChanged();
                    //commandHelper.ShareItemsCommand.RaiseCanExecuteChanged();
                    //commandHelper.ItemsPropertiesCommand.RaiseCanExecuteChanged();

                    //commandHelper.AddItemsToWorkspaceCommand.RaiseCanExecuteChanged();
                    //commandHelper.AddItemsToFavoritesCommand.RaiseCanExecuteChanged();
                    //commandHelper.RemoveItemsFromFavoritesCommand.RaiseCanExecuteChanged();
                    //commandHelper.SetItemsWorkspaceTemplateCommand.RaiseCanExecuteChanged();
                    //commandHelper.SetItemsFolderTemplateCommand.RaiseCanExecuteChanged();
                    break;

                case nameof(CommandHelper.ClipboardHasFiles):
                    commandHelper.PasteCommand.RaiseCanExecuteChanged();
                    break;

                case nameof(JumpPointItem.IsFavorite):
                    commandHelper.AddItemsToFavoritesCommand.RaiseCanExecuteChanged();
                    commandHelper.RemoveItemsFromFavoritesCommand.RaiseCanExecuteChanged();
                    break;
            }
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
        public NewTabParameter(AppPath appPath, object parameter = null)
        {
            AppPath = appPath;
            Parameter = parameter;
        }

        public AppPath AppPath { get; }

        public object Parameter { get; }
    }

}
