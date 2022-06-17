using JumpPoint.Platform;
using JumpPoint.Uwp.Helpers;
using NittyGritty.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;

namespace JumpPoint.Uwp
{
    public static class Program
    {
        static void Main(string[] args)
        {
            if (AppInstance.RecommendedInstance != null)
            {
                AppInstance.RecommendedInstance.RedirectActivationTo();
            }
            else
            {
                var eventArgs = AppInstance.GetActivatedEventArgs();
                var key = GetKey(eventArgs);
                var instance = AppInstance.FindOrRegisterInstanceForKey(key);
                if (instance.IsCurrentInstance)
                {
                    // If we successfully registered this instance, we can now just
                    // go ahead and do normal XAML initialization.
                    global::Windows.UI.Xaml.Application.Start((p) => new App());
                }
                else
                {
                    // Some other instance has registered for this key, so we'll 
                    // redirect this activation to that instance instead.
                    instance.RedirectActivationTo();
                }
            }
        }

        static string GetKey(IActivatedEventArgs args)
        {
            switch (args)
            {
                case ProtocolActivatedEventArgs protocolArgs when EnumHelper<ProtocolPath>.ParseOrDefault(protocolArgs.Uri.Host, ignoreCase: true) == ProtocolPath.Chat:
                case CommandLineActivatedEventArgs commandLineArgs when ActivationHelper.GetAppPath(commandLineArgs) == AppPath.Chat:
                    return $"{Prefix.MAIN_SCHEME}_CHAT";

                case ProtocolActivatedEventArgs protocolArgs when EnumHelper<ProtocolPath>.ParseOrDefault(protocolArgs.Uri.Host, ignoreCase: true) == ProtocolPath.Clipboard:
                case CommandLineActivatedEventArgs commandLineArgs when ActivationHelper.GetAppPath(commandLineArgs) == AppPath.ClipboardManager:
                    return $"{Prefix.MAIN_SCHEME}_CLIPBOARD";

                default:
                    return Guid.NewGuid().ToString();
            }
        }
    }
}
