using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using NittyGritty.Commands;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class AppLinksViewModel : ShellContextViewModelBase
    {

        public AppLinksViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {

        }

        private Collection<AppLinkProvider> _providers;

        public Collection<AppLinkProvider> Providers
        {
            get { return _providers; }
            set { Set(ref _providers, value); }
        }

        private AsyncRelayCommand<AppLinkProvider> _Provider;
        public AsyncRelayCommand<AppLinkProvider> ProviderCommand => _Provider ?? (_Provider = new AsyncRelayCommand<AppLinkProvider>(
            async provider =>
            {
                var appLinkInfo = await AppLinkProviderManager.Pick(provider);
                if (appLinkInfo != null)
                {
                    var appLink = await AppLinkService.Create(appLinkInfo);
                    await RefreshCommand.TryExecute();
                }
            }));

        protected override async Task Refresh(CancellationToken token)
        {
            var providers = await AppLinkProviderManager.GetProviders();
            Providers = new Collection<AppLinkProvider>(providers);

            var appLinks = await AppLinkService.GetAppLinks();
            Items.Clear();
            Items.AddRange(appLinks);

            for (int i = 0; i < appLinks.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(appLinks[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            base.LoadState(parameter, state);
            PathInfo.Place(nameof(AppPath.AppLinks), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
