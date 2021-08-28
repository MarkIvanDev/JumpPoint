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
    public sealed partial class DrmPropertiesControl : UserControl
    {
        public DrmPropertiesControl()
        {
            this.InitializeComponent();
        }


        public DrmProperties Drm
        {
            get { return (DrmProperties)GetValue(DrmProperty); }
            set { SetValue(DrmProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Drm.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DrmProperty =
            DependencyProperty.Register("Drm", typeof(DrmProperties), typeof(DrmPropertiesControl), new PropertyMetadata(null));


    }
}
