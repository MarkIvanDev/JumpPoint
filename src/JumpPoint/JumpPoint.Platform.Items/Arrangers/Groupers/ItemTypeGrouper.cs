using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class ItemTypeGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i => i.Type);

            groups = isAscending ?
                groups.OrderBy(g => g.Key != JumpPointItemType.Unknown ? (int)g.Key : int.MaxValue) :
                groups.OrderByDescending(g => g.Key != JumpPointItemType.Unknown ? (int)g.Key : int.MinValue);

            return groups.Select(g => new Group<string, JumpPointItem>(g.Key.Humanize(), g.ToList()));
        }
    }
}
