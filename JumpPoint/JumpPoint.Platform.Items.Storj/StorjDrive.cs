﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
using uplink.NET.Models;

namespace JumpPoint.Platform.Items.Storj
{
    public class StorjDrive : CloudDrive
    {
        public StorjDrive(StorjAccount account) :
            base(CloudStorageProvider.Storj, $"{account.Name} ({account.Email})", $@"cloud:\Storj\{account.Name}",
                null, null, null, FileAttributes.Directory, null)
        {
            Account = account;
            FileSystem = CloudStorageProvider.Storj.Humanize();
        }

        public StorjAccount Account { get; }

    }
}
