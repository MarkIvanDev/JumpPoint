using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace JumpPoint.Uwp.Controls
{
    public sealed class IconButton : Button
    {
        public IconButton()
        {
            this.DefaultStyleKey = typeof(IconButton);
            RegisterPropertyChangedCallback(CommandProperty, OnCommandChanged);
        }

        private void OnCommandChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender.GetValue(dp) is XamlUICommand command)
            {
                IconSource = command.IconSource;
                Content = command.Label;
                var tooltip = new StringBuilder();
                tooltip.Append(string.IsNullOrEmpty(command.Description) ? command.Label : command.Description);
                if (command.KeyboardAccelerators.FirstOrDefault() is KeyboardAccelerator ka)
                {
                    tooltip.Append(" (");
                    tooltip.Append(ka.Modifiers != VirtualKeyModifiers.None ? ka.Modifiers.ToString() : string.Empty);
                    tooltip.Append(ka.Modifiers != VirtualKeyModifiers.None ? $"+{ka.Key}" : ka.Key.ToString());
                    tooltip.Append(")");
                }
                ToolTipService.SetToolTip(this, tooltip.ToString());
            }
        }

        public IconSource IconSource
        {
            get { return (IconSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(IconSource), typeof(IconButton), new PropertyMetadata(null));



        public bool IsCompact
        {
            get { return (bool)GetValue(IsCompactProperty); }
            set { SetValue(IsCompactProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCompact.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register("IsCompact", typeof(bool), typeof(IconButton), new PropertyMetadata(false));


    }
}
