using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items.Storage;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters
{
    public class HiddenOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is FileAttributes atts)
            {
                return (atts & FileAttributes.Hidden) == FileAttributes.Hidden ? 0.5 : 1.0;
            }
            else if (value is StorageItemBase item && item.Attributes.HasValue)
            {
                return (item.Attributes.Value & FileAttributes.Hidden) == FileAttributes.Hidden ? 0.5 : 1.0;
            }
            else
            {
                return 1.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
