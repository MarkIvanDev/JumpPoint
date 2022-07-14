using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Extensions
{
    public static partial class NewItemManager
    {
        static Task<IList<NewItem>> PlatformGetNewItems()
            => throw new NotImplementedException();

        static Task PlatformRun(NewItem newItem, DirectoryBase destination)
            => throw new NotImplementedException();
    }
}
