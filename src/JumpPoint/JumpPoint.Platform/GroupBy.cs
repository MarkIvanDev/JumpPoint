using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform
{
    public enum GroupBy
    {
        None = 0,
        Name = 1,
        DateModified = 2,
        DisplayType = 3,
        Size = 4,
        ItemType = 5,

        Custom = int.MaxValue
    }
}
