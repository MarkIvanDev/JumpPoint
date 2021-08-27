using System;
using System.Collections.ObjectModel;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using JumpPoint.Platform.Services;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class WorkspaceTemplatePickerViewModel : ObservableObject
    {
        public Collection<WorkspaceTemplate> WorkspaceTemplates { get; } = new Collection<WorkspaceTemplate>(WorkspaceService.GetTemplates());

        private WorkspaceTemplate? _template;

        public WorkspaceTemplate? Template
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
