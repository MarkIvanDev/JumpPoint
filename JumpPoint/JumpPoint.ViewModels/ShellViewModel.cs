using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using NittyGritty.Platform.Store;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Watchers;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Dialogs;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Commands;
using NittyGritty.Models;
using NittyGritty.Services;
using NittyGritty.ViewModels;
using JumpPoint.Platform;
using JumpPoint.Platform.Models.Extensions;

namespace JumpPoint.ViewModels
{
    public class ShellViewModel : ViewModelBase
    {
        private readonly static SemaphoreSlim refreshSemaphore = new SemaphoreSlim(1, 1);

        private readonly IDialogService dialogService;
        private readonly IClipboardService clipboardService;
        private readonly IShareService shareService;
        private readonly IAddOnService addOnService;
        private readonly IShortcutService shortcutService;
        private readonly ITileIconHelper iconHelper;
        private readonly CommandHelper commandHelper;

        public ShellViewModel(IDialogService dialogService,
                              IClipboardService clipboardService,
                              IShareService shareService,
                              IAddOnService addOnService,
                              IShortcutService shortcutService,
                              ITileIconHelper iconHelper,
                              ShellItems shellItems,
                              CommandHelper commandHelper)
        {
            this.dialogService = dialogService;
            this.clipboardService = clipboardService;
            this.shareService = shareService;
            this.addOnService = addOnService;
            this.shortcutService = shortcutService;
            this.iconHelper = iconHelper;
            ShellItems = shellItems;
            this.commandHelper = commandHelper;
        }

        public ShellItems ShellItems { get; }

        private AsyncRelayCommand<ShellItem> _ShellItem;
        public AsyncRelayCommand<ShellItem> ShellItemCommand => _ShellItem ?? (_ShellItem = new AsyncRelayCommand<ShellItem>(
            async item =>
            {
                if (item.Key == ViewModelKeys.Chatbot)
                {
                    await JumpPointService.OpenNewWindow(AppPath.Chat, null);
                }
                else if (!(item.Key is null))
                {
                    NavigationHelper.ToKey(item.Key, item.Parameter);
                }
            }));

        private ShellContextViewModelBase _context;

        public ShellContextViewModelBase Context
        {
            get { return _context; }
            set
            {
                Set(ref _context, value);
            }
        }

        #region Breadcrumb Bar

        private AsyncRelayCommand<Breadcrumb> _Breadcrumb;
        public AsyncRelayCommand<Breadcrumb> BreadcrumbCommand => _Breadcrumb ?? (_Breadcrumb = new AsyncRelayCommand<Breadcrumb>(
            async breadcrumb =>
            {
                await Task.CompletedTask;
                NavigationHelper.ToBreadcrumb(breadcrumb, Context?.Item);
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
                PathQuery = Context.PathInfo.Path;
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
                else if (lastCrumb != null && !lastCrumb.Path.Equals(Context.PathInfo.Path, StringComparison.OrdinalIgnoreCase))
                {
                    NavigationHelper.ToBreadcrumb(lastCrumb, null);
                }
                IsEditingPath = false;
                PathQuery = null;
            }));
        
        #endregion

        #region View Ribbon

        private RelayCommand _InvertSelection;
        public RelayCommand InvertSelectionCommand => _InvertSelection ?? (_InvertSelection = new RelayCommand(
            () =>
            {
                Messenger.Default.Send(new NotificationMessage(nameof(InvertSelectionCommand)), MessengerTokens.JumpPointViewerSelection);
            },
            () =>
            {
                if (Context == null) return false;
                switch (Context.PathInfo.Type)
                {
                    case AppPath.Dashboard:
                    case AppPath.Folder:
                    case AppPath.Drive:
                    case AppPath.Workspace:
                    case AppPath.Favorites:
                    case AppPath.Drives:
                    case AppPath.Workspaces:
                    case AppPath.AppLinks:
                        return true;

                    case AppPath.Properties:
                    case AppPath.Settings:
                    case AppPath.Unknown:
                    default:
                        return false;
                }
            }));

        private RelayCommand _SelectAll;
        public RelayCommand SelectAllCommand => _SelectAll ?? (_SelectAll = new RelayCommand(
            () =>
            {
                Messenger.Default.Send(new NotificationMessage(nameof(SelectAllCommand)), MessengerTokens.JumpPointViewerSelection);
            },
            () =>
            {
                if (Context == null) return false;
                switch (Context.PathInfo.Type)
                {
                    case AppPath.Dashboard:
                    case AppPath.Folder:
                    case AppPath.Drive:
                    case AppPath.Workspace:
                    case AppPath.Favorites:
                    case AppPath.Drives:
                    case AppPath.Workspaces:
                    case AppPath.AppLinks:
                        return true;
                    
                    case AppPath.Properties:
                    case AppPath.Settings:
                    case AppPath.Unknown:
                    default:
                        return false;
                }
            }));

        private AsyncRelayCommand _SelectNone;
        public AsyncRelayCommand SelectNoneCommand => _SelectNone ?? (_SelectNone = new AsyncRelayCommand(
            async () =>
            {
                Context.SelectedItems.Clear();
                await Task.CompletedTask;
            }));

        #endregion

        #region Support Developer

        private bool _isSubscriber;

        public bool IsSubscriber
        {
            get { return _isSubscriber; }
            set { Set(ref _isSubscriber, value); }
        }

        private ReadOnlyCollection<SubscriptionAddOn> _subscriptions;

