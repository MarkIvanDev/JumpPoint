using System;
using System.Text;
using JumpPoint.Platform.Items.Templates;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models
{
    public class FolderInfo : ObservableObject
    {

        private int _id;

        [AutoIncrement, PrimaryKey]
        public int Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        private string _path;

        [NotNull, Unique, Collation("NOCASE")]
        public string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

        private FolderTemplate _template;

        [NotNull]
        public FolderTemplate Template
        {
            get { return _template; }
            set { Set(ref _template, value); }
        }

    }
}
