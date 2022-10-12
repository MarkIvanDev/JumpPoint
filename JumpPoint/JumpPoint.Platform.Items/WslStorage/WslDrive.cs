using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Items.WslStorage
{
    public class WslDrive : DriveBase, IWslDirectory
    {
        public WslDrive(WslDistro distro, IFolder context, string name,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.WSL, name, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Distro = distro;
            Context = context;
            DriveTemplate = distro.ToDriveTemplate();
        }

        public WslDistro Distro { get; }

        public IFolder Context { get; set; }
    }
}
