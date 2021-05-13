using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public interface ICloudStorageItem
    {
        CloudStorageProvider Provider { get; }
    }
}