        public ReadOnlyCollection<SubscriptionAddOn> Subscriptions
        {
            get { return _subscriptions; }
            set { Set(ref _subscriptions, value); }
        }

        private AsyncRelayCommand _loadSubscriptionsCommand;

        public AsyncRelayCommand LoadSubscriptionsCommand
        {
            get
            {
                return _loadSubscriptionsCommand
                    ?? (_loadSubscriptionsCommand = new AsyncRelayCommand(
                    async () =>
                    {
                        await Run(async (token) =>
                        {
                            Subscriptions = await addOnService.GetSubscriptionAddOns(
                                AddOnKeys.Monthly1, AddOnKeys.Monthly2, AddOnKeys.Monthly3, AddOnKeys.Monthly4, AddOnKeys.Monthly5);
                            foreach (var item in Subscriptions)
                            {
                                var isActive = await addOnService.IsActive(item);
                                if (isActive)
                                {
                                    IsSubscriber = isActive;
                                    break;
                                }
                            }
                        });
                    }));
            }
        }

        private AsyncRelayCommand<SubscriptionAddOn> _PurchaseSubscription;
        public AsyncRelayCommand<SubscriptionAddOn> PurchaseSubscriptionCommand => _PurchaseSubscription ?? (_PurchaseSubscription = new AsyncRelayCommand<SubscriptionAddOn>(
            async item =>
            {
                if (await addOnService.TryPurchase(item))
                {
                    foreach (var subscription in Subscriptions)
                    {
                        var isActive = await addOnService.IsActive(subscription);
                        if (isActive)
                        {
                            IsSubscriber = isActive;
                            break;
                        }
                    }
                }
            }));

        private AsyncRelayCommand _Rate;
        public AsyncRelayCommand RateCommand => _Rate ?? (_Rate = new AsyncRelayCommand(
            async () =>
            {
                await Browser.OpenAsync("ms-windows-store://review/?ProductId=9PL7B4PZ0HS0");
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

        private void ProcessParameter(string parameter)
        {
            var query = QueryString.Parse(parameter);
            if (Enum.TryParse<AppPath>(query[nameof(PathInfo.Type)], out var type))
            {
                NavigationHelper.ToPathType(type, parameter);
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

            // Hook Up Listeners
            ShellItems.Start();
            PortableStorageWatcher.Start();
            clipboardService.Start();
            clipboardService.ContentChanged += OnClipboardContentChanged;
            shareService.Start();

            await LoadSubscriptionsCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessengerTokens.CommandManagement, ManageCommands);
            CancelAll();

            state[nameof(PathInfo)] = new QueryString()
            {
                { nameof(PathInfo.Type), Context.PathInfo.Type.ToString() },
                { nameof(PathInfo.Path), Context.PathInfo.Path }
            }.ToString();

            // Unhook Listeners
            ShellItems.Stop();
            PortableStorageWatcher.Stop();
            clipboardService.Stop();
            clipboardService.ContentChanged -= OnClipboardContentChanged;
            shareService.Stop();
        }

        private void ManageCommands(NotificationMessage message)
        {
            switch (message.Notification)
            {
                case nameof(Context.Item):
                    commandHelper.OpenPathInFileExplorerCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenPathInCommandPromptCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenPathInPowershellCommand.RaiseCanExecuteChanged();

                    commandHelper.AddPathToWorkspaceCommand.RaiseCanExecuteChanged();
                    commandHelper.AddPathToFavoritesCommand.RaiseCanExecuteChanged();
                    commandHelper.RemovePathFromFavoritesCommand.RaiseCanExecuteChanged();

                    commandHelper.SetPathWorkspaceTemplateCommand.RaiseCanExecuteChanged();
                    commandHelper.SetPathFolderTemplateCommand.RaiseCanExecuteChanged();
                    break;

                case nameof(Context.PathInfo):
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

                case nameof(Context.SelectedItems):
                    // Toolbar
                    commandHelper.CopyCommand.RaiseCanExecuteChanged();
                    commandHelper.CutCommand.RaiseCanExecuteChanged();
                    commandHelper.RenameCommand.RaiseCanExecuteChanged();
                    commandHelper.DeleteCommand.RaiseCanExecuteChanged();
                    commandHelper.DeletePermanentlyCommand.RaiseCanExecuteChanged();

                    commandHelper.OpenCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenWithCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenItemsInNewWindowCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenItemsInFileExplorerCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenItemsInCommandPromptCommand.RaiseCanExecuteChanged();
                    commandHelper.OpenItemsInPowershellCommand.RaiseCanExecuteChanged();
                    commandHelper.ShareItemsCommand.RaiseCanExecuteChanged();
                    commandHelper.ItemsPropertiesCommand.RaiseCanExecuteChanged();

                    commandHelper.AddItemsToWorkspaceCommand.RaiseCanExecuteChanged();
                    commandHelper.AddItemsToFavoritesCommand.RaiseCanExecuteChanged();
                    commandHelper.RemoveItemsFromFavoritesCommand.RaiseCanExecuteChanged();
                    commandHelper.SetItemsWorkspaceTemplateCommand.RaiseCanExecuteChanged();
                    commandHelper.SetItemsFolderTemplateCommand.RaiseCanExecuteChanged();
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

        private async void OnClipboardContentChanged(object sender, EventArgs e)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                try
                {
                    var data = await clipboardService.GetData();
                    commandHelper.ClipboardHasFiles = data.StorageItems != null && data.StorageItems.Count > 0;
                }
                catch
                {
                }
                
            });
        }

    }
}
