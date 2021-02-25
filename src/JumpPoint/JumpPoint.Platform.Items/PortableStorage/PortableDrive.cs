using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Items.Templates;
using NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Items.PortableStorage
{
    public class PortableDrive : DriveBase, IPortableDirectory
    {
        public PortableDrive(string name, IFolder context,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Portable, name, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Context = context;
            DriveTemplate = DriveTemplate.Removable;
        }

        public IFolder Context { get; }

        private string _deviceId;

        public string DeviceId
        {
            get { return _deviceId; }
            set { Set(ref _deviceId, value); }
        }

    }
}
