using JumpPoint.Platform.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.ViewModels.Helpers
{
    public class OpenItemParameter
    {
        public OpenItemParameter(TabbedNavigationHelper navigationHelper, JumpPointItem item)
        {
            NavigationHelper = navigationHelper;
            Item = item;
        }

        public TabbedNavigationHelper NavigationHelper { get; }

        public JumpPointItem Item { get; }
    }
}
