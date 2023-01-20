using System.ComponentModel;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Views
{
    public sealed partial class WorkspacesPage : PageBase, INotifyPropertyChanged
    {
        public WorkspacesPage()
        {
            this.InitializeComponent();
        }

        public WorkspacesViewModel ViewModel => DataContext as WorkspacesViewModel;

        #region INPC

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameter = TabParameter.FromJson(e.Parameter?.ToString());
            if (parameter != null)
            {
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.Workspaces, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

    }
}
