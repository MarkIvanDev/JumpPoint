using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items.LocalStorage
{
    public class LocalDrive : DriveBase, ILocalDirectory
    {
        public LocalDrive(string name,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Local, name, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
        }
    }
}
