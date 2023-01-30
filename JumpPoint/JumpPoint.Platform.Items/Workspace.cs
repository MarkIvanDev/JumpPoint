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

        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                Path = $@"workspace:\{base.Name}";
                DisplayName = base.Name;
            }
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
            set
            {
                Set(ref _appLinkCount, value);
                RaisePropertyChanged(nameof(ItemCount));
            }
        }

        private ulong driveCount;

        public ulong DriveCount
        {
            get { return driveCount; }
            set
            {
                Set(ref driveCount, value);
                RaisePropertyChanged(nameof(ItemCount));
            }
        }

        private ulong folderCount;

        public ulong FolderCount
        {
            get { return folderCount; }
            set
            {
                Set(ref folderCount, value);
                RaisePropertyChanged(nameof(ItemCount));
            }
        }

        private ulong fileCount;

        public ulong FileCount
        {
            get { return fileCount; }
            set
            {
                Set(ref fileCount, value);
                RaisePropertyChanged(nameof(ItemCount));
            }
        }

        public ulong ItemCount => DriveCount + FolderCount + FileCount + AppLinkCount;

    }

    public class SelectableWorkspace : SelectableItem<Workspace>
    {
    }

}
