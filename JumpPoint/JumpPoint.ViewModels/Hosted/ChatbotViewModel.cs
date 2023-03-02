using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Commands;
using NittyGritty.Platform.Contacts;
using NittyGritty.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace JumpPoint.ViewModels.Hosted
{
    public class ChatbotViewModel : ViewModelBase
    {

        private readonly CommandHelper commandHelper;

        public ChatbotViewModel(CommandHelper commandHelper)
        {
            Messages = new ObservableCollection<ChatMessage>();
            this.commandHelper = commandHelper;
        }

        public ObservableCollection<ChatMessage> Messages { get; }

        private string _textMessage;

        public string TextMessage
        {
            get { return _textMessage ?? (_textMessage = string.Empty); }
            set { Set(ref _textMessage, value); }
        }

        private AsyncRelayCommand<string> _Execute;
        public AsyncRelayCommand<string> ExecuteCommand => _Execute ?? (_Execute = new AsyncRelayCommand<string>(
            async command =>
            {
                TextMessage = command;
                await SendCommand.TryExecute();
            }));

        private AsyncRelayCommand _Send;

        public AsyncRelayCommand SendCommand => _Send ?? (_Send = new AsyncRelayCommand(
            async () =>
            {
                var text = TextMessage;
                Messages.Add(new SimpleChatMessage(ChatMessageSource.User)
                {
                    Message = text
                });
                TextMessage = null;
                var response = await ChatService.GetResponse(text);
                Messages.Add(response);
                if (response is OpenChatMessage open)
                {
                    await commandHelper.OpenUriCommand.TryExecute(open.Uri);
                }
                else if (response is ActionChatMessage action)
                {
                    switch (action.Action)
                    {
                        case ActionMessage.Clear:
                            Messages.Clear();
                            break;

                        default:
                            break;
                    }
                }
            }));

        public override void LoadState(object parameter, Dictionary<string, object> state)
        {
            if (Messages.Count == 0)
            {
                Messages.Add(new CommandListChatMessage(ChatMessageSource.Bot)
                {
                    Title = "Hi there! Start by typing a command or",
                    Items =
                    {
                        CommandInfo.HelpCommands
                    }
                });
            }
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            
        }
    }

}
