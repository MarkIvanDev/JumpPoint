using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Storage.Properties.Audio;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class AudioProperties : ObservableObject
    {

        private uint? _bitrate;

        public uint? Bitrate
        {
            get { return _bitrate; }
            set { Set(ref _bitrate, value); }
        }

        private bool? _isVariableBitrate;

        public bool? IsVariableBitrate
        {
            get { return _isVariableBitrate; }
            set { Set(ref _isVariableBitrate, value); }
        }

        private uint? _peakValue;

        public uint? PeakValue
        {
            get { return _peakValue; }
            set { Set(ref _peakValue, value); }
        }

        private ChannelCount? _channelCount;

        public ChannelCount? ChannelCount
        {
            get { return _channelCount; }
            set { Set(ref _channelCount, value); }
        }

        private string _compression;

        public string Compression
        {
            get { return _compression; }
            set { Set(ref _compression, value); }
        }

        private string _format;

        public string Format
        {
            get { return _format; }
            set { Set(ref _format, value); }
        }

        private uint? _sampleRate;

        public uint? SampleRate
        {
            get { return _sampleRate; }
            set { Set(ref _sampleRate, value); }
        }

        private uint? _sampleSize;

        public uint? SampleSize
        {
            get { return _sampleSize; }
            set { Set(ref _sampleSize, value); }
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

        public static AudioProperties Extract(IDictionary<string, object> props)
        {
            var audioProperties = new AudioProperties()
            {
                Bitrate = props[Key.Bitrate] as uint?,
                IsVariableBitrate = props[Key.IsVariableBitrate] as bool?,
                PeakValue = props[Key.PeakValue] as uint?,
                ChannelCount = props[Key.ChannelCount] is uint cc ? (ChannelCount?)cc : null,
                Compression = (string)props[Key.Compression],
                Format = (string)props[Key.Format],
                SampleRate = props[Key.SampleRate] as uint?,
                SampleSize = props[Key.SampleSize] as uint?,
                StreamName = (string)props[Key.StreamName],
                StreamNumber = props[Key.StreamNumber] as ushort?
            };
            return audioProperties;
        }

        public static class Key
        {
            public static string Bitrate => "System.Audio.EncodingBitrate";
            public static string IsVariableBitrate => "System.Audio.IsVariableBitRate";
            public static string PeakValue => "System.Audio.PeakValue";
            public static string ChannelCount => "System.Audio.ChannelCount";
            public static string Compression => "System.Audio.Compression";
            public static string Format => "System.Audio.Format";
            public static string SampleRate => "System.Audio.SampleRate";
            public static string SampleSize => "System.Audio.SampleSize";
            public static string StreamName => "System.Audio.StreamName";
            public static string StreamNumber => "System.Audio.StreamNumber";

            public static IEnumerable<string> All()
            {
                yield return Bitrate;
                yield return IsVariableBitrate;
                yield return PeakValue;
                yield return ChannelCount;
                yield return Compression;
                yield return Format;
                yield return SampleRate;
                yield return SampleSize;
                yield return StreamName;
                yield return StreamNumber;
            }
        }
    }
}
