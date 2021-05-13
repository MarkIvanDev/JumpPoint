using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JumpPoint.Platform.Items.Storage;

namespace JumpPoint.Platform.Items.Arrangers.Sorters
{
    public class DateModifiedSorter : Sorter
    {
        public override IEnumerable<JumpPointItem> Sort(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            if (isAscending)
            {
                return items
                    .OrderBy(i => i.Type)
                    .ThenBy(i =>
                    {
                        return (i as StorageItemBase)?.DateModified ?? DateTimeOffset.MaxValue;
                    });
            }
            else
            {
                return items
                    .OrderByDescending(i => i.Type)
                    .ThenByDescending(i =>
                    {
                        return (i as StorageItemBase)?.DateModified ?? DateTimeOffset.MinValue;
                    });
            }
        }
    }
}
