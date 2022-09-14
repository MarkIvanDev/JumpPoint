using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using JumpPoint.Platform.Items.Storage;
using uplink.NET.Models;

namespace JumpPoint.Platform.Items.Storj
{
    public class StorjFolder : CloudFolder
    {
        public StorjFolder(StorjAccount account, Bucket bucket, uplink.NET.Models.Object storjObject, string path) :
            base(CloudStorageProvider.Storj, path, null, storjObject is null ? bucket.Created : storjObject.SystemMetadata.Created, null, FileAttributes.Directory, (ulong?)storjObject?.SystemMetadata?.ContentLength)
        {
            Account = account;
            Bucket = bucket;
            StorjObject = storjObject;
            FolderType = FolderType.Regular;
        }

        public StorjAccount Account { get; }

        public Bucket Bucket { get; }

        public uplink.NET.Models.Object StorjObject { get; }
    }
}
