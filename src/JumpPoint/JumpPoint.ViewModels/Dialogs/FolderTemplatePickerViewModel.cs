using System;
using System.Collections.ObjectModel;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Services;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class FolderTemplatePickerViewModel : ObservableObject
    {
        public Collection<FolderTemplate> FolderTemplates { get; } = new Collection<FolderTemplate>(FolderTemplateService.GetFolderTemplates());

        private FolderTemplate? _template;

        public FolderTemplate? Template
        {
            get { return _template; }
            set
            {
                Set(ref _template, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid => Template.HasValue;

    }
}
