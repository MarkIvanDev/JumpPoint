using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class DriveStorageTypeGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i =>
                {
                    return i is DriveBase drive ?
                        drive.StorageType :
                        (StorageType?)null;
                });

            groups = isAscending ?
                groups.OrderBy(g => g.Key.GetValueOrDefault((StorageType)int.MaxValue)) :
                groups.OrderByDescending(g => g.Key.GetValueOrDefault((StorageType)int.MinValue));

            return groups.Select(g => new Group<string, JumpPointItem>(g.Key?.Humanize() ?? "Unspecified", g.ToList()));
        }
    }
}
