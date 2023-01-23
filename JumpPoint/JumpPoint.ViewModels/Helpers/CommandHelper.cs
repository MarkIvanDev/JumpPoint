using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Services;
using NittyGritty.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty.Services.Core;
using Xamarin.Essentials;
using FileBase = JumpPoint.Platform.Items.FileBase;
using JumpPoint.ViewModels.Dialogs;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Models;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Extensions;
using JumpPoint.ViewModels.Commands;
using System.Linq;
using NittyGritty.Platform.Data;
using System.Collections.ObjectModel;
using NGStorage = NittyGritty.Platform.Storage;
using NittyGritty.Platform.Shortcut;
using System.IO;
using System.Threading.Tasks;
using NittyGritty.Extensions;
using NittyGritty;
using JumpPoint.ViewModels.Parameters;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.ViewModels.Dialogs.Clipboard;
using System.Diagnostics;
using JumpPoint.Platform.Items.CloudStorage;

namespace JumpPoint.ViewModels.Helpers
{
    public class CommandHelper : ObservableObject
    {
        private readonly IDialogService dialogService;
        private readonly IShareService shareService;
        private readonly IShortcutService shortcutService;
        private readonly IClipboardService clipboardService;
        private readonly ITileIconHelper iconHelper;
        private readonly AppSettings appSettings;

        public CommandHelper(IDialogService dialogService,
                             IShareService shareService,
                             IShortcutService shortcutService,
                             IClipboardService clipboardService,
                             ITileIconHelper iconHelper,
                             AppSettings appSettings)
        {
            this.dialogService = dialogService;
            this.shareService = shareService;
            this.shortcutService = shortcutService;
            this.clipboardService = clipboardService;
            this.iconHelper = iconHelper;
            this.appSettings = appSettings;
            clipboardService.Start();
            clipboardService.ContentChanged += OnClipboardContentChanged;
        }

