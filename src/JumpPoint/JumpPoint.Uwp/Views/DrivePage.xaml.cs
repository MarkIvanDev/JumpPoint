using JumpPoint.Platform;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Uwp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DrivePage : NGPage, INotifyPropertyChanged
    {
        public DrivePage()
        {
            this.InitializeComponent();
        }

        public DriveViewModel ViewModel => DataContext as DriveViewModel;

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
                this.DataContext = ViewModelLocator.Instance.GetContext(AppPath.Drive, parameter.TabKey);
                RaisePropertyChanged(nameof(ViewModel));
            }
            base.OnNavigatedTo(e);
        }

    }
}
