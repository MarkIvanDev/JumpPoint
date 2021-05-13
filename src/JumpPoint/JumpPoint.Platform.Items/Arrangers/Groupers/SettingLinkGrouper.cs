using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Humanizer;
using JumpPoint.Platform.Items.Templates;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class SettingLinkGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i =>  i is SettingLink settingLink ? settingLink.Group : SettingLinkGroup.None);

            groups = isAscending ?
                groups.OrderBy(g => g.Key) :
                groups.OrderByDescending(g => g.Key);

            return groups.Select(g => new Group<string, JumpPointItem>(g.Key.Humanize(), g.ToList()));
        }
    }
}
