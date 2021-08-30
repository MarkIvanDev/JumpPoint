using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class FontFamilyBehavior : Behavior<TextBlock>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.FontFamily = FontFamily ?? FontFamily.XamlAutoFontFamily;
            }
        }



        public FontFamily FontFamily
        {
            get { return (FontFamily)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FontFamily.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(FontFamily), typeof(FontFamilyBehavior), new PropertyMetadata(FontFamily.XamlAutoFontFamily, OnFontFamilyChanged));

        private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FontFamilyBehavior behavior && behavior.AssociatedObject != null)
            {
                behavior.AssociatedObject.FontFamily = (FontFamily)e.NewValue;
            }
        }
    }
}
