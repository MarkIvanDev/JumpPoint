using System;
using System.Collections.ObjectModel;
using JumpPoint.Platform.Items;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class AddToWorkspaceViewModel : ObservableObject
    {

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private Collection<SelectableWorkspace> _workspaces;

        public Collection<SelectableWorkspace> Workspaces
        {
            get { return _workspaces; }
            set { Set(ref _workspaces, value); }
        }

    }
}
