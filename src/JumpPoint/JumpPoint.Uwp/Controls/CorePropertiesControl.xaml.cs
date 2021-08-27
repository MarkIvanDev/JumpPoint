using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using JumpPoint.Platform.Items.Storage.Properties;
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
    public sealed partial class CorePropertiesControl : UserControl
    {
        public CorePropertiesControl()
        {
            this.InitializeComponent();
        }


        public CoreProperties Core
        {
            get { return (CoreProperties)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Core.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoreProperty =
            DependencyProperty.Register("Core", typeof(CoreProperties), typeof(CorePropertiesControl), new PropertyMetadata(null));


    }
}
