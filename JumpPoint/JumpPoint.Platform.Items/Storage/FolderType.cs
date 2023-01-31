using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.Storage
{
    public enum FolderType
    {
        [Description("File folder")]
        Regular = 0,
        [Description("User folder")]
        User = 1,
        [Description("System Folder")]
        System = 2
    }
}
