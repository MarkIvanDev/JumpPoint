using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JumpPoint.Platform.Items;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace JumpPoint.Uwp.Controls
{
    public sealed partial class DriveDetails : UserControl
    {
        public DriveDetails()
        {
            this.InitializeComponent();
        }


        public DriveBase Drive
        {
            get { return (DriveBase)GetValue(DriveProperty); }
            set { SetValue(DriveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Drive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DriveProperty =
            DependencyProperty.Register("Drive", typeof(DriveBase), typeof(DriveDetails), new PropertyMetadata(null));



        public bool IsInDetailsPane
        {
            get { return (bool)GetValue(IsInDetailsPaneProperty); }
            set { SetValue(IsInDetailsPaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInDetailsPane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInDetailsPaneProperty =
            DependencyProperty.Register("IsInDetailsPane", typeof(bool), typeof(DriveDetails), new PropertyMetadata(false));


    }
}
