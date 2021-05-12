using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JumpPoint.Platform.Items.Arrangers.Sorters
{
    public class NameSorter : Sorter
    {
        public override IEnumerable<JumpPointItem> Sort(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            if (isAscending)
            {
                return items
                    .OrderBy(i => i.Type)
                    .ThenBy(i => i.Name);
            }
            else
            {
                return items
                    .OrderByDescending(i => i.Type)
                    .ThenByDescending(i => i.Name);
            }
        }
    }
}
