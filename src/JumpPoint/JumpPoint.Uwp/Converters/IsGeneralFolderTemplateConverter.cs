using System;
using System.Text;
using Windows.UI.Xaml.Data;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Uwp.Converters
{
    public class IsGeneralFolderTemplateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is FolderTemplate template && template == FolderTemplate.General;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
