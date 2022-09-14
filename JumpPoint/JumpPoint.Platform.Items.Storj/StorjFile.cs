using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.CloudStorage;
using uplink.NET.Models;

namespace JumpPoint.Platform.Items.Storj
{
    public class StorjFile : CloudFile
    {
        public StorjFile(StorjAccount account, Bucket bucket, uplink.NET.Models.Object storjObject, string path) :
            base(CloudStorageProvider.Storj, path, null, storjObject.SystemMetadata.Created, null, FileAttributes.Normal, (ulong?)storjObject.SystemMetadata.ContentLength)
        {
            Account = account;
            Bucket = bucket;
            StorjObject = storjObject;
        }

        public StorjAccount Account { get; }

        public Bucket Bucket { get; }

        public uplink.NET.Models.Object StorjObject { get; }
    }
}
