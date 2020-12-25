using System;
using System.Collections.Generic;
using System.Text;
using JumpPoint.Platform.Items.Templates;

namespace JumpPoint.Platform.Items
{
    public class SettingLink : JumpPointItem
    {
        public SettingLink(string name, string path, SettingLinkTemplate template, SettingLinkGroup group) :
            base(JumpPointItemType.SettingLink)
        {
            Name = name;
            DisplayName = name;
            Path = path;
            Template = template;
            Group = group;
            DisplayType = "Setting Link";
        }

        public SettingLinkTemplate Template { get; }

        public SettingLinkGroup Group { get; }

    }
}
