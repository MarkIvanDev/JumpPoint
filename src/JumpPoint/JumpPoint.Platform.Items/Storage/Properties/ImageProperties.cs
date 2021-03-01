using System;
using System.Collections.Generic;
using JumpPoint.Platform.Items.Storage.Properties.Image;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class ImageProperties : ObservableObject
    {

        private string _imageID;

        public string ImageID
        {
            get { return _imageID; }
            set { Set(ref _imageID, value); }
        }

        private BitDepth? _bitDepth;

        public BitDepth? BitDepth
        {
            get { return _bitDepth; }
            set { Set(ref _bitDepth, value); }
        }

        private ColorSpace? _colorSpace;

        public ColorSpace? ColorSpace
        {
            get { return _colorSpace; }
            set { Set(ref _colorSpace, value); }
        }

        private double? _compressionLevel;

        public double? CompressionLevel
        {
            get { return _compressionLevel; }
            set { Set(ref _compressionLevel, value); }
        }

        private Compression? _compression;

        public Compression? Compression
        {
            get { return _compression; }
            set { Set(ref _compression, value); }
        }

        private string _dimensions;

        public string Dimensions
        {
            get { return _dimensions; }
            set { Set(ref _dimensions, value); }
        }

        private uint? _width;

        public uint? Width
        {
            get { return _width; }
            set { Set(ref _width, value); }
        }

        private uint? _height;

        public uint? Height
        {
            get { return _height; }
            set { Set(ref _height, value); }
        }

        private double? _horizontalResolution;

        public double? HorizontalResolution
        {
            get { return _horizontalResolution; }
            set { Set(ref _horizontalResolution, value); }
        }

        private double? _verticalResolution;

        public double? VerticalResolution
        {
            get { return _verticalResolution; }
            set { Set(ref _verticalResolution, value); }
        }

        private ResolutionUnit? _resolutionUnit;

        public ResolutionUnit? ResolutionUnit
        {
            get { return _resolutionUnit; }
            set { Set(ref _resolutionUnit, value); }
        }

        public static ImageProperties Extract(IDictionary<string, object> props)
        {
            var imageProperties = new ImageProperties()
            {
                ImageID = (string)props[Key.ImageID],
                BitDepth = props[Key.BitDepth] is uint bd ? (BitDepth?)bd : null,
                ColorSpace = props[Key.ColorSpace] is ushort cs ? (ColorSpace?)cs : null,
                CompressionLevel = props[Key.CompressionLevel] as double?,
                Compression = props[Key.Compression] is ushort c ? (Compression?)c : null,

                Dimensions = (string)props[Key.Dimensions],
                Width = props[Key.Width] as uint?,
                Height = props[Key.Height] as uint?,
                HorizontalResolution = props[Key.HorizontalResolution] as double?,
                VerticalResolution = props[Key.VerticalResolution] as double?,
                ResolutionUnit = props[Key.ResolutionUnit] is short ru ? (ResolutionUnit?)ru : null
            };
            return imageProperties;
        }

        public static class Key
        {
            public static string ImageID => "System.Image.ImageID";
            public static string BitDepth => "System.Image.BitDepth";
            public static string ColorSpace => "System.Image.ColorSpace";
            public static string CompressionLevel => "System.Image.CompressedBitsPerPixel";
            public static string Compression => "System.Image.Compression";

            public static string Dimensions => "System.Image.Dimensions";
            public static string Width => "System.Image.HorizontalSize";
            public static string Height => "System.Image.VerticalSize";
            public static string HorizontalResolution => "System.Image.HorizontalResolution";
            public static string VerticalResolution => "System.Image.VerticalResolution";
            public static string ResolutionUnit => "System.Image.ResolutionUnit";

            public static IEnumerable<string> All()
            {
                yield return ImageID;
                yield return BitDepth;
                yield return ColorSpace;
                yield return CompressionLevel;
                yield return Compression;
                
                yield return Dimensions;
                yield return Width;
                yield return Height;
                yield return HorizontalResolution;
                yield return VerticalResolution;
                yield return ResolutionUnit;
            }
        }

    }

    public enum PhotoOrientation
    {
        //
        // Summary:
        //     An orientation flag is not set.
        Unspecified = 0,
        //
        // Summary:
        //     No rotation needed. The photo can be displayed using its current orientation.
        Normal = 1,
        //
        // Summary:
        //     Flip the photo horizontally.
        FlipHorizontal = 2,
        //
        // Summary:
        //     Rotate the photo 180 degrees.
        Rotate180 = 3,
        //
        // Summary:
        //     Flip the photo vertically.
        FlipVertical = 4,
        //
        // Summary:
        //     Rotate the photo counter-clockwise 90 degrees and then flip it horizontally.
        Transpose = 5,
        //
        // Summary:
        //     Rotate the photo counter-clockwise 270 degrees.
        Rotate270 = 6,
        //
        // Summary:
        //     Rotate the photo counter-clockwise 270 degrees and then flip it horizontally.
        Transverse = 7,
        //
        // Summary:
        //     Rotate the photo counter-clockwise 90 degrees.
        Rotate90 = 8
    }

}