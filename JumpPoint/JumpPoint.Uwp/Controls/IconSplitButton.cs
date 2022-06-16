using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace JumpPoint.Uwp.Controls
{
    public sealed class IconSplitButton : SplitButton
    {
        public IconSplitButton()
        {
            this.DefaultStyleKey = typeof(IconSplitButton);
            RegisterPropertyChangedCallback(CommandProperty, OnCommandChanged);
        }

        private void OnCommandChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender.GetValue(dp) is XamlUICommand command)
            {
                IconSource = command.IconSource;
                Content = command.Label;
            }
        }

        public IconSource IconSource
        {
            get { return (IconSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(IconSource), typeof(IconSplitButton), new PropertyMetadata(null));



        public bool IsCompact
        {
            get { return (bool)GetValue(IsCompactProperty); }
            set { SetValue(IsCompactProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCompact.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register("IsCompact", typeof(bool), typeof(IconSplitButton), new PropertyMetadata(false));



        public bool IsPrimaryButtonEnabled
        {
            get { return (bool)GetValue(IsPrimaryButtonEnabledProperty); }
            set { SetValue(IsPrimaryButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPrimaryButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPrimaryButtonEnabledProperty =
            DependencyProperty.Register("IsPrimaryButtonEnabled", typeof(bool), typeof(IconSplitButton), new PropertyMetadata(true));


    }
}
