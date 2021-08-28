using System;
using System.Text;
using JumpPoint.Platform.Items;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Converters
{
    public static class AppLinkUtilityConverter
    {
        public static Visibility IsLaunchTypeVisible(AppLinkLaunchTypes launchTypes, AppLinkLaunchTypes flag)
        {
            return (launchTypes & flag) == flag ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
