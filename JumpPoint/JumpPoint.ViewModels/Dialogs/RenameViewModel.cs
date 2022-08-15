using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using JumpPoint.Platform;
using NittyGritty;
using Xamarin.Essentials;

namespace JumpPoint.ViewModels.Dialogs
{
    public class RenameViewModel : ObservableObject
    {
        public RenameViewModel(IList<string> names)
        {
            OldName = string.Join("; ", names);
            IsMultiple = names.Count > 1;
            Actions = new Collection<RenameOption>()
            {
                RenameOption.GenerateUniqueName,
                RenameOption.ReplaceExisting,
                RenameOption.DoNothing
            };
            if(IsMultiple)
            {
                // If renaming multiple items, the action will always be generate a unique name
                Action = Actions[0];
            }
        }

        public string OldName { get; }

        public bool IsMultiple { get; }

        private string _newName;

        public string NewName
        {
            get { return _newName; }
            set
            {
                Set(ref _newName, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public Collection<RenameOption> Actions { get; }

        public RenameOption Action
        {
            get { return (RenameOption)Preferences.Get(nameof(RenameOption), (int)RenameOption.GenerateUniqueName); }
            set
            {
                Preferences.Set(nameof(RenameOption), (int)value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get
            {
                // we do not need to check if oldname is equal to newname, the checkUniquesness Predicate handles that for us
                return !string.IsNullOrWhiteSpace(NewName) && NewName.IndexOfAny(Path.GetInvalidFileNameChars()) == -1;
            }
        }

    }

}
