using System;
using System.IO;
using System.Text;
using JumpPoint.Platform.Services;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace JumpPoint.Uwp.Converters
{
    public static class FileThumbnailConverter
    {
        public static ImageSource Convert(string fileType, Stream thumbnail, int height, int width)
        {
            var bitmap = new BitmapImage();
            bitmap.DecodePixelType = DecodePixelType.Logical;
            bitmap.DecodePixelHeight = height > 0 ? height : 0;
            bitmap.DecodePixelWidth = width > 0 ? width : 0;

            var hasBuiltInIcon = StorageService.HasBuiltInIcon(fileType);
            if (hasBuiltInIcon)
            {
                bitmap.UriSource = new Uri($@"ms-appx://JumpPoint.Uwp/Assets/Icons/Files/{fileType.TrimStart('.')}.png");
            }
            else if (thumbnail is null)
            {
                bitmap.UriSource = new Uri(@"ms-appx://JumpPoint.Uwp/Assets/Icons/Items/File.png");
            }
            else
            {
                thumbnail.Position = 0;
                bitmap.SetSource(thumbnail.AsRandomAccessStream());
            }

            return bitmap;
        }
    
    }
}
