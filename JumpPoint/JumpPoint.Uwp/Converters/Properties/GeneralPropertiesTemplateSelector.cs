using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Models;
using NittyGritty.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Converters.Properties
{
    public class GeneralPropertiesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DrivesTemplate { get; set; }

        public DataTemplate FoldersTemplate { get; set; }

        public DataTemplate FilesTemplate { get; set; }

        public DataTemplate WorkspacesTemplate { get; set; }

        public DataTemplate AppTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(item is Group<string, JumpPointItem> group)
            {
                switch (group.Key)
                {
                    case nameof(JumpPointItemType.Drive):
                        return DrivesTemplate;
                    case nameof(JumpPointItemType.Folder):
                        return FoldersTemplate;
                    case nameof(JumpPointItemType.File):
                        return FilesTemplate;
                    case nameof(JumpPointItemType.Workspace):
                        return WorkspacesTemplate;
                    case nameof(JumpPointItemType.AppLink):
                        return AppTemplate;
                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
