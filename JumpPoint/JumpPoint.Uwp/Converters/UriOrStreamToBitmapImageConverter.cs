using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Extensions;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace JumpPoint.Uwp.Converters
{
    public class UriOrStreamToBitmapImageConverter : IValueConverter
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var bitmap = new BitmapImage();
            bitmap.DecodePixelType = DecodePixelType.Logical;
            if (Height != 0)
            {
                bitmap.DecodePixelHeight = Height;
            }
            if (Width != 0)
            {
                bitmap.DecodePixelWidth = Width;
            }

            if (value is Uri uri)
            {
                bitmap.UriSource = uri;
                return bitmap;
            }
            else if (value is Stream stream)
            {
                stream.Position = 0;
                bitmap.SetSource(stream.AsRandomAccessStream());
                return bitmap;
            }
            else if (value is byte[] bytes)
            {
                var ms = bytes.ToMemoryStream();
                ms.Position = 0;
                bitmap.SetSource(ms.AsRandomAccessStream());
                return bitmap;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
