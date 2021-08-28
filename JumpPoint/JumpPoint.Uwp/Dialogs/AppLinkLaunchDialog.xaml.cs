using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JumpPoint.Platform.Items;
using JumpPoint.ViewModels.Dialogs;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Dialogs
{
    public sealed partial class AppLinkLaunchDialog : ContentDialog
    {
        public AppLinkLaunchDialog()
        {
            this.InitializeComponent();
        }

        public AppLinkLaunchViewModel ViewModel => DataContext as AppLinkLaunchViewModel;

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchType = AppLinkLaunchTypes.Uri;
            this.Hide();
        }

        private void btnLaunchForResults_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchType = AppLinkLaunchTypes.UriForResults;
            this.Hide();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LaunchType = AppLinkLaunchTypes.None;
            this.Hide();
        }
    }
}
