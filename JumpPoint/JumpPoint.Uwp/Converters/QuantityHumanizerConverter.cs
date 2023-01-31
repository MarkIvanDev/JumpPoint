using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters
{
    public class QuantityHumanizerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var input = parameter?.ToString() ?? string.Empty;
            if (value is int intValue)
            {
                return ToQuantityExtensions.ToQuantity(input, intValue, ShowQuantityAs.Numeric);
            }
            else if (value is long longValue)
            {
                return ToQuantityExtensions.ToQuantity(input, longValue, ShowQuantityAs.Numeric);
            }
            else if (value is ulong ulongValue)
            {
                return ulongValue == 1
                    ? ToQuantityExtensions.ToQuantity(input, 1, ShowQuantityAs.Numeric)
                    : $"{ulongValue} {ToQuantityExtensions.ToQuantity(input, 2, ShowQuantityAs.None)}";
            }
            else if (value is double doubleValue)
            {
                return ToQuantityExtensions.ToQuantity(input, doubleValue);
            }
            else
            {
                return $"{value} {input}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
