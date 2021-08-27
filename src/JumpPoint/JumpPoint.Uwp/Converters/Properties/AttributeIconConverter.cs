using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters.Properties
{
    public class AttributeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is FileAttributes attributes)
            {
                switch (attributes)
                {
                    case FileAttributes.Archive:
                    case FileAttributes.Compressed:
                    case FileAttributes.Directory:
                    case FileAttributes.Encrypted:
                    case FileAttributes.Hidden:
                    case FileAttributes.ReadOnly:
                    case FileAttributes.System:
                    case FileAttributes.Temporary:
                        return new Uri($@"ms-appx://JumpPoint.Uwp/Assets/Icons/Attributes/{attributes.Humanize()}.png");

                    case FileAttributes.Normal:
                    case 0:
                        return new Uri($@"ms-appx://JumpPoint.Uwp/Assets/Icons/Attributes/Normal.png");

                    case FileAttributes.Device:
                    case FileAttributes.IntegrityStream:
                    case FileAttributes.NoScrubData:
                    case FileAttributes.NotContentIndexed:
                    case FileAttributes.Offline:
                    case FileAttributes.ReparsePoint:
                    case FileAttributes.SparseFile:
                    default:
                        break;
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
