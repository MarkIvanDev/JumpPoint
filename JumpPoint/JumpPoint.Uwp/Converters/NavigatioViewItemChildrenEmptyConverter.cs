using NittyGritty.Models;
using NittyGritty.Uwp.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JumpPoint.Uwp.Converters
{
    public class NavigatioViewItemChildrenEmptyConverter : MultiConverter<IList<ShellItem>>
    {
        public override IList<ShellItem> Convert()
        {
            switch (Bindings[0].Value)
            {
                case int count when count > 0:
                    return Bindings[1].Value as IList<ShellItem>;
                default:
                    return null;
            }
        }
    }
}
