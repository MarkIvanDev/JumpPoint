using System;
using System.Text;
using Windows.UI.Xaml.Data;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Uwp.Converters
{
    public class IsRegularFolderTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is FolderType type && type == FolderType.Regular;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
