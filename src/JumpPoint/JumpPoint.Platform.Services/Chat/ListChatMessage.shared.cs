using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Services
{
    public class ListChatMessage<T> : ChatMessage
    {
        public ListChatMessage(ChatMessageType type, ChatMessageSource source) : base(type, source)
        {
            Items = new List<T>();
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        public IList<T> Items { get; }
    }

    public class ItemListChatMessage : ListChatMessage<JumpPointItem>
    {
        public ItemListChatMessage(ChatMessageSource source) : base(ChatMessageType.ItemList, source)
        {
        }
    }

    public class ToolListChatMessage : ListChatMessage<Tool>
    {
        public ToolListChatMessage(ChatMessageSource source) : base(ChatMessageType.ToolList, source)
        {
        }
    }

    public class AppLinkProviderListChatMessage : ListChatMessage<AppLinkProvider>
    {
        public AppLinkProviderListChatMessage(ChatMessageSource source) : base(ChatMessageType.AppLinkProviderList, source)
        {
        }
    }

    public class CommandListChatMessage : ListChatMessage<string>
    {
        public CommandListChatMessage(ChatMessageSource source) : base(ChatMessageType.CommandList, source)
        {
        }
    }

}
