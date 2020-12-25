using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Items.LocalStorage
{
    public class LocalFolder : FolderBase, ILocalDirectory
    {
        public LocalFolder(
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Local, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            
        }

    }
}
