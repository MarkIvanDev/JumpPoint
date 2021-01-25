using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using NittyGritty.Models;

namespace JumpPoint.Platform.Items
{
    public class Workspace : JumpPointItem
    {
        public Workspace(int id, WorkspaceTemplate template, string name, DateTimeOffset dateCreated) :
            base(JumpPointItemType.Workspace)
        {
            Id = id;
            Template = template;
            Name = name;
            Path = name;
            DisplayName = name;
            DisplayType = nameof(JumpPointItemType.Workspace);
            DateCreated = dateCreated;
        }

        public int Id { get; }

        private WorkspaceTemplate _template;

        public WorkspaceTemplate Template
        {
            get { return _template; }
            set { Set(ref _template, value); }
        }

        private DateTimeOffset _dateCreated;

        public DateTimeOffset DateCreated
        {
            get { return _dateCreated; }
            set { Set(ref _dateCreated, value); }
        }

        private ulong _appLinkCount;

        public ulong AppLinkCount
        {
            get { return _appLinkCount; }
            set { Set(ref _appLinkCount, value); }
        }

        private ulong driveCount;

        public ulong DriveCount
        {
            get { return driveCount; }
            set { Set(ref driveCount, value); }
        }

        private ulong folderCount;

        public ulong FolderCount
        {
            get { return folderCount; }
            set { Set(ref folderCount, value); }
        }

        private ulong fileCount;

        public ulong FileCount
        {
            get { return fileCount; }
            set { Set(ref fileCount, value); }
        }

        private ulong settingCount;

        public ulong SettingCount
        {
            get { return settingCount; }
            set { Set(ref settingCount, value); }
        }

    }

    public class SelectableWorkspace : SelectableItem<Workspace>
    {
    }

}
