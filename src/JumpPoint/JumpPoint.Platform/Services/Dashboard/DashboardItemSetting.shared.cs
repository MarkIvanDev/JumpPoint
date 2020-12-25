using System;
using System.Text;
using NittyGritty;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Services
{
    public class UserFolderSetting : ObservableObject
    {
        public UserFolderSetting(UserFolderTemplate template)
        {
            Template = template;
        }

        public UserFolderTemplate Template { get; }

        public bool IsOn
        {
            get
            {
                return DashboardService.GetStatus(Template);
            }
            set
            {
                DashboardService.SetStatus(Template, value);
                RaisePropertyChanged();
            }
        }
    }

    public class SystemFolderSetting : ObservableObject
    {
        public SystemFolderSetting(SystemFolderTemplate template)
        {
            Template = template;
        }

        public SystemFolderTemplate Template { get; }

        public bool IsOn
        {
            get
            {
                return DashboardService.GetStatus(Template);
            }
            set
            {
                DashboardService.SetStatus(Template, value);
                RaisePropertyChanged();
            }
        }
    }

}
