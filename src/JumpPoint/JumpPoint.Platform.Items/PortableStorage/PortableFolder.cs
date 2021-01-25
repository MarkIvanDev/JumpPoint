using System;
using System.IO;
using System.Text;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Items.PortableStorage
{
    public class PortableFolder : FolderBase, IPortableDirectory
    {
        public PortableFolder(IFolder context,
            string path, DateTimeOffset? dateAccessed, DateTimeOffset? dateCreated, DateTimeOffset? dateModified, FileAttributes? attributes, ulong? size) :
            base(StorageType.Portable, path, dateAccessed, dateCreated, dateModified, attributes, size)
        {
            Context = context;
        }

        public IFolder Context { get; }

    }
}
