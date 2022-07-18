using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items;
using NittyGritty;

namespace JumpPoint.Platform.Models
{
    public class ItemStats : ObservableObject
    {
        private readonly IList<JumpPointItem> items;

        public ItemStats(IList<JumpPointItem> items)
        {
            this.items = items;
        }

        public void Refresh()
        {
            RaiseAllPropertiesChanged();
        }

        public int Total => items.Count;

        public int Drive => items.Count(i => i.Type == JumpPointItemType.Drive);

        public int Folder => items.Count(i => i.Type == JumpPointItemType.Folder);

        public int File => items.Count(i => i.Type == JumpPointItemType.File);

        public ulong FileSize => items.OfType<FileBase>().Aggregate(0UL, (s, i) => s + i.Size.GetValueOrDefault());

        public bool HasNullFileSize => items.OfType<FileBase>().Any(i => !i.Size.HasValue);

        public int Workspace => items.Count(i => i.Type == JumpPointItemType.Workspace);

        public int AppLink => items.Count(i => i.Type == JumpPointItemType.AppLink);

        // TODO: next update, add Libraries

    }

}
