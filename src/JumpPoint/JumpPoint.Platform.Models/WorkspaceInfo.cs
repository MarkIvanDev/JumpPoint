using System;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models.Workspace
{
    public class WorkspaceInfo : ObservableObject
    {
        private int _id;

        [AutoIncrement, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private string _name;

        [NotNull, Unique, Collation("NOCASE")]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        private DateTimeOffset _dateCreated;

        [NotNull]
        public DateTimeOffset DateCreated
        {
            get { return _dateCreated; }
            set { Set(ref _dateCreated, value); }
        }

        private WorkspaceTemplate _template;

        [NotNull]
        public WorkspaceTemplate Template
        {
            get { return _template; }
            set { Set(ref _template, value); }
        }
    }
}
