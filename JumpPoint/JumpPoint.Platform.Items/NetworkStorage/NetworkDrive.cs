using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items.NetworkStorage
{
    public class NetworkDrive : DriveBase, INetworkDirectory
    {
        public NetworkDrive(string name,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Network, name, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            DriveTemplate = DriveTemplate.Network;
        }
    }
}
