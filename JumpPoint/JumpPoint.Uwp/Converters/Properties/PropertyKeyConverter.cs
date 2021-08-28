using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters.Properties
{
    public class PropertyKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is int count && parameter is string key)
            {
                return $"{key.ToQuantity(count, ShowQuantityAs.None)}:";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
