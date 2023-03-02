using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.Platform.Services
{
    public class ActionChatMessage : ChatMessage
    {
        public ActionChatMessage(ActionMessage action) : base(ChatMessageType.Action, ChatMessageSource.Bot)
        {
            Action = action;
        }

        public ActionMessage Action { get; }
    }

    public enum ActionMessage
    {
        Clear = 0,
    }
}
