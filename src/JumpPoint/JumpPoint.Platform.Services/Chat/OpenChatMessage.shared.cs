using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Services
{
    public class OpenChatMessage : ChatMessage
    {
        public OpenChatMessage(ChatMessageSource source) : base(ChatMessageType.Open, source)
        {
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private Uri _uri;

        public Uri Uri
        {
            get { return _uri; }
            set { Set(ref _uri, value); }
        }

    }
}
