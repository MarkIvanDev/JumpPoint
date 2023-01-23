using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Platform.Theme;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters
{
    public class AppThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is AppTheme theme ? (ElementTheme)theme : ElementTheme.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
