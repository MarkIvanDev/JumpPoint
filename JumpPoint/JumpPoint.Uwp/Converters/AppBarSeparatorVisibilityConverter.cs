using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NittyGritty.Uwp.Converters;
using Windows.UI.Xaml;

namespace JumpPoint.Uwp.Converters
{
    public class AppBarSeparatorVisibilityConverter : MultiConverter<Visibility>
    {
        public override Visibility Convert()
        {
            var isEnabled = new List<bool>();
            foreach (var item in Bindings)
            {
                if(item.Value is bool b)
                {
                    isEnabled.Add(b);
                }
            }
            if(isEnabled.Count == 0 || isEnabled.Any(i => i == true))
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }
    }
}
