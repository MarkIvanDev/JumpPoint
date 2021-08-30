using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NittyGritty.Collections;

namespace JumpPoint.Platform.Items.Arrangers.Groupers
{
    public class NameGrouper : Grouper
    {
        public override IEnumerable<Group<string, JumpPointItem>> Group(IEnumerable<JumpPointItem> items, bool isAscending)
        {
            var groups = items
                .GroupBy(i =>
                {
                    var firstChar = i.Name[0];
                    if (char.IsDigit(firstChar))
                    {
                        return "#";
                    }
                    else if (char.IsLetter(firstChar))
                    {
                        return char.ToUpper(firstChar).ToString();
                    }
                    else
                    {
                        return "&";
                    }
                });
            groups = isAscending ?
                groups.OrderBy(g => g.Key) :
                groups.OrderByDescending(g => g.Key);

            return groups.Select(g => new Group<string, JumpPointItem>(g.Key, g.ToList()));
        }
    }
}
