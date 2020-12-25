using System;
using System.Collections.Generic;
using NittyGritty;

namespace JumpPoint.Platform.Items.Storage.Properties
{
    public class DocumentProperties : ObservableObject
    {

        private string _documentID;

        public string DocumentID
        {
            get { return _documentID; }
            set { Set(ref _documentID, value); }
        }

        private string _revision;

        public string Revision
        {
            get { return _revision; }
            set { Set(ref _revision, value); }
        }

        private string _version;

        public string Version
        {
            get { return _version; }
            set { Set(ref _version, value); }
        }

        private string _lastAuthor;

        public string LastAuthor
        {
            get { return _lastAuthor; }
            set { Set(ref _lastAuthor, value); }
        }

        private IList<string> _contributors;

        public IList<string> Contributors
        {
            get { return _contributors; }
            set { Set(ref _contributors, value); }
        }

        private string _manager;

        public string Manager
        {
            get { return _manager; }
            set { Set(ref _manager, value); }
        }

        private string _template;

        public string Template
        {
            get { return _template; }
            set { Set(ref _template, value); }
        }

        private DateTimeOffset? _dateCreated;

        public DateTimeOffset? DateCreated
        {
            get { return _dateCreated; }
            set { Set(ref _dateCreated, value); }
        }

        private DateTimeOffset? _lastPrinted;

        public DateTimeOffset? LastPrinted
        {
            get { return _lastPrinted; }
            set { Set(ref _lastPrinted, value); }
        }

        private DateTimeOffset? _lastSaved;

        public DateTimeOffset? LastSaved
        {
            get { return _lastSaved; }
            set { Set(ref _lastSaved, value); }
        }

        private TimeSpan? _totalEditingTime;

        public TimeSpan? TotalEditingTime
        {
            get { return _totalEditingTime; }
            set { Set(ref _totalEditingTime, value); }
        }

        private int? _pageCount;

        public int? PageCount
        {
            get { return _pageCount; }
            set { Set(ref _pageCount, value); }
        }

        private int? _paragraphCount;

        public int? ParagraphCount
        {
            get { return _paragraphCount; }
            set { Set(ref _paragraphCount, value); }
        }

        private int? _lineCount;

        public int? LineCount
        {
            get { return _lineCount; }
            set { Set(ref _lineCount, value); }
        }

        private int? _wordCount;

        public int? WordCount
        {
            get { return _wordCount; }
            set { Set(ref _wordCount, value); }
        }

        private int? _characterCount;

        public int? CharacterCount
        {
            get { return _characterCount; }
            set { Set(ref _characterCount, value); }
        }

        public static DocumentProperties Extract(IDictionary<string, object> props)
        {
            var documentProperties = new DocumentProperties()
            {
                DocumentID = (string)props[Key.DocumentID],
                Revision = (string)props[Key.Revision],
                Version = (string)props[Key.Version],
                LastAuthor = (string)props[Key.LastAuthor],
                Contributors = props[Key.Contributors] as IList<string> ?? new List<string>(),
                Manager = (string)props[Key.Manager],
                Template = (string)props[Key.Template],

                DateCreated = props[Key.DateCreated] as DateTimeOffset?,
                LastPrinted = props[Key.LastPrinted] as DateTimeOffset?,
                LastSaved = props[Key.LastSaved] as DateTimeOffset?,
                TotalEditingTime = props[Key.TotalEditingTime] is ulong tet ? new TimeSpan?(TimeSpan.FromMilliseconds(tet * 0.0001)) : null,

                PageCount = props[Key.PageCount] as int?,
                ParagraphCount = props[Key.ParagraphCount] as int?,
                LineCount = props[Key.LineCount] as int?,
                WordCount = props[Key.WordCount] as int?,
                CharacterCount = props[Key.CharacterCount] as int?
            };
            return documentProperties;
        }

        public static class Key
        {
            public static string DocumentID => "System.Document.DocumentID";
            public static string Revision => "System.Document.RevisionNumber";
            public static string Version => "System.Document.Version";
            public static string LastAuthor => "System.Document.LastAuthor";
            public static string Contributors => "System.Document.Contributor";
            public static string Manager => "System.Document.Manager";
            public static string Template => "System.Document.Template";

            public static string DateCreated => "System.Document.DateCreated";
            public static string LastPrinted => "System.Document.DatePrinted";
            public static string LastSaved => "System.Document.DateSaved";
            public static string TotalEditingTime => "System.Document.TotalEditingTime";

            public static string PageCount => "System.Document.PageCount";
            public static string ParagraphCount => "System.Document.ParagraphCount";
            public static string LineCount => "System.Document.LineCount";
            public static string WordCount => "System.Document.WordCount";
            public static string CharacterCount => "System.Document.CharacterCount";

            public static IEnumerable<string> All()
            {
                yield return DocumentID;
                yield return Revision;
                yield return Version;
                yield return LastAuthor;
                yield return Contributors;
                yield return Manager;
                yield return Template;
                
                yield return DateCreated;
                yield return LastPrinted;
                yield return LastSaved;
                yield return TotalEditingTime;
                
                yield return PageCount;
                yield return ParagraphCount;
                yield return LineCount;
                yield return WordCount;
                yield return CharacterCount;
            }
        }
    }
}