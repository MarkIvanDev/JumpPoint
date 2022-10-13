using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty.Platform.Storage;

namespace JumpPoint.Platform.Items.WslStorage
{
    public interface IWslDirectory
    {
        string Path { get; }

        IFolder Context { get; }
    }
}
