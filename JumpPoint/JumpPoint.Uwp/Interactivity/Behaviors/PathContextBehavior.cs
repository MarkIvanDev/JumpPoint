using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using NittyGritty.Uwp.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using CommandBarFlyout = Microsoft.UI.Xaml.Controls.CommandBarFlyout;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class PathContextBehavior : Behavior<ListViewBase>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.RightTapped += OnRightTapped;
            }
        }

        private void OnRightTapped(object sender, RightTappedRoutedEventArgs args)
        {
            var parentContainer = XamlHelper.FindParent<SelectorItem>(args.OriginalSource as DependencyObject);
            if (parentContainer is null)
            {
                var options = new FlyoutShowOptions()
                {
                    ShowMode = FlyoutShowMode.Standard,
                    Position = args.GetPosition((FrameworkElement)sender)
                };
                Flyout.ShowAt((FrameworkElement)sender, options);
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.RightTapped -= OnRightTapped;
            }
        }



        public CommandBarFlyout Flyout
        {
            get { return (CommandBarFlyout)GetValue(FlyoutProperty); }
            set { SetValue(FlyoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Flyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlyoutProperty =
            DependencyProperty.Register("Flyout", typeof(CommandBarFlyout), typeof(PathContextBehavior), new PropertyMetadata(null));


    }
}
