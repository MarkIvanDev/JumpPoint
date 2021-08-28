using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.CloudStorage;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class CloudDriveGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i =>
                {
                    return i is ICloudStorageItem cloudItem ?
                        cloudItem.Provider :
                        CloudStorageProvider.Unknown;
                });

            groups = isAscending ?
                groups.OrderBy(g => g.Key != CloudStorageProvider.Unknown ? (int)g.Key : int.MaxValue) :
                groups.OrderByDescending(g => g.Key != CloudStorageProvider.Unknown ? (int)g.Key : int.MinValue);

            return groups.Select(g => new Group<string, JumpPointItem>(g.Key.Humanize(), g.ToList()));
        }
    }
}
