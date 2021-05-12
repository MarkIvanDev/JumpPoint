using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class SizeGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i =>
                {
                    if (i is StorageItemBase storageItem && storageItem.Size.HasValue)
                    {
                        switch (storageItem.Size.Value)
                        {
                            case 0:
                                return SizeGroup.Empty;

                            case ulong tiny when tiny > 0 && tiny <= 16_384:
                                return SizeGroup.Tiny;

                            case ulong small when small > 16_384 && small <= 1_048_576:
                                return SizeGroup.Small;

                            case ulong medium when medium > 1_048_576 && medium <= 134_217_728:
                                return SizeGroup.Medium;

                            case ulong large when large > 134_217_728 && large <= 1_073_741_824:
                                return SizeGroup.Large;

                            case ulong huge when huge > 1_073_741_824 && huge <= 4_294_967_296:
                                return SizeGroup.Huge;

                            case ulong gigantic when gigantic > 4_294_967_296:
                                return SizeGroup.Gigantic;

                            default:
                                return SizeGroup.Unspecified;
                        }
                    }
                    else
                    {
                        return SizeGroup.Unspecified;
                    }
                });

            groups = isAscending ?
                groups.OrderBy(g => g.Key != SizeGroup.Unspecified ? (int)g.Key : int.MaxValue) :
                groups.OrderByDescending(g => g.Key != SizeGroup.Unspecified ? (int)g.Key : int.MinValue);
            
            return groups.Select(g => new Group<string, JumpPointItem>(g.Key.Humanize(), g.ToList()));
        }
    }

    public enum SizeGroup
    {
        Unspecified = 0,

        [Description("Empty (0 KB)")]
        Empty = 1,

        [Description("Tiny (0 - 16 KB)")]
        Tiny = 2,

        [Description("Small (16 KB - 1 MB)")]
        Small = 3,

        [Description("Medium (1 - 128 MB)")]
        Medium = 4,

        [Description("Large (128 MB - 1 GB)")]
        Large = 5,

        [Description("Huge (1 - 4 GB)")]
        Huge = 6,

        [Description("Gigantic (> 4 GB)")]
        Gigantic = 7
    }
}
