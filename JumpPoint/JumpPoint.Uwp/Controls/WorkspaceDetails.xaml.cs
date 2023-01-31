using System;
using System.Linq;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Controls
{
    public sealed partial class WorkspaceDetails : UserControl
    {
        public WorkspaceDetails()
        {
            this.InitializeComponent();
        }


        public Workspace Workspace
        {
            get { return (Workspace)GetValue(WorkspaceProperty); }
            set { SetValue(WorkspaceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Workspace.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WorkspaceProperty =
            DependencyProperty.Register("Workspace", typeof(Workspace), typeof(WorkspaceDetails), new PropertyMetadata(null, OnWorkspaceChanged));

        private static async void OnWorkspaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WorkspaceDetails details)
            {
                await details.RefreshCount();
            }
        }

        public bool IsInDetailsPane
        {
            get { return (bool)GetValue(IsInDetailsPaneProperty); }
            set { SetValue(IsInDetailsPaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInDetailsPane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInDetailsPaneProperty =
            DependencyProperty.Register("IsInDetailsPane", typeof(bool), typeof(WorkspaceDetails), new PropertyMetadata(false));

        private async Task RefreshCount()
        {
            if (Workspace != null)
            {
                var children = await WorkspaceService.GetItems(Workspace.Id);
                Workspace.DriveCount = (ulong)children.Count(i => i.Type == JumpPointItemType.Drive);
                Workspace.FolderCount = (ulong)children.Count(i => i.Type == JumpPointItemType.Folder);
                Workspace.FileCount = (ulong)children.Count(i => i.Type == JumpPointItemType.File);
                Workspace.AppLinkCount = (ulong)children.Count(i => i.Type == JumpPointItemType.AppLink);
            }
        }

    }
}
