using System.ComponentModel;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Views
{
    public sealed partial class FolderPage : PageBase, INotifyPropertyChanged
    {
        public FolderPage()
        {
            this.InitializeComponent();
        }

        public FolderViewModel ViewModel => DataContext as FolderViewModel;

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
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.Folder, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

    }
}
