using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xaml.Interactivity;
using NittyGritty.UI.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace JumpPoint.Uwp.Interactivity.Actions
{
    public class TappedSelectionAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            if (parameter is TappedRoutedEventArgs args)
            {
                var parentContainer = XamlHelper.FindParent<SelectorItem>(args.OriginalSource as DependencyObject);
                if (parentContainer is null && AssociatedObject != null)
                {
                    switch (AssociatedObject.SelectionMode)
                    {
                        case ListViewSelectionMode.Single:
                            AssociatedObject.SelectedItem = null;
                            break;

                        case ListViewSelectionMode.Multiple:
                        case ListViewSelectionMode.Extended:
                            AssociatedObject.SelectedItems.Clear();
                            break;

                        case ListViewSelectionMode.None:
                        default:
                            break;
                    }
                }
            }

            return true;
        }


        public ListViewBase AssociatedObject
        {
            get { return (ListViewBase)GetValue(AssociatedObjectProperty); }
            set { SetValue(AssociatedObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AssociatedObject.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssociatedObjectProperty =
            DependencyProperty.Register("AssociatedObject", typeof(ListViewBase), typeof(TappedSelectionAction), new PropertyMetadata(null));


    }
}
