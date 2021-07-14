using System;
using System.Collections.Generic;
using System.Text;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public abstract class Grouper
    {
        public abstract IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending);

        public static Grouper Name { get; } = new NameGrouper();

        public static Grouper DateModified { get; } = new DateModifiedGrouper();

        public static Grouper DisplayType { get; } = new DisplayTypeGrouper();

        public static Grouper Size { get; } = new SizeGrouper();

        public static Grouper ItemType { get; } = new ItemTypeGrouper();

        public static Grouper Dashboard { get; } = new DashboardGrouper();

        public static Grouper DriveStorageType { get; } = new DriveStorageTypeGrouper();

        public static Grouper CloudDrive { get; } = new CloudDriveGrouper();

        public static Grouper GetGrouper(GroupBy groupBy, Grouper customGrouper = null)
        {
            switch (groupBy)
            {
                case GroupBy.Name:
                    return Name;

                case GroupBy.DateModified:
                    return DateModified;

                case GroupBy.DisplayType:
                    return DisplayType;

                case GroupBy.Size:
                    return Size;

                case GroupBy.ItemType:
                    return ItemType;

                case GroupBy.Custom:
                    return customGrouper;

                case GroupBy.None:
                default:
                    return null;
            }
        }

    }
}
