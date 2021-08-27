using JumpPoint.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Uwp.Converters
{
    public static class NavigationViewItemConverter
    {
        public static bool SelectsOnInvoked(string key)
        {
            return key != ViewModelKeys.Chatbot && key != ViewModelKeys.ClipboardManager;
        }
    }
}
