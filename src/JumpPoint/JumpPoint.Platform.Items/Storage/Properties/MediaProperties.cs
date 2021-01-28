using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class MediaProperties : ObservableObject
    {

        private string _subtitle;

        public string Subtitle
        {
            get { return _subtitle; }
            set { Set(ref _subtitle, value); }
        }

        private DateTimeOffset? _dateEncoded;

        public DateTimeOffset? DateEncoded
        {
            get { return _dateEncoded; }
            set { Set(ref _dateEncoded, value); }
        }

        private string _dateReleased;

        public string DateReleased
        {
            get { return _dateReleased; }
            set { Set(ref _dateReleased, value); }
        }

        private TimeSpan? _duration;

        public TimeSpan? Duration
        {
            get { return _duration; }
            set { Set(ref _duration, value); }
        }

        private uint? _frameCount;

        public uint? FrameCount
        {
            get { return _frameCount; }
            set { Set(ref _frameCount, value); }
        }

        private IList<string> _producers;

        public IList<string> Producers
        {
            get { return _producers; }
            set { Set(ref _producers, value); }
        }

        private string _publisher;

        public string Publisher
        {
            get { return _publisher; }
            set { Set(ref _publisher, value); }
        }

        private IList<string> _writers;

        public IList<string> Writers
        {
            get { return _writers; }
            set { Set(ref _writers, value); }
        }

        private uint? _year;

        public uint? Year
        {
            get { return _year; }
            set { Set(ref _year, value); }
        }

        private string _seriesName;

        public string SeriesName
        {
            get { return _seriesName; }
            set { Set(ref _seriesName, value); }
        }

        private uint? _seasonNumber;

        public uint? SeasonNumber
        {
            get { return _seasonNumber; }
            set { Set(ref _seasonNumber, value); }
        }

        private uint? _episodeNumber;

        public uint? EpisodeNumber
        {
            get { return _episodeNumber; }
            set { Set(ref _episodeNumber, value); }
        }

        public static MediaProperties Extract(IDictionary<string, object> props)
        {
            var mediaProperties = new MediaProperties()
            {
                Subtitle = (string)props[Key.Subtitle],
                DateEncoded = props[Key.DateEncoded] as DateTimeOffset?,
                DateReleased = (string)props[Key.DateReleased],
                Duration = props[Key.Duration] is ulong d ? new TimeSpan?(TimeSpan.FromMilliseconds(d * 0.0001)) : null,
                FrameCount = props[Key.FrameCount] as uint?,
                Producers = props[Key.Producers] as IList<string> ?? new List<string>(),
                Publisher = (string)props[Key.Publisher],
                Writers = props[Key.Writers] as IList<string> ?? new List<string>(),
                Year = props[Key.Year] as uint?,

                SeriesName = (string)props[Key.SeriesName],
                SeasonNumber = props[Key.SeasonNumber] as uint?,
                EpisodeNumber = props[Key.EpisodeNumber] as uint?
            };
            return mediaProperties;
        }

        public static class Key
        {
            public static string Subtitle => "System.Media.SubTitle";
            public static string DateEncoded => "System.Media.DateEncoded";
            public static string DateReleased => "System.Media.DateReleased";
            public static string Duration => "System.Media.Duration";
            public static string FrameCount => "System.Media.FrameCount";
            public static string Producers => "System.Media.Producer";
            public static string Publisher = "System.Media.Publisher";
            public static string Writers => "System.Media.Writer";
            public static string Year => "System.Media.Year";

            public static string SeriesName => "System.Media.SeriesName";
            public static string SeasonNumber => "System.Media.SeasonNumber";
            public static string EpisodeNumber => "System.Media.EpisodeNumber";

            public static IEnumerable<string> All()
            {
                yield return Subtitle;
                yield return DateEncoded;
                yield return DateReleased;
                yield return Duration;
                yield return FrameCount;
                yield return Producers;
                yield return Publisher;
                yield return Writers;
                yield return Year;
                
                yield return SeriesName;
                yield return SeasonNumber;
                yield return EpisodeNumber;
            }
        }
    }
}
