using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class AttributesDetails : UserControl
    {
        public AttributesDetails()
        {
            this.InitializeComponent();
        }

        public IReadOnlyCollection<FileAttributes> Attributes { get; } = new ReadOnlyCollection<FileAttributes>(new List<FileAttributes>
        {
            FileAttributes.Normal,
            FileAttributes.ReadOnly,
            FileAttributes.Hidden,
            FileAttributes.System,
            FileAttributes.Directory,
            FileAttributes.Archive,
            FileAttributes.Temporary,
            FileAttributes.Compressed,
            FileAttributes.Encrypted,
        });


        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(AttributesDetails), new PropertyMetadata(null));


    }
}
