﻿using NittyGritty;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Models.Favorite
{
    public class FavoriteDrive : ObservableObject
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
