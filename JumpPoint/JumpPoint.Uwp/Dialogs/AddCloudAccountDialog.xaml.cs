using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
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
    public sealed partial class AddCloudAccountDialog : ContentDialog
    {
        public AddCloudAccountDialog()
        {
            this.InitializeComponent();
        }

        public AddCloudAccountViewModel ViewModel => DataContext as AddCloudAccountViewModel;

        public string GetTitle(CloudStorageProvider provider)
        {
            return $"Add {provider.Humanize()} Account";
        }
    }
}
