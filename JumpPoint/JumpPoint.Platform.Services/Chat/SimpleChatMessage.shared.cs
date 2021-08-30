using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Services
{
    public class SimpleChatMessage : ChatMessage
    {
        public SimpleChatMessage(ChatMessageSource source) : base(ChatMessageType.Simple, source)
        {
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }

    }
}
