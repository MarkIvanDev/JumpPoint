﻿using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty;
using SQLite;

namespace JumpPoint.Platform.Models.Workspace
{
    public class WorkspaceAppLinkItem : ObservableObject
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

        private string _path;

        [NotNull, Collation("NOCASE"), Unique]
        public string Path
        {
            get { return _path; }
            set { Set(ref _path, value); }
        }

    }
}
