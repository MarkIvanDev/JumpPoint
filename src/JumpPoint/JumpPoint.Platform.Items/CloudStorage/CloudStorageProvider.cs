﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace JumpPoint.Platform.Items.CloudStorage
{
    public enum CloudStorageProvider
    {
        Unknown = 0,
        [Description("OneDrive")]
        OneDrive = 1
    }
}
