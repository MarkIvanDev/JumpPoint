using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JumpPoint.Platform;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Uwp;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WslPage : NGPage, INotifyPropertyChanged
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
            if (e.Parameter is TabParameter parameter)
            {
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.WSL, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

    }
}
