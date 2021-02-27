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

        private string _division;

        public string Division
        {
            get { return _division; }
            set { Set(ref _division, value); }
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

        private string _presentationFormat;

        public string PresentationFormat
        {
            get { return _presentationFormat; }
            set { Set(ref _presentationFormat, value); }
        }

        private string _clientId;

        public string ClientId
        {
            get { return _clientId; }
            set { Set(ref _clientId, value); }
        }

        private int? _security;

        public int? Security
        {
            get { return _security; }
            set { Set(ref _security, value); }
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

        private int? _byteCount;

        public int? ByteCount
        {
            get { return _byteCount; }
            set { Set(ref _byteCount, value); }
        }

        private int? _multimediaClipCount;

        public int? MultimediaClipCount
        {
            get { return _multimediaClipCount; }
            set { Set(ref _multimediaClipCount, value); }
        }

        private int? _noteCount;

        public int? NoteCount
        {
            get { return _noteCount; }
            set { Set(ref _noteCount, value); }
        }

        private int? _slideCount;

        public int? SlideCount
        {
            get { return _slideCount; }
            set { Set(ref _slideCount, value); }
        }

        private int? _hiddenSlideCount;

        public int? HiddenSlideCount
        {
            get { return _hiddenSlideCount; }
            set { Set(ref _hiddenSlideCount, value); }
        }

        public static DocumentProperties Extract(IDictionary<string, object> props)
        {
            var documentProperties = new DocumentProperties()
            {
                DocumentID = (string)props[Key.DocumentId],
                Revision = (string)props[Key.Revision],
                Version = (string)props[Key.Version],
                LastAuthor = (string)props[Key.LastAuthor],
                Contributors = props[Key.Contributors] as IList<string> ?? new List<string>(),
                Division = (string)props[Key.Division],
                Manager = (string)props[Key.Manager],
                Template = (string)props[Key.Template],
                PresentationFormat = (string)props[Key.PresentationFormat],
                ClientId = (string)props[Key.ClientId],
                Security = props[Key.Security] as int?,

                DateCreated = props[Key.DateCreated] as DateTimeOffset?,
                LastPrinted = props[Key.LastPrinted] as DateTimeOffset?,
                LastSaved = props[Key.LastSaved] as DateTimeOffset?,
                TotalEditingTime = props[Key.TotalEditingTime] is ulong tet ? new TimeSpan?(TimeSpan.FromMilliseconds(tet * 0.0001)) : null,

                PageCount = props[Key.PageCount] as int?,
                ParagraphCount = props[Key.ParagraphCount] as int?,
                LineCount = props[Key.LineCount] as int?,
                WordCount = props[Key.WordCount] as int?,
                CharacterCount = props[Key.CharacterCount] as int?,
                ByteCount = props[Key.ByteCount] as int?,
                MultimediaClipCount = props[Key.MultimediaClipCount] as int?,
                NoteCount = props[Key.NoteCount] as int?,
                SlideCount = props[Key.SlideCount] as int?,
                HiddenSlideCount = props[Key.HiddenSlideCount] as int?
            };
            return documentProperties;
        }

        public static class Key
        {
            public static string DocumentId => "System.Document.DocumentID";
            public static string Revision => "System.Document.RevisionNumber";
            public static string Version => "System.Document.Version";
            public static string LastAuthor => "System.Document.LastAuthor";
            public static string Contributors => "System.Document.Contributor";
            public static string Division => "System.Document.Division";
            public static string Manager => "System.Document.Manager";
            public static string Template => "System.Document.Template";
            public static string PresentationFormat => "System.Document.PresentationFormat";
            public static string ClientId => "System.Document.ClientID";
            public static string Security => "System.Document.Security";

            public static string DateCreated => "System.Document.DateCreated";
            public static string LastPrinted => "System.Document.DatePrinted";
            public static string LastSaved => "System.Document.DateSaved";
            public static string TotalEditingTime => "System.Document.TotalEditingTime";

            public static string PageCount => "System.Document.PageCount";
            public static string ParagraphCount => "System.Document.ParagraphCount";
            public static string LineCount => "System.Document.LineCount";
            public static string WordCount => "System.Document.WordCount";
            public static string CharacterCount => "System.Document.CharacterCount";
            public static string ByteCount => "System.Document.ByteCount";
            public static string MultimediaClipCount => "System.Document.MultimediaClipCount";
            public static string NoteCount => "System.Document.NoteCount";
            public static string SlideCount => "System.Document.SlideCount";
            public static string HiddenSlideCount => "System.Document.HiddenSlideCount";


            public static IEnumerable<string> All()
            {
                yield return DocumentId;
                yield return Revision;
                yield return Version;
                yield return LastAuthor;
                yield return Contributors;
                yield return Division;
                yield return Manager;
                yield return Template;
                yield return PresentationFormat;
                yield return ClientId;
                yield return Security;
                
                yield return DateCreated;
                yield return LastPrinted;
                yield return LastSaved;
                yield return TotalEditingTime;
                
                yield return PageCount;
                yield return ParagraphCount;
                yield return LineCount;
                yield return WordCount;
                yield return CharacterCount;
                yield return ByteCount;
                yield return MultimediaClipCount;
                yield return NoteCount;
                yield return SlideCount;
                yield return HiddenSlideCount;
            }
        }
    }
}