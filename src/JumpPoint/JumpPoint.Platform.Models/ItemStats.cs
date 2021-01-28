using System;
using System.Text;
using JumpPoint.Platform.Items;
using NittyGritty;

namespace JumpPoint.Platform.Models
{
    public class ItemStats : ObservableObject
    {
        public void Update(int? drives = null, int? folders = null, int? files = null, int? workspaces = null, int? settingLinks = null, int? appLinks = null)
        {
            if (drives.HasValue)
            {
                Drive = drives.Value;
            }

            if (folders.HasValue)
            {
                Folder = folders.Value;
            }

            if (files.HasValue)
            {
                File = files.Value;
            }

            if (workspaces.HasValue)
            {
                Workspace = workspaces.Value;
            }

            if (settingLinks.HasValue)
            {
                SettingLink = settingLinks.Value;
            }

            if (appLinks.HasValue)
            {
                AppLink = appLinks.Value;
            }
        }

        public void Increment(JumpPointItemType type, int number)
        {
            switch (type)
            {
                case JumpPointItemType.File:
                    File += number;
                    break;
                case JumpPointItemType.Folder:
                    Folder += number;
                    break;
                case JumpPointItemType.Drive:
                    Drive += number;
                    break;
                case JumpPointItemType.Workspace:
                    Workspace += number;
                    break;
                case JumpPointItemType.SettingLink:
                    SettingLink += number;
                    break;
                case JumpPointItemType.AppLink:
                    AppLink += number;
                    break;
                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    break;
            }
        }

        public void Reset()
        {
            Update(0, 0, 0, 0, 0, 0);
        }

        public int Total
        {
            get { return Drive + Folder + File + Workspace + SettingLink + AppLink; }
        }

        private int _drive;

        public int Drive
        {
            get { return _drive; }
            private set
            {
                Set(ref _drive, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private int _folder;

        public int Folder
        {
            get { return _folder; }
            private set
            {
                Set(ref _folder, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private int _file;

        public int File
        {
            get { return _file; }
            private set
            {
                Set(ref _file, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private int _workspace;

        public int Workspace
        {
            get { return _workspace; }
            private set
            {
                Set(ref _workspace, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private int _settingLink;

        public int SettingLink
        {
            get { return _settingLink; }
            private set
            {
                Set(ref _settingLink, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        private int _appLink;

        public int AppLink
        {
            get { return _appLink; }
            private set
            {
                Set(ref _appLink, value);
                RaisePropertyChanged(nameof(Total));
            }
        }

        // TODO: next update, add Libraries

    }

}
