using JumpPoint.Platform.Items.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.ViewModels.Parameters
{
    public class CopyToParameter
    {
        public CopyToParameter(DirectoryBase destination, IList<StorageItemBase> items)
        {
            Destination = destination;
            Items = items;
        }

        public DirectoryBase Destination { get; }

        public IList<StorageItemBase> Items { get; }
    }
}
