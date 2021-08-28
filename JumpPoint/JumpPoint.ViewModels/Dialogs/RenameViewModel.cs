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
            Actions = new Collection<RenameCollisionOption>()
            {
                RenameCollisionOption.GenerateUniqueName,
                RenameCollisionOption.ReplaceExisting,
                RenameCollisionOption.DoNothing
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

        public Collection<RenameCollisionOption> Actions { get; }

        public RenameCollisionOption Action
        {
            get { return (RenameCollisionOption)Preferences.Get(nameof(RenameCollisionOption), (int)RenameCollisionOption.GenerateUniqueName); }
            set
            {
                Preferences.Set(nameof(RenameCollisionOption), (int)value);
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
