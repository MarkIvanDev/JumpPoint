using JumpPoint.Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Converters
{
    public static class ChatMessageConverter
    {
        public static Thickness GetMargin(ChatMessageSource source)
        {
            switch (source)
            {
                case ChatMessageSource.User:
                    return new Thickness(48, 4, 0, 4);

                case ChatMessageSource.Bot:
                    return new Thickness(0, 4, 48, 4);

                default:
                    return new Thickness();
            }
        }

        public static HorizontalAlignment GetHorizontalAlignment(ChatMessageSource source)
        {
            switch (source)
            {
                case ChatMessageSource.User:
                    return HorizontalAlignment.Right;

                case ChatMessageSource.Bot:
                    return HorizontalAlignment.Left;

                default:
                    return HorizontalAlignment.Stretch;
            }
        }

        public static string IsEmpty(string text)
        {
            return (!string.IsNullOrEmpty(text)).ToString();
        }
    }
}
