using JumpPoint.Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Converters.Selectors
{
    public class ClipboardItemTemplateSelector : DataTemplateSelector
    {

        public DataTemplate Folder { get; set; }

        public DataTemplate File { get; set; }

        public DataTemplate Text { get; set; }

        public DataTemplate Bitmap { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch ((item as ClipboardItem)?.Type)
            {
                case ClipboardItemType.Folder:
                    return Folder;

                case ClipboardItemType.File:
                    return File;

                case ClipboardItemType.Text:
                    return Text;

                case ClipboardItemType.Bitmap:
                    return Bitmap;

                case ClipboardItemType.Unknown:
                default:
                    return null;
            }
        }
    }
}
