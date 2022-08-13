using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.FullTrust.Core.ChangeNotifier
{
    public class NotifyChange
    {
        public string Path { get; set; }

        public ChangeType ChangeType { get; set; }

        public string FullPath { get; set; }

        public string Name { get; set; }

        public string OldFullPath { get; set; }

        public string OldName { get; set; }

        public bool IsDirectory { get; set; }

    }

    public enum ChangeType
    {
        Unknown = 0,
        Created = 1,
        Deleted = 2,
        Changed = 3,
        Renamed = 4
    }
}
