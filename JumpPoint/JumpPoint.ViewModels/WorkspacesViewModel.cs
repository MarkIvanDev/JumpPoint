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
            Items.AddRange(workspaces);
            token.ThrowIfCancellationRequested();

            for (int i = 0; i < workspaces.Count; i++)
            {
                token.ThrowIfCancellationRequested();
                await JumpPointService.Load(workspaces[i]);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
        }

        protected override async Task Initialize(TabParameter parameter, Dictionary<string, object> state)
        {
            PathInfo.Place(nameof(AppPath.Workspaces), parameter);
            await RefreshCommand.TryExecute();
        }

    }
}
