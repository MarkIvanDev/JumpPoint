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

        private IList<string> _authors;

        public IList<string> Authors
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

        private ulong? _fileReferenceNumber;

        public ulong? FileReferenceNumber
        {
            get { return _fileReferenceNumber; }
            set { Set(ref _fileReferenceNumber, value); }
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

        private IList<string> _sharedWith;

        public IList<string> SharedWith
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

        private string _comments;

        public string Comments
        {
            get { return _comments; }
            set { Set(ref _comments, value); }
        }

        private IList<string> _keywords;

        public IList<string> Keywords
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
                Authors = props[Key.Authors] as IList<string> ?? new List<string>(),
                Company = (string)props[Key.Company],
                Copyright = (string)props[Key.Copyright],

                FileReferenceNumber = props[Key.FileReferenceNumber] as ulong?,
                Owner = (string)props[Key.Owner],
                OwnerSID = (string)props[Key.OwnerSID],
                ComputerName = (string)props[Key.ComputerName],
                SharedWith = props[Key.SharedWith] as IList<string> ?? new List<string>(),
                SharingStatus = props[Key.SharingStatus] is uint ss ? (SharingStatus?)ss : null,

                Application = (string)props[Key.Application],
                Rating = props[Key.Rating] as uint?,
                Status = (string)props[Key.Status],
                Comments = (string)props[Key.Comments],
                Keywords = props[Key.Keywords] as IList<string> ?? new List<string>(),
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

            public static string FileReferenceNumber => "System.FileFRN";
            public static string Owner => "System.FileOwner";
            public static string OwnerSID => "System.OwnerSID";
            public static string ComputerName => "System.ComputerName";
            public static string SharedWith => "System.SharedWith";
            public static string SharingStatus => "System.SharingStatus";

            public static string Application => "System.ApplicationName";
            public static string Rating => "System.SimpleRating";
            public static string Status => "System.Status";
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

                yield return FileReferenceNumber;
                yield return Owner;
                yield return OwnerSID;
                yield return ComputerName;
                yield return SharedWith;
                yield return SharingStatus;
                
                yield return Application;
                yield return Rating;
                yield return Status;
                yield return Comments;
                yield return Keywords;
                yield return Trademarks;
            }
        }

    }
}
