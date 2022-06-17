using Microsoft.Xaml.Interactivity;
using NittyGritty.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Interactivity.Actions
{
    public class SelectAllAutoSuggestBoxAction : DependencyObject, IAction
    {
        public object Execute(object sender, object parameter)
        {
            if (sender is AutoSuggestBox box)
            {
                var child = XamlHelper.FindChildren<TextBox>(box).FirstOrDefault();
                if (child != null)
                {
                    child.SelectAll();
                }
            }

            return true;
        }
    }
}
