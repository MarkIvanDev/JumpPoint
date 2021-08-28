using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Xaml.Interactivity;
using NittyGritty.Utilities;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class NotifyCommandBehavior : Behavior<XamlUICommand>
    {
        private long? token = null;
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                if (AssociatedObject.Command != null)
                {
                    AssociatedObject.Command.CanExecuteChanged += Command_CanExecuteChanged;
                }
                else
                {
                    token = AssociatedObject.RegisterPropertyChangedCallback(XamlUICommand.CommandProperty, OnCommandChanged);
                }
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                if (AssociatedObject.Command != null)
                {
                    AssociatedObject.Command.CanExecuteChanged -= Command_CanExecuteChanged;
                }

                if (token.HasValue)
                {
                    AssociatedObject.UnregisterPropertyChangedCallback(XamlUICommand.CommandProperty, token.Value);
                    token = null;
                }
            }
        }

        private void OnCommandChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (AssociatedObject.Command != null)
            {
                EventUtilities.RegisterEvent<ICommand, EventHandler, EventArgs>(
                    AssociatedObject.Command,
                    h => AssociatedObject.Command.CanExecuteChanged += h,
                    h => AssociatedObject.Command.CanExecuteChanged -= h,
                    h => (o, e) => h(o, e),
                    (subscriber, s, e) => AssociatedObject.NotifyCanExecuteChanged());
            }
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.NotifyCanExecuteChanged();
            }
        }
    }
}
