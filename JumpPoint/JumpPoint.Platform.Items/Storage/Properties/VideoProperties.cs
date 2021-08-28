using System;
using System.Collections.Generic;
using JumpPoint.Platform.Items.Storage.Properties.Video;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class VideoProperties : ObservableObject
    {   

        private uint? _dataRate;

        public uint? DataRate
        {
            get { return _dataRate; }
            set { Set(ref _dataRate, value); }
        }

        private string _compression;

        public string Compression
        {
            get { return _compression; }
            set { Set(ref _compression, value); }
        }

        private string[] _directors;

        public string[] Directors
        {
            get { return _directors; }
            set { Set(ref _directors, value); }
        }

        private uint? _frameRate;

        public uint? FrameRate
        {
            get { return _frameRate; }
            set { Set(ref _frameRate, value); }
        }

        private uint? _frameWidth;

        public uint? FrameWidth
        {
            get { return _frameWidth; }
            set { Set(ref _frameWidth, value); }
        }

        private uint? _frameHeight;

        public uint? FrameHeight
        {
            get { return _frameHeight; }
            set { Set(ref _frameHeight, value); }
        }

        private uint? _horizontalAspectRatio;

        public uint? HorizontalAspectRatio
        {
            get { return _horizontalAspectRatio; }
            set { Set(ref _horizontalAspectRatio, value); }
        }

        private uint? _verticalAspectRatio;

        public uint? VerticalAspectRatio
        {
            get { return _verticalAspectRatio; }
            set { Set(ref _verticalAspectRatio, value); }
        }

        private Orientation? _orientation;

        public Orientation? Orientation
        {
            get { return _orientation; }
            set { Set(ref _orientation, value); }
        }

        private uint? _sampleSize;

        public uint? SampleSize
        {
            get { return _sampleSize; }
            set { Set(ref _sampleSize, value); }
        }

        private uint? _totalBitrate;

        public uint? TotalBitrate
        {
            get { return _totalBitrate; }
            set { Set(ref _totalBitrate, value); }
        }

        private uint? _fourCC;

        public uint? FourCC
        {
            get { return _fourCC; }
            set { Set(ref _fourCC, value); }
        }

        private bool? _isSpherical;

        public bool? IsSpherical
        {
            get { return _isSpherical; }
            set { Set(ref _isSpherical, value); }
        }

        private bool? _isStereo;

        public bool? IsStereo
        {
            get { return _isStereo; }
            set { Set(ref _isStereo, value); }
        }

        private string _streamName;

        public string StreamName
        {
            get { return _streamName; }
            set { Set(ref _streamName, value); }
        }

        private ushort? _streamNumber;

        public ushort? StreamNumber
        {
            get { return _streamNumber; }
            set { Set(ref _streamNumber, value); }
        }

        private bool? _transcodedForSync;

        public bool? TranscodedForSync
        {
            get { return _transcodedForSync; }
            set { Set(ref _transcodedForSync, value); }
        }

        public static VideoProperties Extract(IDictionary<string, object> props)
        {
            var videoProperties = new VideoProperties()
            {
                DataRate = props[Key.DataRate] as uint?,
                Compression = (string)props[Key.Compression],
                Directors = props[Key.Directors] as string[] ?? new string[0],
                FrameRate = props[Key.FrameRate] as uint?,
                FrameWidth = props[Key.FrameWidth] as uint?,
                FrameHeight = props[Key.FrameHeight] as uint?,
                HorizontalAspectRatio = props[Key.HorizontalAspectRatio] as uint?,
                VerticalAspectRatio = props[Key.VerticalAspectRatio] as uint?,
                Orientation = props[Key.Orientation] is uint o ? (Orientation?)o : null,
                SampleSize = props[Key.SampleSize] as uint?,
                TotalBitrate = props[Key.TotalBitrate] as uint?,

                FourCC = props[Key.FourCC] as uint?,
                IsSpherical = props[Key.IsSpherical] as bool?,
                IsStereo = props[Key.IsStereo] as bool?,
                StreamName = (string)props[Key.StreamName],
                StreamNumber = props[Key.StreamNumber] as ushort?,
                TranscodedForSync = props[Key.TranscodedForSync] as bool?
            };
            return videoProperties;
        }

        public static class Key
        {
            public static string DataRate => "System.Video.EncodingBitrate";
            public static string Compression => "System.Video.Compression";
            public static string Directors => "System.Video.Director";
            public static string FrameRate => "System.Video.FrameRate";
            public static string FrameWidth => "System.Video.FrameWidth";
            public static string FrameHeight => "System.Video.FrameHeight";
            public static string HorizontalAspectRatio => "System.Video.HorizontalAspectRatio";
            public static string VerticalAspectRatio => "System.Video.VerticalAspectRatio";
            public static string Orientation => "System.Video.Orientation";
            public static string SampleSize => "System.Video.SampleSize";
            public static string TotalBitrate => "System.Video.TotalBitrate";

            public static string FourCC => "System.Video.FourCC";
            public static string IsSpherical => "System.Video.IsSpherical";
            public static string IsStereo => "System.Video.IsStereo";
            public static string StreamName => "System.Video.StreamName";
            public static string StreamNumber => "System.Video.StreamNumber";
            public static string TranscodedForSync => "System.Video.TranscodedForSync";

            public static IEnumerable<string> All()
            {
                yield return DataRate;
                yield return Compression;
                yield return Directors;
                yield return FrameRate;
                yield return FrameWidth;
                yield return FrameHeight;
                yield return HorizontalAspectRatio;
                yield return VerticalAspectRatio;
                yield return Orientation;
                yield return SampleSize;
                yield return TotalBitrate;

                yield return FourCC;
                yield return IsSpherical;
                yield return IsStereo;
                yield return StreamName;
                yield return StreamNumber;
                yield return TranscodedForSync;
            }
        }

    }

}