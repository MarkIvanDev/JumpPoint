using JumpPoint.ViewModels.Dialogs.Clipboard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace JumpPoint.Uwp.Dialogs.Clipboard
{
    public sealed partial class MoveDialog : ContentDialog
    {
        public MoveDialog()
        {
            this.InitializeComponent();
            this.Closing += MoveDialog_Closing;
        }

        private void MoveDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            if (!ViewModel.CanClose)
            {
                args.Cancel = true;
            }
        }

        public MoveViewModel ViewModel => DataContext as MoveViewModel;
    }
}
