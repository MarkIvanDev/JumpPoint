using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using NittyGritty.Services;

namespace JumpPoint.ViewModels
{
    public class FavoritesViewModel : ShellContextViewModelBase
    {

        public FavoritesViewModel(IShortcutService shortcutService) : base(shortcutService)
        {

        }

        protected override async Task Refresh(CancellationToken token)
        {
            // Grouped by Jump Point Item Type
            var favorites = await DashboardService.GetFavorites();
            Items.ReplaceRange(favorites);

            for (int i = 0; i < favorites.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(favorites[i]);
                ProgressInfo.Update(favorites.Count, i + 1, string.Empty);
            }
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            base.LoadState(parameter, state);
            PathInfo.Place(nameof(AppPath.Favorites), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
