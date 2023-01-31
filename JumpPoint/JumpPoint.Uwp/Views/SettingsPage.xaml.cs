using System.ComponentModel;
using JumpPoint.Platform;
using JumpPoint.Platform.Models;
using JumpPoint.Uwp.Controls;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Views
{
    public sealed partial class SettingsPage : PageBase, INotifyPropertyChanged
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel ViewModel => DataContext as SettingsViewModel;

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
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.Settings, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

        private async void OnRunAtStartupToggled(object sender, RoutedEventArgs e)
        {
            var toggle = (ToggleSwitch)sender;
            if (toggle.IsOn)
            {
                await ServiceLocator.AppSettings.EnableRunAtStartup();
            }
            else
            {
                await ServiceLocator.AppSettings.DisableRunAtStartup();
            }
        }

        public Visibility GetStatusDescriptionVisibility(StartupStatus status)
        {
            switch (status)
            {
                case StartupStatus.Disabled:
                case StartupStatus.Enabled:
                    return Visibility.Collapsed;

                case StartupStatus.DisabledByUser:
                case StartupStatus.DisabledByPolicy:
                case StartupStatus.EnabledByPolicy:
                default:
                    return Visibility.Visible;
            }
        }
    }
}
