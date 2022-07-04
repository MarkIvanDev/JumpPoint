using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace JumpPoint.Uwp.Converters
{
    public class ChatMessageBackgroundConverter : DependencyObject
    {


        public ChatMessageSource Source
        {
            get { return (ChatMessageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ChatMessageSource), typeof(ChatMessageBackgroundConverter), new PropertyMetadata(ChatMessageSource.User, OnPropertyChanged));



        public Brush UserBackground
        {
            get { return (Brush)GetValue(UserBackgroundProperty); }
            set { SetValue(UserBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserBackgroundProperty =
            DependencyProperty.Register("UserBackground", typeof(Brush), typeof(ChatMessageBackgroundConverter), new PropertyMetadata(null, OnPropertyChanged));



        public Brush BotBackground
        {
            get { return (Brush)GetValue(BotBackgroundProperty); }
            set { SetValue(BotBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BotBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BotBackgroundProperty =
            DependencyProperty.Register("BotBackground", typeof(Brush), typeof(ChatMessageBackgroundConverter), new PropertyMetadata(null, OnPropertyChanged));



        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            private set { SetValue(BackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(ChatMessageBackgroundConverter), new PropertyMetadata(null));



        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ChatMessageBackgroundConverter converter)
            {
                switch (converter.Source)
                {
                    case ChatMessageSource.Bot:
                        converter.Background = converter.BotBackground;
                        break;

                    case ChatMessageSource.User:
                    default:
                        converter.Background = converter.UserBackground;
                        break;
                }
            }
        }
    }
}
