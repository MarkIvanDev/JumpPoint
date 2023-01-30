using System.ComponentModel;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Uwp.Controls;
using JumpPoint.ViewModels;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Views
{
    public sealed partial class WslPage : PageBase, INotifyPropertyChanged
    {
        public WslPage()
        {
            this.InitializeComponent();
        }

        public WslViewModel ViewModel => DataContext as WslViewModel;

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
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.WSL, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

    }
}
