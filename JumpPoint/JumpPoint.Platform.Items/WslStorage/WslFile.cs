using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Items.WslStorage
{
    public class WslFile : FileBase
    {
        public WslFile(WslDistro distro, IFile context,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.WSL, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Distro = distro;
            Context = context;
        }

        public WslDistro Distro { get; }

        public IFile Context { get; }
    }
}
