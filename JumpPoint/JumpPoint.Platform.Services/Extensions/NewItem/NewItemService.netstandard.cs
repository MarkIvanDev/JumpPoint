using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Services
{
    public static partial class NewItemService
    {
        static Task<IList<NewItem>> PlatformGetNewItems()
            => throw new NotImplementedException();

        static Task<bool> PlatformRun(NewItem newItem, DirectoryBase destination)
            => throw new NotImplementedException();
    }
}
