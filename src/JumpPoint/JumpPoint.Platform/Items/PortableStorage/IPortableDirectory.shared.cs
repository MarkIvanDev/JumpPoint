using System;
using System.Text;
using NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Items.PortableStorage
{
    public interface IPortableDirectory
    {
        string Path { get; }

        IFolder Context { get; }
    }
}
