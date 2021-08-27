using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace JumpPoint.Uwp.Controls
{
    public sealed class MarkdownViewer : Control
    {
        public MarkdownViewer()
        {
            this.DefaultStyleKey = typeof(MarkdownViewer);
        }


        public string Markdown
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Markdown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Markdown", typeof(string), typeof(MarkdownViewer), new PropertyMetadata(null));



    }
}
