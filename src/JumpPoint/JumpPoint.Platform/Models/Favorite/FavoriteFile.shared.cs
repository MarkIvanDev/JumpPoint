using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models.Favorite
{
    public class FavoriteFile : ObservableObject
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
    }
}
