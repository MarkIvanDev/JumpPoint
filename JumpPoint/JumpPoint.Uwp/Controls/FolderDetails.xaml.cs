using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Services;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace JumpPoint.Uwp.Controls
{
    public sealed partial class FolderDetails : UserControl
    {
        public FolderDetails()
        {
            this.InitializeComponent();
        }


        public FolderBase Folder
        {
            get { return (FolderBase)GetValue(FolderProperty); }
            set { SetValue(FolderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Folder.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FolderProperty =
            DependencyProperty.Register("Folder", typeof(FolderBase), typeof(FolderDetails), new PropertyMetadata(null, OnFolderChanged));

        private static async void OnFolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FolderDetails details)
            {
                await details.RefreshCount();
            }
        }

        public bool IsInDetailsPane
        {
            get { return (bool)GetValue(IsInDetailsPaneProperty); }
            set { SetValue(IsInDetailsPaneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsInDetailsPane.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsInDetailsPaneProperty =
            DependencyProperty.Register("IsInDetailsPane", typeof(bool), typeof(FolderDetails), new PropertyMetadata(false));

        private async Task RefreshCount()
        {
            if (Folder != null)
            {
                var children = await StorageService.GetItems(Folder);
                Folder.FolderCount = (ulong)children.Count(i => i.Type == JumpPointItemType.Folder);
                Folder.FileCount = (ulong)children.Count(i => i.Type == JumpPointItemType.File);
            }
        }
    }
}
