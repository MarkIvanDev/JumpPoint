using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JumpPoint.Platform.Items.Storage
{
    public abstract class DirectoryBase : StorageItemBase
    {
        public DirectoryBase(JumpPointItemType type, StorageType storageType,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(type, storageType, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
        }
    }
}
