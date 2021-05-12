using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class DisplayTypeGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i =>
                {
                    switch (i)
                    {
                        case FolderBase folder:
                            return folder.FolderType.Humanize();

                        case DriveBase drive:
                            return drive.StorageType.Humanize();

                        default:
                            return i.DisplayType;
                    }
                });

            groups = isAscending ?
                groups.OrderBy(g => g.Key) :
                groups.OrderByDescending(g => g.Key);
                
            return groups.Select(g => new Group<string, JumpPointItem>(g.Key, g.ToList()));
        }
    }
}
