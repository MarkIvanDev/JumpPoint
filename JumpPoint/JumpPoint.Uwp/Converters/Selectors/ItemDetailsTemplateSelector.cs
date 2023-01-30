using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Converters.Selectors
{
    public class ItemDetailsTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Drive { get; set; }

        public DataTemplate Folder { get; set; }

        public DataTemplate File { get; set; }

        public DataTemplate Workspace { get; set; }

        public DataTemplate AppLink { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch ((item as JumpPointItem)?.Type)
            {
                case JumpPointItemType.Drive:
                    return Drive;

                case JumpPointItemType.Folder:
                    return Folder;

                case JumpPointItemType.File:
                    return File;

                case JumpPointItemType.Workspace:
                    return Workspace;

                case JumpPointItemType.AppLink:
                    return AppLink;

                case JumpPointItemType.Library:
                case JumpPointItemType.Unknown:
                default:
                    return null;
            }
        }
    }
}
