﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public abstract class CloudDrive : DriveBase, ICloudDirectory
    {
        
        public CloudDrive(CloudStorageProvider provider, string name,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Cloud, name, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Provider = provider;
            DriveTemplate = provider.ToDriveTemplate();
        }

        public CloudStorageProvider Provider { get; }

    }
}
