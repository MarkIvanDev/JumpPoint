using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class RenameCloudAccountViewModel : ObservableObject
    {
        public RenameCloudAccountViewModel(CloudAccount account)
        {
            Account = account;
            Name = account.Name;
        }

        public CloudAccount Account { get; }

        private string _name;

        public string Name
        {
            get { return _name; }
            set
            {
                Set(ref _name, value);
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
