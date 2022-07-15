using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class NewFileViewModel : ObservableObject
    {

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
