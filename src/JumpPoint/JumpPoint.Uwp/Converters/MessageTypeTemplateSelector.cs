using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Hosted;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Converters
{
    public class MessageTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Simple { get; set; }

        public DataTemplate Open { get; set; }

        public DataTemplate ItemList { get; set; }

        public DataTemplate ToolList { get; set; }

        public DataTemplate AppLinkProviderList { get; set; }

        public DataTemplate CommandList { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is ChatMessage message)
            {
                switch (message.Type)
                {
                    case ChatMessageType.Simple:
                        return Simple;

                    case ChatMessageType.Open:
                        return Open;

                    case ChatMessageType.ItemList:
                        return ItemList;

                    case ChatMessageType.ToolList:
                        return ToolList;

                    case ChatMessageType.AppLinkProviderList:
                        return AppLinkProviderList;

                    case ChatMessageType.CommandList:
                        return CommandList;

                    default:
                        return null;
                }
            }
            return null;
        }
    }
}
