using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Hosted;
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

namespace JumpPoint.Uwp.Hosted
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ManualAppLinkPickerPage : NGPage
    {
        public ManualAppLinkPickerPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModelLocator.Instance.GetUniqueInstance<ManualAppLinkPickerViewModel>();
        }

        public ManualAppLinkPickerViewModel ViewModel => DataContext as ManualAppLinkPickerViewModel;
    }
}
