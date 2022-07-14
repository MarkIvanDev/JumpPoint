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
        public static event EventHandler ExtensionCollectionChanged;

        public static async Task<IList<NewItem>> GetNewItems()
            => await PlatformGetNewItems();

        public static async Task Run(NewItem newItem, DirectoryBase destination)
            => await PlatformRun(newItem, destination);
    }
}
