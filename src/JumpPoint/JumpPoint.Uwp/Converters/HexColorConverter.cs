using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Extensions;
using Windows.UI.Xaml.Data;
using Xamarin.Essentials;

namespace JumpPoint.Uwp.Converters
{
    public class HexColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = value?.ToString();
            return string.IsNullOrWhiteSpace(color) ?
                Color.Transparent :
                ColorConverters.FromHex(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Color color)
            {
                return color.ToHex();
            }
            else
            {
                return Color.Transparent.ToHex();
            }
        }
    }
}
