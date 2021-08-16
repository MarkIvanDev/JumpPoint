using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Models
{
    public class SidebarMessage
    {
        public SidebarMessage(CollectionChangedAction action, IList<JumpPointItem> items)
        {
            Action = action;
            Items = items ?? new List<JumpPointItem>();
        }

        public CollectionChangedAction Action { get; }

        public IList<JumpPointItem> Items { get; }
    }
}
