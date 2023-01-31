using System;
using System.Collections.Generic;
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
    public sealed partial class AttributeIcon : UserControl
    {
        public AttributeIcon()
        {
            this.InitializeComponent();
        }


        public int IconWidth
        {
            get { return (int)GetValue(IconWidthProperty); }
            set { SetValue(IconWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.Register("IconWidth", typeof(int), typeof(AttributeIcon), new PropertyMetadata(0));



        public int IconHeight
        {
            get { return (int)GetValue(IconHeightProperty); }
            set { SetValue(IconHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.Register("IconHeight", typeof(int), typeof(AttributeIcon), new PropertyMetadata(0));



        public FileAttributes? Attribute
        {
            get { return (FileAttributes?)GetValue(AttributeProperty); }
            set { SetValue(AttributeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Attribute.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttributeProperty =
            DependencyProperty.Register("Attribute", typeof(FileAttributes?), typeof(AttributeIcon), new PropertyMetadata(null, OnAttributeChanged));

        private static void OnAttributeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AttributeIcon icon)
            {
                icon.Compute();
            }
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Data.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(AttributeIcon), new PropertyMetadata(null, OnDataChanged));

        private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is AttributeIcon icon)
            {
                icon.Compute();
            }
        }

        private void Compute()
        {
            if (Data != null)
            {
                var att = Attribute ?? FileAttributes.Normal;
                if (Data is IList<FileAttributes> listAttributes)
                {
                    if (listAttributes.All(a => (a & att) == att))
                    {
                        // If all attributes has the specified attribute, return 1.0
                        image.Opacity = 1.0;
                        image.Visibility = Visibility.Visible;
                    }
                    else if (listAttributes.Any(a => (a & att) == att))
                    {
                        // If only some attributes has the specified attribute, return 0.5
                        image.Opacity = 0.5;
                        image.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        // If none of the attributes has the specified attribute, return 0.0
                        image.Opacity = 0.0;
                        image.Visibility = Visibility.Collapsed;
                    }
                    return;
                }
                else if (Data is FileAttributes attribute)
                {
                    if ((attribute & att) == att)
                    {
                        image.Opacity = 1.0;
                        image.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        image.Opacity = 0.0;
                        image.Visibility = Visibility.Collapsed;
                    }
                    return;
                }
            }
            image.Visibility = Visibility.Collapsed;
        }
    }
}
