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

        private string[] _producers;

        public string[] Producers
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

        private string[] _writers;

        public string[] Writers
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

        private uint? _averageLevel;

        public uint? AverageLevel
        {
            get { return _averageLevel; }
            set { Set(ref _averageLevel, value); }
        }

        private string _encodedBy;

        public string EncodedBy
        {
            get { return _encodedBy; }
            set { Set(ref _encodedBy, value); }
        }

        private string _encodingSettings;

        public string EncodingSettings
        {
            get { return _encodingSettings; }
            set { Set(ref _encodingSettings, value); }
        }

        private string _protectionType;

        public string ProtectionType
        {
            get { return _protectionType; }
            set { Set(ref _protectionType, value); }
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

        private string _classPrimaryId;

        public string ClassPrimaryId
        {
            get { return _classPrimaryId; }
            set { Set(ref _classPrimaryId, value); }
        }

        private string _classSecondaryId;

        public string ClassSecondaryId
        {
            get { return _classSecondaryId; }
            set { Set(ref _classSecondaryId, value); }
        }

        private string _collectionGroupId;

        public string CollectionGroupId
        {
            get { return _collectionGroupId; }
            set { Set(ref _collectionGroupId, value); }
        }

        private string _collectionId;

        public string CollectionId
        {
            get { return _collectionId; }
            set { Set(ref _collectionId, value); }
        }

        private string _contentId;

        public string ContentId
        {
            get { return _contentId; }
            set { Set(ref _contentId, value); }
        }

        private string _dlnaProfileId;

        public string DlnaProfileId
        {
            get { return _dlnaProfileId; }
            set { Set(ref _dlnaProfileId, value); }
        }

        private string _dvdId;

        public string DvdId
        {
            get { return _dvdId; }
            set { Set(ref _dvdId, value); }
        }

        private string _MCDI;

        public string MCDI
        {
            get { return _MCDI; }
            set { Set(ref _MCDI, value); }
        }

        private string _subscriptionContentId;

        public string SubscriptionContentId
        {
            get { return _subscriptionContentId; }
            set { Set(ref _subscriptionContentId, value); }
        }

        private string _uniqueFileIdentifier;

        public string UniqueFileIdentifier
        {
            get { return _uniqueFileIdentifier; }
            set { Set(ref _uniqueFileIdentifier, value); }
        }

        private string _contentDistributor;

        public string ContentDistributor
        {
            get { return _contentDistributor; }
            set { Set(ref _contentDistributor, value); }
        }

        private string _creatorApplication;

        public string CreatorApplication
        {
            get { return _creatorApplication; }
            set { Set(ref _creatorApplication, value); }
        }

        private string _creatorApplicationVersion;

        public string CreatorApplicationVersion
        {
            get { return _creatorApplicationVersion; }
            set { Set(ref _creatorApplicationVersion, value); }
        }

        private string _metadataContentProvider;

        public string MetadataContentProvider
        {
            get { return _metadataContentProvider; }
            set { Set(ref _metadataContentProvider, value); }
        }

        private string _providerRating;

        public string ProviderRating
        {
            get { return _providerRating; }
            set { Set(ref _providerRating, value); }
        }

        private string _providerStyle;

        public string ProviderStyle
        {
            get { return _providerStyle; }
            set { Set(ref _providerStyle, value); }
        }

        private string _authorUrl;

        public string AuthorUrl
        {
            get { return _authorUrl; }
            set { Set(ref _authorUrl, value); }
        }

        private string _promotionUrl;

        public string PromotionUrl
        {
            get { return _promotionUrl; }
            set { Set(ref _promotionUrl, value); }
        }

        private string _userWebUrl;

        public string UserWebUrl
        {
            get { return _userWebUrl; }
            set { Set(ref _userWebUrl, value); }
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
                Producers = props[Key.Producers] as string[] ?? new string[0],
                Publisher = (string)props[Key.Publisher],
                Writers = props[Key.Writers] as string[] ?? new string[0],
                Year = props[Key.Year] as uint?,
                AverageLevel = props[Key.AverageLevel] as uint?,
                EncodedBy = (string)props[Key.EncodedBy],
                EncodingSettings = (string)props[Key.EncodingSettings],
                ProtectionType = (string)props[Key.ProtectionType],

                SeriesName = (string)props[Key.SeriesName],
                SeasonNumber = props[Key.SeasonNumber] as uint?,
                EpisodeNumber = props[Key.EpisodeNumber] as uint?,

                ClassPrimaryId = (string)props[Key.ClassPrimaryId],
                ClassSecondaryId = (string)props[Key.ClassSecondaryId],
                CollectionGroupId = (string)props[Key.CollectionGroupId],
                CollectionId = (string)props[Key.CollectionId],
                ContentId = (string)props[Key.ContentId],
                DlnaProfileId = (string)props[Key.DlnaProfileId],
                DvdId = (string)props[Key.DvdId],
                MCDI = (string)props[Key.MCDI],
                SubscriptionContentId = (string)props[Key.SubscriptionContentId],
                UniqueFileIdentifier = (string)props[Key.UniqueFileIdentifier],

                ContentDistributor = (string)props[Key.ContentDistributor],
                CreatorApplication = (string)props[Key.CreatorApplication],
                CreatorApplicationVersion = (string)props[Key.CreatorApplicationVersion],
                MetadataContentProvider = (string)props[Key.MetadataContentProvider],
                ProviderRating = (string)props[Key.ProviderRating],
                ProviderStyle = (string)props[Key.ProviderStyle],

                AuthorUrl = (string)props[Key.AuthorUrl],
                PromotionUrl = (string)props[Key.PromotionUrl],
                UserWebUrl = (string)props[Key.UserWebUrl]
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
            public static string Publisher => "System.Media.Publisher";
            public static string Writers => "System.Media.Writer";
            public static string Year => "System.Media.Year";
            public static string AverageLevel => "System.Media.AverageLevel";
            public static string EncodedBy => "System.Media.EncodedBy";
            public static string EncodingSettings => "System.Media.EncodingSettings";
            public static string ProtectionType => "System.Media.ProtectionType";

            public static string SeriesName => "System.Media.SeriesName";
            public static string SeasonNumber => "System.Media.SeasonNumber";
            public static string EpisodeNumber => "System.Media.EpisodeNumber";

            public static string ClassPrimaryId => "System.Media.ClassPrimaryID";
            public static string ClassSecondaryId => "System.Media.ClassSecondaryID";
            public static string CollectionGroupId => "System.Media.CollectionGroupID";
            public static string CollectionId => "System.Media.CollectionID";
            public static string ContentId => "System.Media.ContentID";
            public static string DlnaProfileId => "System.Media.DlnaProfileID";
            public static string DvdId => "System.Media.DVDID";
            public static string MCDI => "System.Media.MCDI";
            public static string SubscriptionContentId => "System.Media.SubscriptionContentId";
            public static string UniqueFileIdentifier => "System.Media.UniqueFileIdentifier";

            public static string ContentDistributor => "System.Media.ContentDistributor";
            public static string CreatorApplication => "System.Media.CreatorApplication";
            public static string CreatorApplicationVersion => "System.Media.CreatorApplicationVersion";
            public static string MetadataContentProvider => "System.Media.MetadataContentProvider";
            public static string ProviderRating => "System.Media.ProviderRating";
            public static string ProviderStyle => "System.Media.ProviderStyle";

            public static string AuthorUrl => "System.Media.AuthorUrl";
            public static string PromotionUrl => "System.Media.PromotionUrl";
            public static string UserWebUrl => "System.Media.UserWebUrl";

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
                yield return AverageLevel;
                yield return EncodedBy;
                yield return EncodingSettings;
                yield return ProtectionType;
                
                yield return SeriesName;
                yield return SeasonNumber;
                yield return EpisodeNumber;

                yield return ClassPrimaryId;
                yield return ClassSecondaryId;
                yield return CollectionGroupId;
                yield return CollectionId;
                yield return ContentId;
                yield return DlnaProfileId;
                yield return DvdId;
                yield return MCDI;
                yield return SubscriptionContentId;
                yield return UniqueFileIdentifier;

                yield return ContentDistributor;
                yield return CreatorApplication;
                yield return CreatorApplicationVersion;
                yield return MetadataContentProvider;
                yield return ProviderRating;
                yield return ProviderStyle;

                yield return AuthorUrl;
                yield return PromotionUrl;
                yield return UserWebUrl;
            }
        }
    }
}
