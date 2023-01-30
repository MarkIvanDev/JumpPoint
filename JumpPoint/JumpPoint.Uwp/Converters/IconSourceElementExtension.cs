using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace JumpPoint.Uwp.Converters
{
    [MarkupExtensionReturnType(ReturnType = typeof(IconSourceElement))]
    public sealed class IconSourceElementExtension : MarkupExtension
    {
        public IconSource Source { get; set; }

        protected override object ProvideValue()
        {
            return new IconSourceElement
            {
                IconSource = Source,
            };
        }
    }
}
