using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using GalaSoft.MvvmLight.Messaging;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Dialogs;
using NittyGritty.Commands;
using NittyGritty.Services.Core;
using JumpPoint.Platform;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Items.CloudStorage;
using System.Linq;
using JumpPoint.Platform.Extensions;
using Glif.Pickers;

namespace JumpPoint.ViewModels
{
    public class SettingsViewModel : ShellContextViewModelBase
    {
        private readonly IDialogService dialogService;
        private readonly AppSettings appSettings;

        public SettingsViewModel(IDialogService dialogService, IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {
            this.dialogService = dialogService;
            this.appSettings = appSettings;
        }

        #region Font

        private AsyncRelayCommand _ChangeFont;
        public AsyncRelayCommand ChangeFontCommand => _ChangeFont ?? (_ChangeFont = new AsyncRelayCommand(
            async () =>
            {
                var font = await FontPicker.PickFont(new FontPickerOptions
                {
                    Sources = FontSources.All,
                    ShowFontSize = false,
                    SuggestedFontSize = 16
                });
                if (font != null)
                {
                    appSettings.Font = font.Font;
                    appSettings.FontStyle = font.FontStyle;
                    appSettings.FontWeight = font.FontWeight;
                    appSettings.FontStretch = font.FontStretch;
                }
            }));

        #endregion

        #region System

        private AsyncRelayCommand _RunAtStartupToggled;
        public AsyncRelayCommand RunAtStartupToggledCommand => _RunAtStartupToggled ?? (_RunAtStartupToggled = new AsyncRelayCommand(
            async () =>
            {
                if (appSettings.RunAtStartup)
                {
                    await appSettings.EnableRunAtStartup();
                }
                else
                {
                    await appSettings.DisableRunAtStartup();
                }
            }));

        #endregion

        #region Accounts

        private ObservableCollection<CloudAccountGroup> _accounts;

        public ObservableCollection<CloudAccountGroup> Accounts
        {
            get { return _accounts; }
            set { Set(ref _accounts, value); }
        }

        private AsyncRelayCommand<CloudStorageProvider?> _AddAccount;
        public AsyncRelayCommand<CloudStorageProvider?> AddAccountCommand => _AddAccount ?? (_AddAccount = new AsyncRelayCommand<CloudStorageProvider?>(
            async provider =>
            {
                if (provider.HasValue)
                {
                    IDictionary<string, string> data = null;
                    if (CloudStorageService.TryGetAccountProperties(provider.Value, out var keys))
                    {
                        if (keys.Count > 0)
                        {
                            var dataViewModel = new AddCloudAccountViewModel(provider.Value, keys);
                            var result = await dialogService.Show(DialogKeys.AddCloudAccount, dataViewModel);
                            if (result)
                            {
                                data = dataViewModel.GetData();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    var account = await CloudStorageService.AddAccount(provider.Value, data);
                    if (account != null)
                    {
                        var group = Accounts.FirstOrDefault(g => g.Key == account.Provider);
                        if (group != null)
                        {
                            group.Items.Add(account);
                            Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                                new SidebarMessage(CollectionChangedAction.Reset, null), nameof(AppPath.CloudDrives)),
                                MessengerTokens.SidebarManagement);
                        }
                    }
                }
            }));

        private AsyncRelayCommand<CloudAccount> _RenameAccount;
        public AsyncRelayCommand<CloudAccount> RenameAccountCommand => _RenameAccount ?? (_RenameAccount = new AsyncRelayCommand<CloudAccount>(
            async account =>
            {
                var viewModel = new RenameCloudAccountViewModel(account);
                var result = await dialogService.Show(DialogKeys.RenameCloudAccount, viewModel);
                if (result)
                {
                    var newName = await CloudStorageService.RenameAccount(account, viewModel.Name);
                    account.Name = newName;
                    Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                        new SidebarMessage(CollectionChangedAction.Reset, null), nameof(AppPath.CloudDrives)),
                        MessengerTokens.SidebarManagement);
                }
            }));

        private AsyncRelayCommand<CloudAccount> _RemoveAccount;
        public AsyncRelayCommand<CloudAccount> RemoveAccountCommand => _RemoveAccount ?? (_RemoveAccount = new AsyncRelayCommand<CloudAccount>(
            async account =>
            {
                await CloudStorageService.RemoveAccount(account);
                var group = Accounts.FirstOrDefault(g => g.Key == account.Provider);
                if (group != null)
                {
                    group.Items.Remove(account);
                    Messenger.Default.Send(new NotificationMessage<SidebarMessage>(
                        new SidebarMessage(CollectionChangedAction.Reset, null), nameof(AppPath.CloudDrives)),
                        MessengerTokens.SidebarManagement);
                }
            }));

        #endregion

        #region Extensions

        private ObservableCollection<NewItem> _newItems;

