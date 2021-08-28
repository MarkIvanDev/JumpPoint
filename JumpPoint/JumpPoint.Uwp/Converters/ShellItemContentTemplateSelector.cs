using System;
using System.Text;
using JumpPoint.Platform.Items;
using NittyGritty.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Converters
{
    public class ShellItemContentTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Text { get; set; }

        public DataTemplate JumpPointItem { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case JumpPointItem _:
                    return JumpPointItem;

                case string _:
                    return Text;

                default:
                    return null;
            }
        }
    }
}
