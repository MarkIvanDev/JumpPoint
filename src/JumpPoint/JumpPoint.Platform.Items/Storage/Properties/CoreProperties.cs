using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Storage.Properties.Core;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class CoreProperties : ObservableObject
    {

        private string _title;

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private string _subject;

        public string Subject
        {
            get { return _subject; }
            set { Set(ref _subject, value); }
        }

        private string _description;

        public string Description
        {
            get { return _description; }
            set { Set(ref _description, value); }
        }

        private string[] _authors;

        public string[] Authors
        {
            get { return _authors; }
            set { Set(ref _authors, value); }
        }

        private string _company;

        public string Company
        {
            get { return _company; }
            set { Set(ref _company, value); }
        }

        private string _copyright;

        public string Copyright
        {
            get { return _copyright; }
            set { Set(ref _copyright, value); }
        }

        private string _language;

        public string Language
        {
            get { return _language; }
            set { Set(ref _language, value); }
        }

        private string _contentType;

        public string ContentType
        {
            get { return _contentType; }
            set { Set(ref _contentType, value); }
        }

        private string _mimeType;

        public string MIMEType
        {
            get { return _mimeType; }
            set { Set(ref _mimeType, value); }
        }

        private DateTimeOffset? _dateAcquired;

        public DateTimeOffset? DateAcquired
        {
            get { return _dateAcquired; }
            set { Set(ref _dateAcquired, value); }
        }

        private DateTimeOffset? _dateArchived;

        public DateTimeOffset? DateArchived
        {
            get { return _dateArchived; }
            set { Set(ref _dateArchived, value); }
        }

        private DateTimeOffset? _dateCompleted;

        public DateTimeOffset? DateCompleted
        {
            get { return _dateCompleted; }
            set { Set(ref _dateCompleted, value); }
        }

        private DateTimeOffset? _dateImported;

        public DateTimeOffset? DateImported
        {
            get { return _dateImported; }
            set { Set(ref _dateImported, value); }
        }

        private ulong? _fileReferenceNumber;

        public ulong? FileReferenceNumber
        {
            get { return _fileReferenceNumber; }
            set { Set(ref _fileReferenceNumber, value); }
        }

        private string _fileVersion;

        public string FileVersion
        {
            get { return _fileVersion; }
            set { Set(ref _fileVersion, value); }
        }

        private string _owner;

        public string Owner
        {
            get { return _owner; }
            set { Set(ref _owner, value); }
        }

        private string _ownerSID;

        public string OwnerSID
        {
            get { return _ownerSID; }
            set { Set(ref _ownerSID, value); }
        }

        private string _computerName;

        public string ComputerName
        {
            get { return _computerName; }
            set { Set(ref _computerName, value); }
        }

        private string[] _sharedWith;

        public string[] SharedWith
        {
            get { return _sharedWith; }
            set { Set(ref _sharedWith, value); }
        }

        private SharingStatus? _sharingStatus;

        public SharingStatus? SharingStatus
        {
            get { return _sharingStatus; }
            set { Set(ref _sharingStatus, value); }
        }

        private string _application;

        public string Application
        {
            get { return _application; }
            set { Set(ref _application, value); }
        }

        private uint? _rating;

        public uint? Rating
        {
            get { return _rating; }
            set { Set(ref _rating, value); }
        }

        private string _status;

        public string Status
        {
            get { return _status; }
            set { Set(ref _status, value); }
        }

        private FlagColor? _flagColor;

        public FlagColor? FlagColor
        {
            get { return _flagColor; }
            set { Set(ref _flagColor, value); }
        }

        private FlagStatus? _flagStatus;

        public FlagStatus? FlagStatus
        {
            get { return _flagStatus; }
            set { Set(ref _flagStatus, value); }
        }

        private Importance? _importance;

        public Importance? Importance
        {
            get { return _importance; }
            set { Set(ref _importance, value); }
        }

        private Priority? _priority;

        public Priority? Priority
        {
            get { return _priority; }
            set { Set(ref _priority, value); }
        }

        private Sensitivity? _sensitivity;

        public Sensitivity? Sensitivity
        {
            get { return _sensitivity; }
            set { Set(ref _sensitivity, value); }
        }

        private string _comments;

        public string Comments
        {
            get { return _comments; }
            set { Set(ref _comments, value); }
        }

        private string[] _keywords;

        public string[] Keywords
        {
            get { return _keywords; }
            set { Set(ref _keywords, value); }
        }

        private string _trademarks;

        public string Trademarks
        {
            get { return _trademarks; }
            set { Set(ref _trademarks, value); }
        }

        public static CoreProperties Extract(IDictionary<string, object> props)
        {
            var coreProperties = new CoreProperties()
            {
                Title = (string)props[Key.Title],
                Subject = (string)props[Key.Subject],
                Description = (string)props[Key.Description],
                Authors = props[Key.Authors] as string[] ?? new string[0],
                Company = (string)props[Key.Company],
                Copyright = (string)props[Key.Copyright],
                Language = (string)props[Key.Language],
                ContentType = (string)props[Key.ContentType],
                MIMEType = (string)props[Key.MIMEType],

                DateAcquired = props[Key.DateAcquired] as DateTimeOffset?,
                DateArchived = props[Key.DateArchived] as DateTimeOffset?,
                DateCompleted = props[Key.DateCompleted] as DateTimeOffset?,
                DateImported = props[Key.DateImported] as DateTimeOffset?,

                FileReferenceNumber = props[Key.FileReferenceNumber] as ulong?,
                FileVersion = (string)props[Key.FileVersion],
                Owner = (string)props[Key.Owner],
                OwnerSID = (string)props[Key.OwnerSID],
                ComputerName = (string)props[Key.ComputerName],
                SharedWith = props[Key.SharedWith] as string[] ?? new string[0],
                SharingStatus = props[Key.SharingStatus] is uint ss ? (SharingStatus?)ss : null,

                Application = (string)props[Key.Application],
                Rating = props[Key.Rating] as uint?,
                Status = (string)props[Key.Status],
                FlagColor = props[Key.FlagColor] is ushort fc ? (FlagColor?)fc : null,
                FlagStatus = props[Key.FlagStatus] is int fs ? (FlagStatus?)fs : null,
                Importance = props[Key.Importance] is int imp ? (Importance?)imp : null,
                Priority = props[Key.Priority] is ushort prior ? (Priority?)prior : null,
                Sensitivity = props[Key.Sensitivity] is ushort sens ? (Sensitivity?)sens : null,
                Comments = (string)props[Key.Comments],
                Keywords = props[Key.Keywords] as string[] ?? new string[0],
                Trademarks = (string)props[Key.Trademarks],
            };
            return coreProperties;
        }

        public static class Key
        {
            public static string Title => "System.Title";
            public static string Subject => "System.Subject";
            public static string Description => "System.FileDescription";
            public static string Authors => "System.Author";
            public static string Company => "System.Company";
            public static string Copyright => "System.Copyright";
            public static string Language => "System.Language";
            public static string ContentType => "System.ContentType";
            public static string MIMEType => "System.MIMEType";

            public static string DateAcquired => "System.DateAcquired";
            public static string DateArchived => "System.DateArchived";
            public static string DateCompleted => "System.DateCompleted";
            public static string DateImported => "System.DateImported";

            public static string FileReferenceNumber => "System.FileFRN";
            public static string FileVersion => "System.FileVersion";
            public static string Owner => "System.FileOwner";
            public static string OwnerSID => "System.OwnerSID";
            public static string ComputerName => "System.ComputerName";
            public static string SharedWith => "System.SharedWith";
            public static string SharingStatus => "System.SharingStatus";

            public static string Application => "System.ApplicationName";
            public static string Rating => "System.SimpleRating";
            public static string Status => "System.Status";
            public static string FlagColor => "System.FlagColor";
            public static string FlagStatus => "System.FlagStatus";
            public static string Importance => "System.Importance";
            public static string Priority => "System.Priority";
            public static string Sensitivity => "System.Sensitivity";
            public static string Comments => "System.Comment";
            public static string Keywords => "System.Keywords";
            public static string Trademarks => "System.Trademarks";

            public static IEnumerable<string> All()
            {
                yield return Title;
                yield return Subject;
                yield return Description;
                yield return Authors;
                yield return Company;
                yield return Copyright;
                yield return Language;
                yield return ContentType;
                yield return MIMEType;

                yield return DateAcquired;
                yield return DateArchived;
                yield return DateCompleted;
                yield return DateImported;

                yield return FileReferenceNumber;
                yield return FileVersion;
                yield return Owner;
                yield return OwnerSID;
                yield return ComputerName;
                yield return SharedWith;
                yield return SharingStatus;
                
                yield return Application;
                yield return Rating;
                yield return Status;
                yield return FlagColor;
                yield return FlagStatus;
                yield return Importance;
                yield return Priority;
                yield return Sensitivity;
                yield return Comments;
                yield return Keywords;
                yield return Trademarks;
            }
        }

    }
}
