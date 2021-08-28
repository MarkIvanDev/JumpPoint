using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using NittyGritty.Models;
using NittyGritty.Commands;
using JumpPoint.ViewModels.Helpers;
using JumpPoint.Platform.Models.Extensions;
using Humanizer;
using NittyGritty.Services;

namespace JumpPoint.ViewModels
{
    public class DashboardViewModel : ShellContextViewModelBase
    {

        public DashboardViewModel(IShortcutService shortcutService) : base(shortcutService)
        {
            
        }

        public override bool HasCustomGrouping => true;

        public Collection<ShellItem> QuickLinks { get; } = new Collection<ShellItem>
        {
            new ShellItem { Content = nameof(AppPath.Favorites), Key = ViewModelKeys.Favorites, Tag = AppPath.Favorites },
            new ShellItem { Content = nameof(AppPath.Workspaces), Key = ViewModelKeys.Workspaces, Tag = AppPath.Workspaces },
            new ShellItem { Content = nameof(AppPath.Drives), Key = ViewModelKeys.Drives, Tag = AppPath.Drives },
            new ShellItem { Content = AppPath.CloudDrives.Humanize(), Key = ViewModelKeys.CloudDrives, Tag = AppPath.CloudDrives },
            new ShellItem { Content = AppPath.AppLinks.Humanize(), Key = ViewModelKeys.AppLinks, Tag = AppPath.AppLinks }
        };

        #region Shell Integration
        
        protected override async Task Refresh(CancellationToken token)
        {
            Items.Clear();
            var items = new List<JumpPointItem>();

            var favorites = await DashboardService.GetFavorites();
            items.AddRange(favorites);
            token.ThrowIfCancellationRequested();

            var userFolders = await DashboardService.GetUserFolders(false);
            items.AddRange(userFolders);
            token.ThrowIfCancellationRequested();

            var systemFolders = await DashboardService.GetSystemFolders(false);
            items.AddRange(systemFolders);
            token.ThrowIfCancellationRequested();

            Items.ReplaceRange(items);

            for (int i = 0; i < items.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(items[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }
        
        #endregion

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            base.LoadState(parameter, state);
            PathInfo.Place(nameof(AppPath.Dashboard), parameter);
            await RefreshCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
        }
    }
}
