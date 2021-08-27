using System;
using System.Text;
using JumpPoint.Platform.Items;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Helpers
{
    public class JumpPointItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FolderTemplate { get; set; }

        public DataTemplate FileTemplate { get; set; }

        public DataTemplate DriveTemplate { get; set; }

        public DataTemplate WorkspaceTemplate { get; set; }

        public DataTemplate LibraryTemplate { get; set; }

        public DataTemplate AppLinkTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if(item is JumpPointItem jpItem)
            {
                switch (jpItem.Type)
                {
                    case JumpPointItemType.File:
                        return FileTemplate;
                    case JumpPointItemType.Folder:
                        return FolderTemplate;
                    case JumpPointItemType.Drive:
                        return DriveTemplate;
                    case JumpPointItemType.Workspace:
                        return WorkspaceTemplate;
                    case JumpPointItemType.Library:
                        return LibraryTemplate;
                    case JumpPointItemType.AppLink:
                        return AppLinkTemplate;

                    case JumpPointItemType.Unknown:
                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
