using System;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using NittyGritty;
using NittyGritty.Commands;

namespace JumpPoint.ViewModels.Dialogs
{
    public class NewWorkspaceViewModel : ObservableObject
    {
        public NewWorkspaceViewModel()
        {
            Template = WorkspaceTemplate.Briefcase;
        }

        private WorkspaceTemplate _template;

        public WorkspaceTemplate Template
        {
            get { return _template; }
            set
            {
                Set(ref _template, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        private RelayCommand<WorkspaceTemplate> _setTemplateCommand;

        /// <summary>
        /// Gets the SetTemplateCommand.
        /// </summary>
        public RelayCommand<WorkspaceTemplate> SetTemplateCommand
        {
            get
            {
                return _setTemplateCommand
                    ?? (_setTemplateCommand = new RelayCommand<WorkspaceTemplate>(
                    template =>
                    {
                        Template = template;
                    }));
            }
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
