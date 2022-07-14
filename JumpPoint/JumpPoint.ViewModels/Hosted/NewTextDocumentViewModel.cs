using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JumpPoint.Extensions;
using JumpPoint.Extensions.NewItems;
using NittyGritty.Commands;
using NittyGritty.Extensions;
using NittyGritty.Platform.Payloads;
using NittyGritty.Utilities;
using NittyGritty.ViewModels;

namespace JumpPoint.ViewModels.Hosted
{
    public class NewTextDocumentViewModel : ViewModelBase
    {
        private IProtocolForResultsPayload protocolForResultsPayload = null;

        public NewTextDocumentViewModel()
        {
            FileExtensions = new List<FileExtension>()
            {
                new FileExtension("All types (*.*)", ""),
                new FileExtension("Text file (*.txt)", ".txt"),
                new FileExtension("JSON file (*.json)", ".json"),
                new FileExtension("XML file (*.xml)", ".xml"),
            };
            Encodings = new List<TextEncoding>()
            {
                new TextEncoding(Encoding.ASCII, true),
                new TextEncoding(Encoding.UTF8, false),
                new TextEncoding(Encoding.UTF8, true),
                new TextEncoding(Encoding.Unicode, true),
                new TextEncoding(Encoding.BigEndianUnicode, true)
            };
        }

        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set
            {
                Set(ref _fileName, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public IList<FileExtension> FileExtensions { get; }

        private FileExtension _fileExtension;

        public FileExtension FileExtension
        {
            get { return _fileExtension; }
            set
            {
                Set(ref _fileExtension, value);
                RaisePropertyChanged(nameof(IsValid));
                UpdateContent();
            }
        }

        private string _content;

        public string Content
        {
            get { return _content; }
            set { Set(ref _content, value); }
        }

        public IList<TextEncoding> Encodings { get; }

        private TextEncoding _textEncoding;

        public TextEncoding TextEncoding
        {
            get { return _textEncoding; }
            set
            {
                Set(ref _textEncoding, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { Set(ref _errorMessage, value); }
        }

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FileName) || FileName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                {
                    ErrorMessage = "File name is invalid";
                    return false;
                }

                if (FileExtension is null)
                {
                    ErrorMessage = "Please pick a file extension";
                    return false;
                }

                if (TextEncoding is null)
                {
                    ErrorMessage = "Please pick an encoding";
                    return false;
                }

                ErrorMessage = string.Empty;
                return true;
            }
        }

        private void UpdateContent()
        {
            if (FileExtension != null && string.IsNullOrEmpty(Content))
            {
                switch (FileExtension.Extension)
                {
                    case ".json":
                        Content = $"{{{Environment.NewLine}\t{Environment.NewLine}}}";
                        break;

                    case ".xml":
                        Content = $@"<?xml version=""1.0"" encoding=""UTF - 8""?>{Environment.NewLine}";
                        break;

                    case ".txt":
                    case "":
                    default:
                        break;
                }
            }
        }

        private AsyncRelayCommand _Create;
        public AsyncRelayCommand CreateCommand => _Create ?? (_Create = new AsyncRelayCommand(
            async () =>
            {
                ErrorMessage = string.Empty;
                if (IsValid)
                {
                    var result = new NewItemResultPayload();
                    result.FileName = FileName.Trim().WithEnding(FileExtension.Extension);

                    var bytes = CodeHelper.InvokeOrDefault(() => TextEncoding.Encoding.GetBytes(Content ?? string.Empty).ToList());
                    if (bytes != null)
                    {
                        if (TextEncoding.HasPreamble)
                        {
                            bytes.InsertRange(0, TextEncoding.Encoding.GetPreamble());
                        }
                        result.ContentToken = await IOHelper.WriteBytes(bytes.ToArray());
                        protocolForResultsPayload?.ReportResults(await NewItemHelper.GetData(new List<NewItemResultPayload> { result }));
                    }
                    else
                    {
                        ErrorMessage = "There was a problem when encoding the content";
                        return;
                    }
                }
            }));

        private RelayCommand _Cancel;
        public RelayCommand CancelCommand => _Cancel ?? (_Cancel = new RelayCommand(
            () =>
            {
                protocolForResultsPayload?.ReportResults(null);
            }));

        public override void LoadState(object parameter, Dictionary<string, object> state)
        {
            IsLoading = true;
            if (parameter is IProtocolForResultsPayload payload)
            {
                protocolForResultsPayload = payload;
                FileName = "New Text Document";
                FileExtension = FileExtensions.ElementAtOrDefault(1);
                TextEncoding = Encodings.FirstOrDefault(e => e.Encoding.CodePage == Encoding.UTF8.CodePage);
            }
            IsLoading = false;
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            
        }
    }

    public class FileExtension
    {
        public FileExtension(string displayName, string extension)
        {
            DisplayName = displayName;
            Extension = extension;
        }

        public string DisplayName { get; }
        public string Extension { get; }
    }

    public class TextEncoding
    {
        public TextEncoding(Encoding encoding, bool hasPreamble)
        {
            Encoding = encoding;
            HasPreamble = hasPreamble;
        }

        public Encoding Encoding { get; }

        public bool HasPreamble { get; }
    }
}