        public ObservableCollection<NewItem> NewItems
        {
            get { return _newItems; }
            set { Set(ref _newItems, value); }
        }

        private ObservableCollection<Tool> _tools;

        public ObservableCollection<Tool> Tools
        {
            get { return _tools; }
            set { Set(ref _tools, value); }
        }

        private ObservableCollection<AppLinkProvider> _appLinkProviders;

        public ObservableCollection<AppLinkProvider> AppLinkProviders
        {
            get { return _appLinkProviders; }
            set { Set(ref _appLinkProviders, value); }
        }

        #endregion

        #region Dashboard

        private ReadOnlyCollection<UserFolderSetting> _dashboardUserFolders;

        public ReadOnlyCollection<UserFolderSetting> DashboardUserFolders
        {
            get { return _dashboardUserFolders; }
            set { Set(ref _dashboardUserFolders, value); }
        }

        private ReadOnlyCollection<SystemFolderSetting> _dashboardSystemFolders;

        public ReadOnlyCollection<SystemFolderSetting> DashboardSystemFolders
        {
            get { return _dashboardSystemFolders; }
            set { Set(ref _dashboardSystemFolders, value); }
        }

        #endregion Dashboard

        protected override async Task Refresh(CancellationToken token)
        {
            await appSettings.RefreshRunAtStartup();
            token.ThrowIfCancellationRequested();

            Accounts = new ObservableCollection<CloudAccountGroup>
            {
                new CloudAccountGroup(CloudStorageProvider.OneDrive, await CloudStorageService.GetAccounts(CloudStorageProvider.OneDrive)),
                new CloudAccountGroup(CloudStorageProvider.Storj, await CloudStorageService.GetAccounts(CloudStorageProvider.Storj)),
                new CloudAccountGroup(CloudStorageProvider.OpenDrive, await CloudStorageService.GetAccounts(CloudStorageProvider.OpenDrive))
            };
            token.ThrowIfCancellationRequested();

            var newItems = await NewItemService.GetNewItems();
            NewItems = new ObservableCollection<NewItem>(newItems);
            token.ThrowIfCancellationRequested();

            var tools = await ToolService.GetTools();
            Tools = new ObservableCollection<Tool>(tools);
            token.ThrowIfCancellationRequested();

            var appLinkProviders = await AppLinkProviderService.GetProviders();
            AppLinkProviders = new ObservableCollection<AppLinkProvider>(appLinkProviders);
            token.ThrowIfCancellationRequested();

            DashboardUserFolders = new ReadOnlyCollection<UserFolderSetting>(await DashboardService.GetUserFolderSettings());
            DashboardSystemFolders = new ReadOnlyCollection<SystemFolderSetting>(await DashboardService.GetSystemFolderSettings());
            token.ThrowIfCancellationRequested();

            //using (var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("JumpPoint.ViewModels.Developer.ProtocolActivation.md"))
            //{
            //    var reader = new StreamReader(stream);
            //    ProtocolActivation = await reader.ReadToEndAsync();
            //}
        }

        #region Developers

        private string _protocolActivation;

        public string ProtocolActivation
        {
            get { return _protocolActivation; }
            set { Set(ref _protocolActivation, value); }
        }

        #endregion

        #region About

        private AsyncRelayCommand _EmailCommand;

        /// <summary>
        /// Gets the EmailCommand.
        /// </summary>
        public AsyncRelayCommand EmailCommand
        {
            get
            {
                return _EmailCommand
                    ?? (_EmailCommand = new AsyncRelayCommand(
                    async () =>
                    {
                        var message = new EmailMessage()
                        {
                            To = new List<string>() { "jumppoint.app@outlook.com" },
                            Subject = "Jump Point"
                        };
                        await Email.ComposeAsync(message);
                    }));
            }
        }

        private AsyncRelayCommand _githubCommand;

        /// <summary>
        /// Gets the GithubCommand.
        /// </summary>
        public AsyncRelayCommand GithubCommand
        {
            get
            {
                return _githubCommand
                    ?? (_githubCommand = new AsyncRelayCommand(
                    async () =>
                    {
                        await Browser.OpenAsync(@"https://github.com/MarkIvanDev/JumpPoint");
                    }));
            }
        }

        private AsyncRelayCommand _twitterCommand;

        /// <summary>
        /// Gets the TwitterCommand.
        /// </summary>
        public AsyncRelayCommand TwitterCommand
        {
            get
            {
                return _twitterCommand
                    ?? (_twitterCommand = new AsyncRelayCommand(
                    async () =>
                    {
                        await Browser.OpenAsync(@"https://twitter.com/Rivolvan_Speaks");
                    }));
            }
        }

        #endregion

        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            PathInfo.Place(nameof(AppPath.Settings), parameter);
            await RefreshCommand.TryExecute();
        }

    }

}
