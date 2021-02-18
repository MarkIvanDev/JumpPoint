using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public abstract class CloudFile : FileBase
    {
        public CloudFile(CloudStorageProvider provider,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Cloud, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Provider = provider;
        }

        public CloudStorageProvider Provider { get; }
    }
}
