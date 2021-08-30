using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Items.Arrangers.Sorters
{
    public abstract class Sorter
    {
        public abstract IEnumerable<JumpPointItem> Sort(IEnumerable<JumpPointItem> items, bool isAscending);

        public static Sorter Name { get; } = new NameSorter();

        public static Sorter DateModified { get; } = new DateModifiedSorter();

        public static Sorter DisplayType { get; } = new DisplayTypeSorter();

        public static Sorter Size { get; } = new SizeSorter();

        public static Sorter GetSorter(SortBy sortBy)
        {
            switch (sortBy)
            {
                case SortBy.Name:
                default:
                    return Name;

                case SortBy.DateModified:
                    return DateModified;

                case SortBy.DisplayType:
                    return DisplayType;

                case SortBy.Size:
                    return Size;
            }
        }
    }
}
