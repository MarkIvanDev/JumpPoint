using NittyGritty;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Services
{
    public abstract class ChatMessage : ObservableObject
    {
        public ChatMessage(ChatMessageType type, ChatMessageSource source)
        {
            Type = type;
            Source = source;
        }

        public ChatMessageType Type { get; }

        public ChatMessageSource Source { get; }
    }

    public enum ChatMessageType
    {
        Simple = 0,
        Open = 1,
        ItemList = 2,
        ToolList = 3,
        AppLinkProviderList = 4,
        CommandList = 5
    }

    public enum ChatMessageSource
    {
        User = 0,
        Bot = 1
    }
}
