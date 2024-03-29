﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Models;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class CloudViewModel : ShellContextViewModelBase
    {

        public CloudViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {

        }

        private CloudStorageProvider _provider;

        public CloudStorageProvider Provider
        {
            get { return _provider; }
            set { Set(ref _provider, value); }
        }

        protected override async Task Refresh(CancellationToken token)
        {
            var drives = await CloudStorageService.GetDrives(Provider);
            Items.AddRange(drives);
            token.ThrowIfCancellationRequested();

            for (int i = 0; i < drives.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(drives[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            string path = null;
            if (parameter?.Parameter is string queryString)
            {
                var args = QueryString.Parse(queryString);
                path = args[nameof(PathInfo.Path)];
                Provider = CloudStorageService.GetProvider(path);
            }
            else if (state != null)
            {
                Provider = RestoreStateItem(state, nameof(Provider), CloudStorageProvider.Unknown);
                path = $@"cloud:\{Provider}";
            }

            PathInfo.Place(path, parameter);
            PathInfo.Tag = Provider;
            await RefreshCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
            state[nameof(Provider)] = Provider.ToString();
        }

    }
}
