using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Uwp.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace JumpPoint.Uwp.Converters.EventArgs
{
    public class ItemDoubleClickEventArgsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DoubleTappedRoutedEventArgs args && parameter is string tabKey)
            {
                var parentContainer = XamlHelper.FindParent<SelectorItem>(args.OriginalSource as DependencyObject);
                if (parentContainer?.Content is JumpPointItem item)
                {
                    return new OpenItemParameter(ViewModelLocator.Instance.GetNavigationHelper(tabKey), item);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
