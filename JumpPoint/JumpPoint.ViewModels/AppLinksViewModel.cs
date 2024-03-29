﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
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

        private IList<AppLinkProvider> _providers;

        public IList<AppLinkProvider> Providers
        {
            get { return _providers; }
            set { Set(ref _providers, value); }
        }

        private AsyncRelayCommand<AppLinkProvider> _Provider;
        public AsyncRelayCommand<AppLinkProvider> ProviderCommand => _Provider ?? (_Provider = new AsyncRelayCommand<AppLinkProvider>(
            async provider =>
            {
                var appLinkInfo = await AppLinkProviderService.Pick(provider);
                if (appLinkInfo != null)
                {
                    var appLink = await AppLinkService.Create(appLinkInfo);
                    Items.Add(appLink);
                }
            }));

        protected override async Task Refresh(CancellationToken token)
        {
            var providers = await AppLinkProviderService.GetProviders();
            Providers = providers.Where(p => p.IsAvailable && p.IsEnabled).ToList();
            token.ThrowIfCancellationRequested();

            var appLinks = await AppLinkService.GetAppLinks();
            Items.AddRange(appLinks);
            token.ThrowIfCancellationRequested();

            for (int i = 0; i < appLinks.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(appLinks[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            PathInfo.Place(AppPath.AppLinks.Humanize(), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
