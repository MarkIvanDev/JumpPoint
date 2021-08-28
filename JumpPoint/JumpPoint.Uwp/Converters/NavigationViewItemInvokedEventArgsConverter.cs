using JumpPoint.ViewModels;
using Microsoft.UI.Xaml.Controls;
using NittyGritty.Models;
using NittyGritty.Uwp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters
{
    public class NavigationViewItemInvokedEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is NavigationViewItemInvokedEventArgs args)
            {
                if (args.IsSettingsInvoked)
                {
                    return new ShellItem()
                    {
                        Type = ShellItemType.Settings,
                        Key = ViewModelKeys.Settings
                    };
                }
                else
                {
                    return args.InvokedItemContainer.Tag as ShellItem;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public enum NavigationViewItemStrategy
    {
        ShellItem = 0,
        Extension = 1,
        Other = 9
    }
}
