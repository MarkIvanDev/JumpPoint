using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public abstract class CloudFolder : FolderBase, ICloudDirectory
    {
        public CloudFolder(CloudStorageService service,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Cloud, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Service = service;
        }

        public CloudStorageService Service { get; }
    }
}
