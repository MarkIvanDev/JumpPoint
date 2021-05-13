using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage
{
    public enum FolderType
    {
        Unknown = 0,
        [Description("File folder")]
        Regular = 1,
        [Description("User folder")]
        User = 2,
        [Description("System Folder")]
        System = 3
    }
}
