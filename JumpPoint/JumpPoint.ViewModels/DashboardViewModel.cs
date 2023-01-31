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
using JumpPoint.Platform.Models.Extensions;
using Humanizer;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class DashboardViewModel : ShellContextViewModelBase
    {

        public DashboardViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
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

        protected override async Task Refresh(CancellationToken token)
        {
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

            Items.AddRange(items);

            for (int i = 0; i < items.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(items[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }
        
        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            PathInfo.Place(nameof(AppPath.Dashboard), parameter);
            await RefreshCommand.TryExecute();
        }
    }
}
