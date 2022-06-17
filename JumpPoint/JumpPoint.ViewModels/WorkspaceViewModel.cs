using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Models;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class WorkspaceViewModel : ShellContextViewModelBase
    {

        public WorkspaceViewModel(IShortcutService shortcutService) : base(shortcutService)
        {

        }

        #region Shell Integration

        protected override async Task Refresh(CancellationToken token)
        {
            Item = await WorkspaceService.GetWorkspaceByName(PathInfo.DisplayName);
            if (!(Item is Workspace workspace))
            {
                throw new OperationCanceledException();
            }
            PathInfo.Tag = workspace.Template;

            // Grouped by Jump Point Item Type
            var items = await WorkspaceService.GetItems(workspace.Id);
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
            string path = null;
            if (parameter is TabParameter tab && tab.Parameter is string queryString)
            {
                var args = QueryString.Parse(queryString);
                path = args[nameof(PathInfo.Path)];
            }
            else if (state != null)
            {
                path = RestoreStateItem<string>(state, nameof(PathInfo.Path));
            }
            
            PathInfo.Place(path, parameter);
            await RefreshCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            base.SaveState(state);
            if (PathInfo.Type == AppPath.Workspace)
            {
                state[nameof(PathInfo.Path)] = PathInfo.Path;
            }
        }
    }
}
