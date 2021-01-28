using System;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models.Workspace
{
    public class WorkspaceSettingItem : ObservableObject
    {

        private int _id;

        [AutoIncrement, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private int _workspaceId;

        [NotNull]
        public int WorkspaceId
        {
            get { return _workspaceId; }
            set { Set(ref _workspaceId, value); }
        }

        private SettingLinkTemplate _setting;

        [NotNull, Unique]
        public SettingLinkTemplate Setting
        {
            get { return _setting; }
            set { Set(ref _setting, value); }
        }

    }
}
