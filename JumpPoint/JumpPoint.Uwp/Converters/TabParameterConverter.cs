using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Models.Extensions;
using NittyGritty.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace JumpPoint.Uwp.Converters
{
    public static class TabParameterConverter
    {
        public static IList<PageStackEntry> ReverseStack(IList<PageStackEntry> stack)
        {
            return stack.Reverse().ToList();
        }

        public static string GetPath(object parameter)
        {
            var tabParameter = TabParameter.FromJson(parameter?.ToString());
            if (tabParameter != null)
            {
                var query = QueryString.Parse(tabParameter.Parameter);
                return query.TryGetValue(nameof(PathInfo.Path), out var path) ? path : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetDisplayName(object parameter)
        {
            var path = GetPath(parameter);
            return path.GetBreadcrumbs().LastOrDefault()?.DisplayName ?? string.Empty;
        }

        public static IconSource GetIconSource(object parameter)
        {
            var path = GetPath(parameter);
            return IconConverter.GetPathTypeIconSource(path.GetBreadcrumbs().LastOrDefault()?.AppPath, null);
        }
    }
}