        private async void OnClipboardContentChanged(object sender, EventArgs e)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    ClipboardHasFiles = clipboardService.ContainsStorageItems();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Clipboard Content changed: {ex.Message}");
                }

            });
        }

        private AsyncRelayCommand<Uri> _OpenUri;
        public AsyncRelayCommand<Uri> OpenUriCommand => _OpenUri ?? (_OpenUri = new AsyncRelayCommand<Uri>(
            async uri =>
            {
                await Launcher.OpenAsync(uri);
            }));


        private AsyncRelayCommand<JumpPointItem> _LaunchItem;
        public AsyncRelayCommand<JumpPointItem> LaunchItemCommand => _LaunchItem ?? (_LaunchItem = new AsyncRelayCommand<JumpPointItem>(
            async item =>
            {
                switch (item.Type)
                {
                    case JumpPointItemType.Drive:
                        await Launcher.OpenAsync(JumpPointService.GetAppUri(AppPath.Drive, item.Path));
                        break;

                    case JumpPointItemType.Folder:
                        await Launcher.OpenAsync(JumpPointService.GetAppUri(AppPath.Folder, item.Path));
                        break;

                    case JumpPointItemType.File:
                        await JumpPointService.OpenFile((FileBase)item, true);
                        break;

                    case JumpPointItemType.Workspace:
                        await Launcher.OpenAsync(JumpPointService.GetAppUri(AppPath.Workspace, item.Path));
                        break;

                    case JumpPointItemType.AppLink when item is AppLink appLink:
                        var launchType = appLink.LaunchTypes;
                        if (launchType == AppLinkLaunchTypes.All || appLink.Query.Count > 0 || appLink.InputData.Count > 0)
                        {
                            var appLinkLaunch = new AppLinkLaunchViewModel(appLink);
                            await dialogService.Show(DialogKeys.AppLinkLaunch, appLinkLaunch, appSettings.Theme);
                            launchType = appLinkLaunch.LaunchType;
                        }

                        switch (launchType)
                        {
                            case AppLinkLaunchTypes.Uri:
                                await AppLinkService.OpenUri(appLink);
                                break;
                            case AppLinkLaunchTypes.UriForResults:
                                var results = await AppLinkService.OpenUriForResults(appLink);
                                var appLinkLaunchResults = new AppLinkLaunchResultsViewModel(appLink, results);
                                await dialogService.Show(DialogKeys.AppLinkLaunchResults, appLinkLaunchResults, appSettings.Theme);
                                break;
                            case AppLinkLaunchTypes.All:
                            case AppLinkLaunchTypes.None:
                            default:
                                break;
                        }
                        break;

                    case JumpPointItemType.Library:
                    case JumpPointItemType.Unknown:
                    default:
                        break;
                }
            }));

        private async Task AddToWorkspace(IList<JumpPointItem> items)
        {
            var viewModel = new AddToWorkspaceViewModel();
            var result = await dialogService.Show(DialogKeys.AddToWorkspace, viewModel,
                async (vm) =>
                {
                    vm.IsLoading = true;
                    var wss = (await WorkspaceService.GetWorkspaces()).Select(w => new SelectableWorkspace() { Item = w });
                    var collection = new Collection<SelectableWorkspace>();
                    foreach (var ws in wss)
                    {
                        var results = await Task.WhenAll(items.Select(f => WorkspaceService.ItemExists(ws.Item.Id, f)));
                        if (results.All(r => r == true))
                        {
                            ws.IsSelected = true;
                        }
                        else if (results.Any(r => r == true))
                        {
                            ws.IsSelected = null;
                        }
                        else
                        {
                            ws.IsSelected = false;
                        }
                        collection.Add(ws);
                    }
                    vm.Workspaces = collection;
                    vm.IsLoading = false;
                }, appSettings.Theme);

            if (result)
            {
                foreach (var ws in viewModel.Workspaces)
                {
                    if (ws.IsSelected == true)
                    {
                        foreach (var item in items)
                        {
                            await WorkspaceService.InsertItem(ws.Item.Id, item);
                        }
                    }
                    else if (ws.IsSelected == false)
                    {
                        foreach (var item in items)
                        {
                            await WorkspaceService.RemoveItem(ws.Item.Id, item);
                        }
                    }
                }
            }
        }


        #region Navigation Bar

        #region Direction Commands
        // We will use the TabViewModel as a parameter because the Direction commands need the NavigationHelper
        private RelayCommand<TabViewModel> _Back;
        public RelayCommand<TabViewModel> BackCommand => _Back ?? (_Back = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                tab?.NavigationHelper.GoBack();
            }));

        private RelayCommand<TabViewModel> _Forward;
        public RelayCommand<TabViewModel> ForwardCommand => _Forward ?? (_Forward = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                tab?.NavigationHelper.GoForward();
            }));

        private AsyncRelayCommand<TabViewModel> _Up;
        public AsyncRelayCommand<TabViewModel> UpCommand => _Up ?? (_Up = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                if (tab?.Context is null) return;

                switch (tab.Context.PathInfo.Type)
                {
                    case AppPath.Folder when tab.Context.Item is FolderBase folder:
                        var upCrumb = tab.Context.PathInfo.Breadcrumbs.ElementAtOrDefault(tab.Context.PathInfo.Breadcrumbs.Count - 2);
                        if (upCrumb != null)
                        {
                            tab.NavigationHelper.ToDirectory(upCrumb.AppPath, folder.StorageType, upCrumb.Path);
                        }
                        break;

                    default:
                        tab.NavigationHelper.GoUp(tab.Context.PathInfo.Type);
                        break;
                }
                await Task.CompletedTask;
            }));

        private RelayCommand<TabViewModel> _Dashboard;
        public RelayCommand<TabViewModel> DashboardCommand => _Dashboard ?? (_Dashboard = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                tab?.NavigationHelper.ToPathType(AppPath.Dashboard);
            }));

        private RelayCommand<TabViewModel> _Settings;
        public RelayCommand<TabViewModel> SettingsCommand => _Settings ?? (_Settings = new RelayCommand<TabViewModel>(
            (tab) =>
            {
                tab?.NavigationHelper.ToKey(ViewModelKeys.Settings);
            }));

        #endregion

        #region New Item
        // We will use the TabViewModel as a parameter because the New Item commands need the NavigationHelper

        private AsyncRelayCommand<TabViewModel> _NewFile;
        public AsyncRelayCommand<TabViewModel> NewFileCommand => _NewFile ?? (_NewFile = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                if (tab?.Context is null) return;

                var viewModel = new NewFileViewModel();
                var result = await dialogService.Show(DialogKeys.NewFile, viewModel);
                if (result && tab.Context.Item is DirectoryBase parent)
                {
                    var newFile = await StorageService.CreateFile(parent, viewModel.Name, CreateOption.GenerateUniqueName, null);
                    if (newFile != null)
                    {
                        if (parent.StorageType == StorageType.Cloud || parent.StorageType == StorageType.WSL)
                        {
                            await tab.Context.RefreshCommand.TryExecute();
                        }
                    }
                    else
                    {
                        await dialogService.ShowMessage("You may need permission to perform this action", "Destination Folder Access Denied", appSettings.Theme);
                    }
                }
            }));

        private AsyncRelayCommand<TabViewModel> _NewFolder;
        public AsyncRelayCommand<TabViewModel> NewFolderCommand => _NewFolder ?? (_NewFolder = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                if (tab?.Context is null) return;

                var viewModel = new NewFolderViewModel();
                var result = await dialogService.Show(DialogKeys.NewFolder, viewModel, appSettings.Theme);
                if (result && tab.Context.Item is DirectoryBase parent)
                {
                    var newFolder = await StorageService.CreateFolder(parent, viewModel.Name, CreateOption.GenerateUniqueName);
                    if (newFolder != null)
                    {
                        if (parent.StorageType == StorageType.Cloud || parent.StorageType == StorageType.WSL)
                        {
                            await tab.Context.RefreshCommand.TryExecute();
                        }
                    }
                    else
                    {
                        await dialogService.ShowMessage("You may need permission to perform this action", "Destination Folder Access Denied", appSettings.Theme);
                    }
                }
            }));

        private AsyncRelayCommand<TabViewModel> _NewWorkspace;
        public AsyncRelayCommand<TabViewModel> NewWorkspaceCommand => _NewWorkspace ?? (_NewWorkspace = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                var viewModel = new NewWorkspaceViewModel();
                var result = await dialogService.Show(DialogKeys.NewWorkspace, viewModel, appSettings.Theme);
                if (result)
                {
                    var ws = new WorkspaceInfo()
                    {
                        Name = viewModel.Name,
                        DateCreated = DateTimeOffset.UtcNow,
                        Template = viewModel.Template
                    };
                    var workspace = await WorkspaceService.Create(ws);

                    if (workspace != null)
                    {
                        Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                            new SidebarMessage(CollectionChangedAction.Add, new List<JumpPointItem> { workspace }), nameof(AppPath.Workspaces)),
                            MessengerTokens.SidebarManagement);
                        tab?.NavigationHelper.ToWorkspace(workspace);
                    }
                }
            }));

        private AsyncRelayCommand<TabViewModel> _NewAppLink;
        public AsyncRelayCommand<TabViewModel> NewAppLinkCommand => _NewAppLink ?? (_NewAppLink = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                var viewModel = new AppLinkProviderPickerViewModel();
                var result = await dialogService.Show(DialogKeys.AppLinkProviderPicker, viewModel, async (vm) =>
                {
                    await vm.Initialize();
                }, appSettings.Theme);
                if (result && viewModel.Provider != null)
                {
                    var appLinkInfo = await AppLinkProviderService.Pick(viewModel.Provider);
                    if (appLinkInfo != null)
                    {
                        var appLink = await AppLinkService.Create(appLinkInfo);
                        if (appLink != null)
                        {
                            tab?.NavigationHelper.ToKey(ViewModelKeys.AppLinks);
                        }
                    }
                }
            }));

        private AsyncRelayCommand<TabViewModel> _MoreNewItems;
        public AsyncRelayCommand<TabViewModel> MoreNewItemsCommand => _MoreNewItems ?? (_MoreNewItems = new AsyncRelayCommand<TabViewModel>(
            async (tab) =>
            {
                if (tab?.Context is null) return;

                var viewModel = new NewItemPickerViewModel();
                var result = await dialogService.Show(DialogKeys.NewItemPicker, viewModel, async (vm) =>
                {
                    await vm.Initialize();
                }, appSettings.Theme);
                if (result && viewModel.NewItem != null && tab.Context.Item is DirectoryBase destination)
                {
                    var itemsCreated = await NewItemService.Run(viewModel.NewItem, destination);
                    if (itemsCreated)
                    {
                        if (destination.StorageType == StorageType.Cloud || destination.StorageType == StorageType.WSL)
                        {
                            await tab.Context.RefreshCommand.TryExecute();
                        }
                    }
                    else
                    {
                        await dialogService.ShowMessage("You may need permission to perform this action", "Destination Folder Access Denied", appSettings.Theme);
                    }
                }
            }));

        #endregion

        #region Context Commands

        private AsyncRelayCommand<ShellContextViewModelBase> _CopyPath;
        public AsyncRelayCommand<ShellContextViewModelBase> CopyPathCommand => _CopyPath ?? (_CopyPath = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await Clipboard.SetTextAsync(context.PathInfo.Path);
            }));

        #region Open
        private AsyncRelayCommand<ShellContextViewModelBase> _NewWindow;
        public AsyncRelayCommand<ShellContextViewModelBase> NewWindowCommand => _NewWindow ?? (_NewWindow = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await JumpPointService.OpenNewWindow(context.PathInfo.Type, context.PathInfo.Path);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenPathInNewWindow;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenPathInNewWindowCommand => _OpenPathInNewWindow ?? (_OpenPathInNewWindow = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await JumpPointService.OpenNewWindow(context.PathInfo.Type, context.PathInfo.Path);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenPathInFileExplorer;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenPathInFileExplorerCommand => _OpenPathInFileExplorer ?? (_OpenPathInFileExplorer = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await JumpPointService.OpenInFileExplorer(context.PathInfo.Path);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenPathInCommandPrompt;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenPathInCommandPromptCommand => _OpenPathInCommandPrompt ?? (_OpenPathInCommandPrompt = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await DesktopService.OpenInCommandPrompt(new List<string> { context.PathInfo.Path });
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenPathInPowershell;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenPathInPowershellCommand => _OpenPathInPowershell ?? (_OpenPathInPowershell = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await DesktopService.OpenInPowershell(new List<string> { context.PathInfo.Path });
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenPathInWindowsTerminal;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenPathInWindowsTerminalCommand => _OpenPathInWindowsTerminal ?? (_OpenPathInWindowsTerminal = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await DesktopService.OpenInWindowsTerminal(new List<string> { context.PathInfo.Path });
            }));

        #endregion

        private AsyncRelayCommand<ShellContextViewModelBase> _AddPathToWorkspace;
        public AsyncRelayCommand<ShellContextViewModelBase> AddPathToWorkspaceCommand => _AddPathToWorkspace ?? (_AddPathToWorkspace = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.Item is null) return;

                var fsi = Enumerable.Repeat(context.Item, 1).ToList();
                await AddToWorkspace(fsi);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _AddPathToFavorites;
        public AsyncRelayCommand<ShellContextViewModelBase> AddPathToFavoritesCommand => _AddPathToFavorites ?? (_AddPathToFavorites = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.Item is null) return;

                await DashboardService.SetStatus(context.Item, true);
                context.Item.IsFavorite = true;
                Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                    new SidebarMessage(CollectionChangedAction.Add, new List<JumpPointItem> { context.Item }), nameof(AppPath.Favorites)),
                    MessengerTokens.SidebarManagement);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _RemovePathFromFavorites;
        public AsyncRelayCommand<ShellContextViewModelBase> RemovePathFromFavoritesCommand => _RemovePathFromFavorites ?? (_RemovePathFromFavorites = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.Item is null) return;

                await DashboardService.SetStatus(context.Item, false);
                context.Item.IsFavorite = false;
                Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                    new SidebarMessage(CollectionChangedAction.Remove, new List<JumpPointItem> { context.Item }), nameof(AppPath.Favorites)),
                    MessengerTokens.SidebarManagement);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _SetPathWorkspaceTemplate;
        public AsyncRelayCommand<ShellContextViewModelBase> SetPathWorkspaceTemplateCommand => _SetPathWorkspaceTemplate ?? (_SetPathWorkspaceTemplate = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.Item is null) return;

                var viewModel = new WorkspaceTemplatePickerViewModel();
                var result = await dialogService.Show(DialogKeys.WorkspaceTemplatePicker, viewModel, appSettings.Theme);
                if (result && viewModel.Template.HasValue && context.Item is Workspace ws)
                {
                    await WorkspaceService.SetTemplate(ws.Id, viewModel.Template.Value);
                    ws.Template = viewModel.Template.Value;
                    context.PathInfo.Tag = ws.Template;
                    Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                        new SidebarMessage(CollectionChangedAction.Update, new List<JumpPointItem> { ws }), nameof(AppPath.Workspace)),
                        MessengerTokens.SidebarManagement);
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _SetPathFolderTemplate;
        public AsyncRelayCommand<ShellContextViewModelBase> SetPathFolderTemplateCommand => _SetPathFolderTemplate ?? (_SetPathFolderTemplate = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.Item is null) return;

                var viewModel = new FolderTemplatePickerViewModel();
                var result = await dialogService.Show(DialogKeys.FolderTemplatePicker, viewModel, appSettings.Theme);
                if (result && viewModel.Template.HasValue && context.Item is FolderBase f)
                {
                    await FolderTemplateService.SetFolderTemplate(f, viewModel.Template.Value);
                    f.FolderTemplate = viewModel.Template.Value;
                    context.PathInfo.Tag = f.FolderTemplate;
                    Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                        new SidebarMessage(CollectionChangedAction.Update, new List<JumpPointItem> { f }), nameof(AppPath.Folder)),
                        MessengerTokens.SidebarManagement);
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _SharePath;
        public AsyncRelayCommand<ShellContextViewModelBase> SharePathCommand => _SharePath ?? (_SharePath = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var shareData = new DataPayload()
                {
                    Title = context.PathInfo.Path,
                    Description = "Shared from Jump Point",
                    AppLink = JumpPointService.GetAppUri(context.PathInfo.Type, context.PathInfo.Path)
                };

                if (context.Item is StorageItemBase fsi)
                {
                    var storageItems = await JumpPointService.Convert(fsi);
                    shareData.StorageItems = new ReadOnlyCollection<NGStorage.IStorageItem>(Enumerable.Repeat(storageItems, 1).ToList());
                }

                shareService.Share(shareData);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _PathProperties;
        public AsyncRelayCommand<ShellContextViewModelBase> PathPropertiesCommand => _PathProperties ?? (_PathProperties = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var seeds = new Collection<Seed>();
                switch (context.PathInfo.Type)
                {
                    case AppPath.Folder:
                        seeds.Add(new Seed() { Type = JumpPointItemType.Folder, Path = context.PathInfo.Path });
                        break;
                    case AppPath.Drive:
                        seeds.Add(new Seed() { Type = JumpPointItemType.Drive, Path = context.PathInfo.Path });
                        break;
                    case AppPath.Workspace:
                        seeds.Add(new Seed() { Type = JumpPointItemType.Workspace, Path = context.PathInfo.Path });
                        break;

                    default:
                        break;
                }

                await JumpPointService.OpenProperties(seeds);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _PinPath;
        public AsyncRelayCommand<ShellContextViewModelBase> PinPathCommand => _PinPath ?? (_PinPath = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context?.Item is null) return;

                if (!shortcutService.Exists(context.PathHash))
                {
                    await shortcutService.Create(new ShortcutItem()
                    {
                        Id = context.PathHash,
                        DisplayName = context.PathInfo.DisplayName,
                        Arguments = TabbedNavigationHelper.GetParameter(context.PathInfo.Type, context.PathInfo.Path, context.Item),
                        Icon = iconHelper.GetIconUri(context.PathInfo.Type, context.PathInfo.Tag, TileSize.Medium),
                        SmallIcon = iconHelper.GetIconUri(context.PathInfo.Type, context.PathInfo.Tag, TileSize.Small),
                        WideIcon = iconHelper.GetIconUri(context.PathInfo.Type, context.PathInfo.Tag, TileSize.Wide),
                        LargeIcon = iconHelper.GetIconUri(context.PathInfo.Type, context.PathInfo.Tag, TileSize.Large)
                    });
                }
                context.IsPinned = shortcutService.Exists(context.PathHash);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _UnpinPath;
        public AsyncRelayCommand<ShellContextViewModelBase> UnpinPathCommand => _UnpinPath ?? (_UnpinPath = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                if (shortcutService.Exists(context.PathHash))
                {
                    await shortcutService.Delete(context.PathHash);
                }
                context.IsPinned = shortcutService.Exists(context.PathHash);
            }));
        #endregion

        #endregion

        // Ww will use the ShellContextViewModelBase as a parameter
        // because these are also used in JumpPointViewer and only Context is available there
        #region Toolbar

        #region Clipboard
        private bool _clipboardHasFiles;

        public bool ClipboardHasFiles
        {
            get { return _clipboardHasFiles; }
            set
            {
                _clipboardHasFiles = value;
                RaisePropertyChanged();
            }
        }

        private AsyncRelayCommand<ShellContextViewModelBase> _Copy;
        public AsyncRelayCommand<ShellContextViewModelBase> CopyCommand => _Copy ?? (_Copy = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var payload = new DataPayload
                {
                    StorageItems = new ReadOnlyCollection<NGStorage.IStorageItem>(await JumpPointService.Convert(context.SelectedItems))
                };
                clipboardService.Copy(payload);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _Cut;
        public AsyncRelayCommand<ShellContextViewModelBase> CutCommand => _Cut ?? (_Cut = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var payload = new DataPayload
                {
                    StorageItems = new ReadOnlyCollection<NGStorage.IStorageItem>(await JumpPointService.Convert(context.SelectedItems))
                };
                clipboardService.Cut(payload);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _CopyItemsPath;
        public AsyncRelayCommand<ShellContextViewModelBase> CopyItemsPathCommand => _CopyItemsPath ?? (_CopyItemsPath = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                await Clipboard.SetTextAsync(string.Join(appSettings.CopyPathDelimiter.ToDelimiter(), context.SelectedItems.Select(i => i.Path)));
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _Paste;
        public AsyncRelayCommand<ShellContextViewModelBase> PasteCommand => _Paste ?? (_Paste = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                if (context.Item is DirectoryBase destination)
                {
                    await StorageService.Paste(destination);
                }
            }));
        #endregion

        #region Clipboard Manager

        private AsyncRelayCommand<CopyToParameter> _CopyTo;
        public AsyncRelayCommand<CopyToParameter> CopyToCommand => _CopyTo ?? (_CopyTo = new AsyncRelayCommand<CopyToParameter>(
            async copyTo =>
            {
                var pathKind = copyTo.Destination.Path.GetPathKind();
                if (pathKind == PathKind.Mounted || pathKind == PathKind.Network)
                {
                    var regularItems = new List<StorageItemBase>();
                    var unmountedItems = new List<StorageItemBase>();
                    foreach (var item in copyTo.Items)
                    {
                        if (item.StorageType == StorageType.Portable && item.Path.GetPathKind() == PathKind.Unmounted)
                        {
                            unmountedItems.Add(item);
                        }
                        else if (item.StorageType != StorageType.Cloud)
                        {
                            regularItems.Add(item);
                        }
                    }
                    if (regularItems.Count > 0)
                    {
                        await DesktopService.CopyTo(copyTo.Destination.Path, regularItems.Select(i => i.Path).ToList());
                    }
                    if (unmountedItems.Count > 0)
                    {
                        var viewModel = new CopyViewModel(dialogService, copyTo.Destination, unmountedItems);
                        await dialogService.Show(DialogKeys.Copy, viewModel, async (vm) => await vm.Start(), appSettings.Theme);
                    }
                }
                else if (pathKind == PathKind.Unmounted)
                {
                    var viewModel = new CopyViewModel(dialogService, copyTo.Destination, copyTo.Items);
                    await dialogService.Show(DialogKeys.Copy, viewModel, async (vm) => await vm.Start(), appSettings.Theme);
                }
            }));

        private AsyncRelayCommand<MoveToParameter> _MoveTo;
        public AsyncRelayCommand<MoveToParameter> MoveToCommand => _MoveTo ?? (_MoveTo = new AsyncRelayCommand<MoveToParameter>(
            async moveTo =>
            {
                var pathKind = moveTo.Destination.Path.GetPathKind();
                if (pathKind == PathKind.Mounted || pathKind == PathKind.Network)
                {
                    var regularItems = new List<StorageItemBase>();
                    var unmountedItems = new List<StorageItemBase>();
                    foreach (var item in moveTo.Items)
                    {
                        if (item.StorageType == StorageType.Portable && item.Path.GetPathKind() == PathKind.Unmounted)
                        {
                            unmountedItems.Add(item);
                        }
                        else if (item.StorageType != StorageType.Cloud)
                        {
                            regularItems.Add(item);
                        }
                    }
                    if (regularItems.Count > 0)
                    {
                        await DesktopService.MoveTo(moveTo.Destination.Path, regularItems.Select(i => i.Path).ToList());
                    }
                    if (unmountedItems.Count > 0)
                    {
                        var viewModel = new MoveViewModel(dialogService, moveTo.Destination, unmountedItems);
                        await dialogService.Show(DialogKeys.Move, viewModel, async (vm) => await vm.Start(), appSettings.Theme);
                    }
                }
                else if (pathKind == PathKind.Unmounted)
                {
                    var viewModel = new MoveViewModel(dialogService, moveTo.Destination, moveTo.Items);
                    await dialogService.Show(DialogKeys.Move, viewModel, async (vm) => await vm.Start(), appSettings.Theme);
                }
            }));

        private AsyncRelayCommand _ClipboardManager;
        public AsyncRelayCommand ClipboardManagerCommand => _ClipboardManager ?? (_ClipboardManager = new AsyncRelayCommand(
            async () =>
            {
                await JumpPointService.OpenNewWindow(AppPath.ClipboardManager, null);
            }));

        #endregion

        #region Edit

        private AsyncRelayCommand<ShellContextViewModelBase> _Rename;
        public AsyncRelayCommand<ShellContextViewModelBase> RenameCommand => _Rename ?? (_Rename = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var toRename = context.SelectedItems
                    .Where(i => i.Type == JumpPointItemType.Folder || i.Type == JumpPointItemType.File || i.Type == JumpPointItemType.Workspace || i.Type == JumpPointItemType.AppLink)
                    .ToList();
                var viewModel = new RenameViewModel(toRename.Select(i => i.Name).ToList());
                var result = await dialogService.Show(DialogKeys.Rename, viewModel, appSettings.Theme);
                if (result)
                {
                    foreach (var item in toRename)
                    {
                        var newName = await JumpPointService.Rename(item, viewModel.NewName, viewModel.Action);
                        if (!string.IsNullOrEmpty(newName))
                        {
                            item.Name = newName;
                        }
                    }
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _Delete;
        public AsyncRelayCommand<ShellContextViewModelBase> DeleteCommand => _Delete ?? (_Delete = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var result = await dialogService.ShowMessage("Are you sure you want to delete these items?", "Delete Items", "Delete", "Cancel", appSettings.Theme);
                if (result)
                {
                    await JumpPointService.Delete(context.SelectedItems, false);
                    if (context.Item is DirectoryBase dir && (dir.StorageType == StorageType.Cloud || dir.StorageType == StorageType.WSL))
                    {
                        await context.RefreshCommand.TryExecute();
                    }
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _DeletePermanently;
        public AsyncRelayCommand<ShellContextViewModelBase> DeletePermanentlyCommand => _DeletePermanently ?? (_DeletePermanently = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var result = await dialogService.ShowMessage("Are you sure you want to delete these permanently?", "Delete Items Permanently", "Delete", "Cancel", appSettings.Theme);
                if (result)
                {
                    await JumpPointService.Delete(context.SelectedItems, true);
                    if (context.Item is DirectoryBase dir && (dir.StorageType == StorageType.Cloud || dir.StorageType == StorageType.WSL))
                    {
                        await context.RefreshCommand.TryExecute();
                    }
                }
            }));

        #endregion

        #region Open

        private AsyncRelayCommand<ShellContextViewModelBase> _Open;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenCommand => _Open ?? (_Open = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var navigationHelper = ViewModelLocator.Instance.GetNavigationHelper(context.TabKey);
                foreach (var item in context.SelectedItems)
                {
                    await OpenItemCommand.TryExecute(new OpenItemParameter(navigationHelper, item));
                    if (item.Type != JumpPointItemType.File)
                    {
                        break;
                    }
                }
            }));

        private AsyncRelayCommand<OpenItemParameter> _OpenItem;
        public AsyncRelayCommand<OpenItemParameter> OpenItemCommand => _OpenItem ?? (_OpenItem = new AsyncRelayCommand<OpenItemParameter>(
            async (openParameter) =>
            {
                if (openParameter is null) return;

                if (openParameter.Item is FolderBase folder)
                {
                    openParameter.NavigationHelper.ToFolder(folder);
                }
                else if (openParameter.Item is DriveBase drive)
                {
                    openParameter.NavigationHelper.ToDrive(drive);
                }
                else if (openParameter.Item is Workspace workspace)
                {
                    openParameter.NavigationHelper.ToWorkspace(workspace);
                }
                else if (openParameter.Item is Library library)
                {
                    openParameter.NavigationHelper.ToLibrary(library);
                }
                else if (openParameter.Item is FileBase file)
                {
                    if (file.StorageType != StorageType.Cloud)
                    {
                        var result = await JumpPointService.OpenFile(file, true);
                        if (!result)
                        {
                            var openInFE = await dialogService.ShowMessage("You could open the file from file explorer which we will open for you. Proceed?",
                                "Open in File Explorer?", "Open", "Cancel", appSettings.Theme);
                            if (openInFE)
                            {
                                await JumpPointService.OpenInFileExplorer(Path.GetDirectoryName(file.Path), new List<StorageItemBase> { file });
                            }
                        }
                    }
                    else
                    {
                        await CloudStorageService.OpenFile(file);
                    }
                }
                else if (openParameter.Item is AppLink appLink)
                {
                    var launchType = appLink.LaunchTypes;
                    if (launchType == AppLinkLaunchTypes.All || appLink.Query.Count > 0 || appLink.InputData.Count > 0)
                    {
                        var appLinkLaunch = new AppLinkLaunchViewModel(appLink);
                        await dialogService.Show(DialogKeys.AppLinkLaunch, appLinkLaunch, appSettings.Theme);
                        launchType = appLinkLaunch.LaunchType;
                    }

                    switch (launchType)
                    {
                        case AppLinkLaunchTypes.Uri:
                            await AppLinkService.OpenUri(appLink);
                            break;
                        case AppLinkLaunchTypes.UriForResults:
                            var results = await AppLinkService.OpenUriForResults(appLink);
                            var appLinkLaunchResults = new AppLinkLaunchResultsViewModel(appLink, results);
                            await dialogService.Show(DialogKeys.AppLinkLaunchResults, appLinkLaunchResults, appSettings.Theme);
                            break;
                        case AppLinkLaunchTypes.All:
                        case AppLinkLaunchTypes.None:
                        default:
                            break;
                    }
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenWith;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenWithCommand => _OpenWith ?? (_OpenWith = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                foreach (var item in context.SelectedItems)
                {
                    if (item is FileBase file)
                    {
                        await JumpPointService.OpenFile(file, false);
                    }
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInNewWindowCommand;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInNewWindowCommand => _OpenItemsInNewWindowCommand ?? (_OpenItemsInNewWindowCommand = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                foreach (var item in context.SelectedItems)
                {
                    var pathType = AppPath.Unknown;
                    switch (item.Type)
                    {
                        case JumpPointItemType.Folder:
                            pathType = AppPath.Folder;
                            break;
                        case JumpPointItemType.Drive:
                            pathType = AppPath.Drive;
                            break;
                        case JumpPointItemType.Workspace:
                            pathType = AppPath.Workspace;
                            break;
                    }

                    if (pathType != AppPath.Unknown)
                    {
                        await JumpPointService.OpenNewWindow(pathType, item.Path);
                    }
                }

            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInFileExplorer;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInFileExplorerCommand => _OpenItemsInFileExplorer ?? (_OpenItemsInFileExplorer = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var fbs = context.SelectedItems.OfType<DirectoryBase>();
                foreach (var item in fbs)
                {
                    await JumpPointService.OpenInFileExplorer(item.Path);
                }

                var fi = context.SelectedItems.OfType<FileBase>();
                var groups = fi.GroupBy(i => new DirectoryInfo(i.Path).Parent.FullName);
                foreach (var group in groups)
                {
                    await JumpPointService.OpenInFileExplorer(group.Key, group);
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInCommandPrompt;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInCommandPromptCommand => _OpenItemsInCommandPrompt ?? (_OpenItemsInCommandPrompt = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var fbs = context.SelectedItems.OfType<DirectoryBase>();
                await DesktopService.OpenInCommandPrompt(fbs.Select(i => i.Path).ToList());
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInPowershell;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInPowershellCommand => _OpenItemsInPowershell ?? (_OpenItemsInPowershell = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var fbs = context.SelectedItems.OfType<DirectoryBase>();
                await DesktopService.OpenInPowershell(fbs.Select(i => i.Path).ToList());
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _OpenItemsInWindowsTerminal;
        public AsyncRelayCommand<ShellContextViewModelBase> OpenItemsInWindowsTerminalCommand => _OpenItemsInWindowsTerminal ?? (_OpenItemsInWindowsTerminal = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var fbs = context.SelectedItems.OfType<DirectoryBase>();
                await DesktopService.OpenInWindowsTerminal(fbs.Select(i => i.Path).ToList());
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _DownloadItems;
        public AsyncRelayCommand<ShellContextViewModelBase> DownloadItemsCommand => _DownloadItems ?? (_DownloadItems = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var cloudFiles = context.SelectedItems.OfType<CloudFile>().ToList();
                if (cloudFiles.Count > 0)
                {
                    var downloadsFolder = string.Empty;
                    foreach (var item in cloudFiles)
                    {
                        var file = await CloudStorageService.DownloadFile(item);
                        downloadsFolder = Path.GetDirectoryName(file.Path);
                    }
                    await JumpPointService.OpenNewWindow(AppPath.Folder, downloadsFolder);
                }
            }));

        #endregion

        #region Tools

        private AsyncRelayCommand<ShellContextViewModelBase> _AddItemsToWorkspace;
        public AsyncRelayCommand<ShellContextViewModelBase> AddItemsToWorkspaceCommand => _AddItemsToWorkspace ?? (_AddItemsToWorkspace = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var fsi = context.SelectedItems.Where(i => i is StorageItemBase || i is AppLink).ToList();
                await AddToWorkspace(fsi);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _AddItemsToFavorites;
        public AsyncRelayCommand<ShellContextViewModelBase> AddItemsToFavoritesCommand => _AddItemsToFavorites ?? (_AddItemsToFavorites = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var list = context.SelectedItems.ToList();
                foreach (var item in list)
                {
                    await DashboardService.SetStatus(item, true);
                    item.IsFavorite = true;
                }
                Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                    new SidebarMessage(CollectionChangedAction.Add, list), nameof(AppPath.Favorites)),
                    MessengerTokens.SidebarManagement);
                Messenger.Default.Send(new NotificationMessage(nameof(JumpPointItem.IsFavorite)), MessengerTokens.CommandManagement);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _RemoveItemsFromFavorites;
        public AsyncRelayCommand<ShellContextViewModelBase> RemoveItemsFromFavoritesCommand => _RemoveItemsFromFavorites ?? (_RemoveItemsFromFavorites = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var list = context.SelectedItems.ToList();
                foreach (var item in list)
                {
                    await DashboardService.SetStatus(item, false);
                    item.IsFavorite = false;
                }
                Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                    new SidebarMessage(CollectionChangedAction.Remove, list), nameof(AppPath.Favorites)),
                    MessengerTokens.SidebarManagement);
                Messenger.Default.Send(new NotificationMessage(nameof(JumpPointItem.IsFavorite)), MessengerTokens.CommandManagement);
            }));


        private AsyncRelayCommand<ShellContextViewModelBase> _MoreTools;
        public AsyncRelayCommand<ShellContextViewModelBase> MoreToolsCommand => _MoreTools ?? (_MoreTools = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var list = context.SelectedItems.ToList();
                var viewModel = new ToolPickerViewModel(list);
                var result = await dialogService.Show(DialogKeys.ToolPicker, viewModel, async (vm) =>
                {
                    await vm.Initialize();
                }, appSettings.Theme);
                if (result && viewModel.Tool != null)
                {
                    var toolResult = await ToolService.Run(viewModel.Tool, list);
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _SetItemsWorkspaceTemplate;
        public AsyncRelayCommand<ShellContextViewModelBase> SetItemsWorkspaceTemplateCommand => _SetItemsWorkspaceTemplate ?? (_SetItemsWorkspaceTemplate = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var viewModel = new WorkspaceTemplatePickerViewModel();
                var result = await dialogService.Show(DialogKeys.WorkspaceTemplatePicker, viewModel, appSettings.Theme);
                if (result && viewModel.Template.HasValue)
                {
                    var wss = context.SelectedItems.Where(i => i.Type == JumpPointItemType.Workspace).ToList();
                    foreach (Workspace ws in wss)
                    {
                        await WorkspaceService.SetTemplate(ws.Id, viewModel.Template.Value);
                        ws.Template = viewModel.Template.Value;
                    }
                    Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                        new SidebarMessage(CollectionChangedAction.Update, wss), nameof(AppPath.Workspace)),
                        MessengerTokens.SidebarManagement);
                }
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _SetItemsFolderTemplate;
        public AsyncRelayCommand<ShellContextViewModelBase> SetItemsFolderTemplateCommand => _SetItemsFolderTemplate ?? (_SetItemsFolderTemplate = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var viewModel = new FolderTemplatePickerViewModel();
                var result = await dialogService.Show(DialogKeys.FolderTemplatePicker, viewModel, appSettings.Theme);
                if (result && viewModel.Template.HasValue)
                {
                    var fs = context.SelectedItems.Where(i => i.Type == JumpPointItemType.Folder).ToList();
                    foreach (FolderBase f in fs)
                    {
                        await FolderTemplateService.SetFolderTemplate(f, viewModel.Template.Value);
                        f.FolderTemplate = viewModel.Template.Value;
                    }
                    Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                        new SidebarMessage(CollectionChangedAction.Update, fs), nameof(AppPath.Folder)),
                        MessengerTokens.SidebarManagement);
                }
            }));


        #endregion

        private AsyncRelayCommand<ShellContextViewModelBase> _ShareItems;
        public AsyncRelayCommand<ShellContextViewModelBase> ShareItemsCommand => _ShareItems ?? (_ShareItems = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var fsi = context.SelectedItems.OfType<StorageItemBase>().ToArray();
                var toShare = await JumpPointService.Convert(fsi);

                var shareData = new DataPayload()
                {
                    Title = context.PathInfo.Path,
                    Description = "Shared from Jump Point",
                    StorageItems = new ReadOnlyCollection<NGStorage.IStorageItem>(toShare)
                };
                shareService.Share(shareData);
            }));

        private AsyncRelayCommand<ShellContextViewModelBase> _ItemsProperties;
        public AsyncRelayCommand<ShellContextViewModelBase> ItemsPropertiesCommand => _ItemsProperties ?? (_ItemsProperties = new AsyncRelayCommand<ShellContextViewModelBase>(
            async (context) =>
            {
                if (context is null) return;

                var seeds = new Collection<Seed>();
                if (context.SelectedItems.Count > 0)
                {
                    seeds.AddRange(context.SelectedItems.Select(i => new Seed()
                    {
                        Type = i.Type,
                        Path = i.Path
                    }));
                }

                await JumpPointService.OpenProperties(seeds);
            }));


        #region View Ribbon

        private RelayCommand<ShellContextViewModelBase> _SelectAll;
        public RelayCommand<ShellContextViewModelBase> SelectAllCommand => _SelectAll ?? (_SelectAll = new RelayCommand<ShellContextViewModelBase>(
            (context) =>
            {
                if (context is null) return;

                Messenger.Default.Send(new NotificationMessage(nameof(SelectAllCommand)), $"{MessengerTokens.JumpPointViewerSelection}_{context.TabKey}");
            }));

        private RelayCommand<ShellContextViewModelBase> _SelectNone;
        public RelayCommand<ShellContextViewModelBase> SelectNoneCommand => _SelectNone ?? (_SelectNone = new RelayCommand<ShellContextViewModelBase>(
            (context) =>
            {
                if (context is null) return;
                context.SelectedItems.Clear();
            }));

        private RelayCommand<ShellContextViewModelBase> _InvertSelection;
        public RelayCommand<ShellContextViewModelBase> InvertSelectionCommand => _InvertSelection ?? (_InvertSelection = new RelayCommand<ShellContextViewModelBase>(
            (context) =>
            {
                if (context is null) return;

                Messenger.Default.Send(new NotificationMessage(nameof(InvertSelectionCommand)), $"{MessengerTokens.JumpPointViewerSelection}_{context.TabKey}");
            }));

        #endregion


        #endregion
    
    }
}
