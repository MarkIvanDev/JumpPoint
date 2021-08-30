using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters
{
    public class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return AreValuesEqual(value, Value, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        public object Value { get; set; }

        private bool AreValuesEqual(object value1, object value2, bool convertType)
        {
            if (value1 == value2)
            {
                return true;
            }

            if (value1 != null && value2 != null && convertType)
            {
                // Try the conversion in both ways:
                return ConvertTypeEquals(value1, value2) || ConvertTypeEquals(value2, value1);
            }

            return false;
        }

        private bool ConvertTypeEquals(object value1, object value2)
        {
            // Let's see if we can convert:
            if (value2 is Enum)
            {
                value1 = ConvertToEnum(value2.GetType(), value1);
            }
            else
            {
                value1 = System.Convert.ChangeType(value1, value2.GetType(), CultureInfo.InvariantCulture);
            }

            return value2.Equals(value1);
        }

        private object ConvertToEnum(Type enumType, object value)
        {
            try
            {
                return Enum.IsDefined(enumType, value) ? Enum.ToObject(enumType, value) : null;
            }
            catch
            {
                return null;
            }
        }

    }
}
