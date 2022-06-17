using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Xaml.Interactivity;
using NittyGritty.Uwp.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace JumpPoint.Uwp.Interactivity.Actions
{
    public class DoubleClickCommandAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            if (parameter is DoubleTappedRoutedEventArgs args)
            {
                var parentContainer = XamlHelper.FindParent<SelectorItem>(args.OriginalSource as DependencyObject);
                if (parentContainer != null && Command != null)
                {
                    Command.Execute(parentContainer.DataContext);
                }
            }
            return true;
        }


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DoubleClickCommandAction), new PropertyMetadata(null));


    }
}
