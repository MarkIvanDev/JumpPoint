using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.Storage;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class DateModifiedGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var utcNow = DateTimeOffset.UtcNow;
            var now = DateTimeOffset.Now;
            var groups = items
                .GroupBy(i =>
                {
                    if (i is StorageItemBase storageItem && storageItem.DateModified.HasValue)
                    {
                        var timeSince = storageItem.DateModified.Value.Offset == TimeSpan.Zero ?
                            utcNow.Subtract(storageItem.DateModified.Value) :
                            now.Subtract(storageItem.DateModified.Value);
                        switch (timeSince)
                        {
                            case TimeSpan past24hrs when past24hrs <= new TimeSpan(1, 0, 0, 0):
                                return DateGroup.Past24hrs;

                            case TimeSpan pastWeek when pastWeek <= new TimeSpan(7, 0, 0, 0):
                                return DateGroup.PastWeek;

                            case TimeSpan pastMonth when pastMonth <= new TimeSpan(30, 0, 0, 0):
                                return DateGroup.PastMonth;

                            case TimeSpan pastYear when pastYear <= new TimeSpan(365, 0, 0, 0):
                                return DateGroup.PastYear;

                            case TimeSpan pastWeek when pastWeek > new TimeSpan(365, 0, 0, 0):
                                return DateGroup.LongTimeAgo;

                            default:
                                return DateGroup.Unspecified;
                        }
                    }
                    else
                    {
                        return DateGroup.Unspecified;
                    }
                });

            groups = isAscending ?
                groups.OrderBy(g => g.Key != DateGroup.Unspecified ? (int)g.Key : int.MaxValue) :
                groups.OrderByDescending(g => g.Key != DateGroup.Unspecified ? (int)g.Key : int.MinValue);

            return groups.Select(g => new Group<string, JumpPointItem>(g.Key.Humanize(), g.ToList()));
        }
    }

    public enum DateGroup
    {
        Unspecified = 0,
        Past24hrs = 1,
        PastWeek = 2,
        PastMonth = 3,
        PastYear = 4,
        LongTimeAgo = 5
    }
}
