using System;
using System.IO;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class NewFolderViewModel : ObservableObject
    {
        public NewFolderViewModel()
        {
            
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                Set(ref _Name, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Name) && Name.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
            }
        }

    }
}
