﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Items.NetworkStorage
{
    public class NetworkFolder : FolderBase, INetworkDirectory
    {
        public NetworkFolder(
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Network, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
        }
    }
}
