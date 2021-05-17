using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.UserProfile;

namespace JumpPoint.Platform.Extensions
{
    public static class PhotoTool
    {
        // This file format does not support the requested operation; for example, metadata or thumbnails.
        const int WINCODEC_ERR_UNSUPPORTEDOPERATION = unchecked((int)0x88982F81);

        private static readonly string WALLPAPER_FOLDER;

        static PhotoTool()
        {
            WALLPAPER_FOLDER = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Wallpaper");
            Directory.CreateDirectory(WALLPAPER_FOLDER);
        }

        public static async Task Rotate(ToolPayload payload, BitmapRotation rotation)
        {
            var file = await ToolManager.GetFile(payload);
            if (file is null) return;

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (var memory = new InMemoryRandomAccessStream())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(memory, decoder);
                encoder.BitmapTransform.Rotation = rotation;

                await Flush(encoder);
                await Save(memory, stream);
            }
        }

        public static async Task RotateLeft(ToolPayload payload)
        {
            await Rotate(payload, BitmapRotation.Clockwise270Degrees);
        }

        public static async Task RotateRight(ToolPayload payload)
        {
            await Rotate(payload, BitmapRotation.Clockwise90Degrees);
        }

        public static async Task Flip(ToolPayload payload, BitmapFlip flip)
        {
            var file = await ToolManager.GetFile(payload);
            if (file is null) return;

            using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (var memory = new InMemoryRandomAccessStream())
            {
                var decoder = await BitmapDecoder.CreateAsync(stream);
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(memory, decoder);
                encoder.BitmapTransform.Flip = flip;

                await Flush(encoder);
                await Save(memory, stream);
            }
        }

        public static async Task FlipHorizontal(ToolPayload payload)
        {
            await Flip(payload, BitmapFlip.Horizontal);
        }

        public static async Task FlipVertical(ToolPayload payload)
        {
            await Flip(payload, BitmapFlip.Vertical);
        }

        public static async Task<bool> SetWallpaper(ToolPayload payload)
        {
            if (!UserProfilePersonalizationSettings.IsSupported()) return false;

            var file = await ToolManager.GetFile(payload);
            if (file is null) return false;

            // Cleanup wallpaper folder
            foreach (var item in Directory.EnumerateFiles(WALLPAPER_FOLDER))
            {
                File.Delete(item);
            }

            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Wallpaper", CreationCollisionOption.OpenIfExists);
            var localFile = await file.CopyAsync(folder, file.Name, NameCollisionOption.GenerateUniqueName);
            return await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(localFile);
        }

        public static async Task Flush(BitmapEncoder encoder)
        {
            // Attempt to generate a new thumbnail to reflect any rotation operation.
            encoder.IsThumbnailGenerated = true;

            try
            {
                await encoder.FlushAsync();
            }
            catch (Exception err)
            {
                switch (err.HResult)
                {
                    case WINCODEC_ERR_UNSUPPORTEDOPERATION:
                        // If the encoder does not support writing a thumbnail, then try again
                        // but disable thumbnail generation.
                        encoder.IsThumbnailGenerated = false;
                        break;
                    default:
                        throw;
                }
            }

            if (encoder.IsThumbnailGenerated == false)
            {
                await encoder.FlushAsync();
            }
        }

        public static async Task Save(IRandomAccessStream source, IRandomAccessStream destination)
        {
            source.Seek(0);
            destination.Seek(0);
            destination.Size = 0;
            await RandomAccessStream.CopyAsync(source, destination);
        }

    }
}
