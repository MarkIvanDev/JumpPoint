using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using JumpPoint.Platform.Services;
using NittyGritty.Services.Core;

namespace JumpPoint.ViewModels
{
    public class WorkspacesViewModel : ShellContextViewModelBase
    {

        public WorkspacesViewModel(IShortcutService shortcutService, AppSettings appSettings) : base(shortcutService, appSettings)
        {

        }

        protected override async Task Refresh(CancellationToken token)
        {
            var workspaces = await WorkspaceService.GetWorkspaces();
            Items.Clear();
            Items.AddRange(workspaces);

            for (int i = 0; i < workspaces.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(workspaces[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            base.LoadState(parameter, state);
            PathInfo.Place(nameof(AppPath.Workspaces), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
