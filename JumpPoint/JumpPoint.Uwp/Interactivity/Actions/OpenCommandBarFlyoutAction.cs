using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;
using NittyGritty.Uwp.Helpers;

namespace JumpPoint.Uwp.Interactivity.Actions
{
    public class OpenCommandBarFlyoutAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            if(parameter is TappedRoutedEventArgs args)
            {
                var parentContainer = XamlHelper.FindParent<SelectorItem>(args.OriginalSource as DependencyObject);
                if(parentContainer != null && parentContainer.IsSelected && CommandBarFlyout != null)
                {
                    var options = new FlyoutShowOptions()
                    {
                        ShowMode = ShowMode,
                        Placement = FlyoutPlacementMode.BottomEdgeAlignedLeft
                    };
                    CommandBarFlyout.ShowAt(parentContainer, options);
                }
            }
            
            return true;
        }


        public CommandBarFlyout CommandBarFlyout
        {
            get { return (CommandBarFlyout)GetValue(CommandBarFlyoutProperty); }
            set { SetValue(CommandBarFlyoutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandBarFlyout.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandBarFlyoutProperty =
            DependencyProperty.Register("CommandBarFlyout", typeof(CommandBarFlyout), typeof(OpenCommandBarFlyoutAction), new PropertyMetadata(null));



        public FlyoutShowMode ShowMode
        {
            get { return (FlyoutShowMode)GetValue(ShowModeProperty); }
            set { SetValue(ShowModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowModeProperty =
            DependencyProperty.Register("ShowMode", typeof(FlyoutShowMode), typeof(OpenCommandBarFlyoutAction), new PropertyMetadata(FlyoutShowMode.Auto));


    }
}
