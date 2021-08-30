using JumpPoint.Extensions.Tools;
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

        public static async Task<ToolResultPayload> Rotate(ToolPayload payload, BitmapRotation rotation)
        {
            try
            {
                var file = await ToolHelper.GetFile(payload);
                if (file is null) return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = "Problem fetching file"
                };

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                using (var memory = new InMemoryRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(memory, decoder);
                    encoder.BitmapTransform.Rotation = rotation;

                    await Flush(encoder);
                    await Save(memory, stream);
                }

                return new ToolResultPayload
                {
                    Result = ToolResult.Successful,
                    Path = payload.Path
                };
            }
            catch (Exception ex)
            {
                return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = ex.Message
                };
            }
        }

        public static async Task<ToolResultPayload> RotateLeft(ToolPayload payload)
        {
            return await Rotate(payload, BitmapRotation.Clockwise270Degrees);
        }

        public static async Task<ToolResultPayload> RotateRight(ToolPayload payload)
        {
            return await Rotate(payload, BitmapRotation.Clockwise90Degrees);
        }

        public static async Task<ToolResultPayload> Flip(ToolPayload payload, BitmapFlip flip)
        {
            try
            {
                var file = await ToolHelper.GetFile(payload);
                if (file is null) return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = "Problem fetching file"
                };

                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                using (var memory = new InMemoryRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var encoder = await BitmapEncoder.CreateForTranscodingAsync(memory, decoder);
                    encoder.BitmapTransform.Flip = flip;

                    await Flush(encoder);
                    await Save(memory, stream);
                }

                return new ToolResultPayload
                {
                    Result = ToolResult.Successful,
                    Path = payload.Path
                };
            }
            catch (Exception ex)
            {
                return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = ex.Message
                };
            }
        }

        public static async Task<ToolResultPayload> FlipHorizontal(ToolPayload payload)
        {
            return await Flip(payload, BitmapFlip.Horizontal);
        }

        public static async Task<ToolResultPayload> FlipVertical(ToolPayload payload)
        {
            return await Flip(payload, BitmapFlip.Vertical);
        }

        public static async Task<ToolResultPayload> SetWallpaper(ToolPayload payload)
        {
            try
            {
                if (!UserProfilePersonalizationSettings.IsSupported()) return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = "Device does not support setting the wallpaper"
                };

                var file = await ToolHelper.GetFile(payload);
                if (file is null) return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = "Problem fetching file"
                };

                // Cleanup wallpaper folder
                foreach (var item in Directory.EnumerateFiles(WALLPAPER_FOLDER))
                {
                    File.Delete(item);
                }

                var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Wallpaper", CreationCollisionOption.OpenIfExists);
                var localFile = await file.CopyAsync(folder, file.Name, NameCollisionOption.GenerateUniqueName);
                var result = await UserProfilePersonalizationSettings.Current.TrySetWallpaperImageAsync(localFile);
                return result ?
                    new ToolResultPayload
                    {
                        Result = ToolResult.Successful,
                        Path = payload.Path
                    } :
                    new ToolResultPayload
                    {
                        Result = ToolResult.Failed,
                        Path = payload.Path,
                        Message = "Problem setting the wallpaper"
                    };
            }
            catch (Exception ex)
            {
                return new ToolResultPayload
                {
                    Result = ToolResult.Failed,
                    Path = payload.Path,
                    Message = ex.Message
                };
            }
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
