using JumpPoint.Platform.Interop;
using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class ClipboardDragDropBehavior : Behavior<ListView>
    {
        protected override void OnAttached()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DragOver += AssociatedObject_DragOver;
                AssociatedObject.Drop += AssociatedObject_Drop;
                AssociatedObject.DragItemsStarting += AssociatedObject_DragItemsStarting;
                AssociatedObject.DragItemsCompleted += AssociatedObject_DragItemsCompleted;
            }
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            if (!(sender as ListView).IsDragSource() &&
                (e.DataView.Contains(StandardDataFormats.StorageItems) ||
                e.DataView.Contains(StandardDataFormats.Text) ||
                e.DataView.Contains(StandardDataFormats.Bitmap)))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
                e.DragUIOverride.Caption = "Add to Clipboard";
                e.DragUIOverride.IsCaptionVisible = true;
                e.DragUIOverride.IsContentVisible = true;
                e.DragUIOverride.IsGlyphVisible = true;
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
        }

        private async void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if ((sender as ListView).IsDragSource() || e.AllowedOperations == DataPackageOperation.None) return;

            var deferral = e.GetDeferral();
            var clipboardItems = await ClipboardService.GetClipboardItems(e.DataView);
            DropCommand?.Execute(clipboardItems);
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.Handled = true;
            deferral.Complete();
        }

        private void AssociatedObject_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var items = e.Items.Cast<ClipboardItem>().ToList();
            if (items.Count > 0)
            {
                e.Data.SetDataProvider(StandardDataFormats.StorageItems, async (request) =>
                {
                    var deferral = request.GetDeferral();
                    try
                    {
                        var storageItems = new List<IStorageItem>();
                        var textNumber = 1;
                        var bitmapNumber = 1;
                        foreach (var item in items)
                        {
                            switch (item)
                            {
                                case ClipboardFolderItem cbFolder:
                                    var folder = await FileInterop.GetStorageFolder(cbFolder.Folder);
                                    if (folder != null)
                                    {
                                        storageItems.Add(folder);
                                    }
                                    break;

                                case ClipboardFileItem cbFile:
                                    var file = await FileInterop.GetStorageFile(cbFile.File);
                                    if (file != null)
                                    {
                                        storageItems.Add(file);
                                    }
                                    break;

                                case ClipboardTextItem cbText:
                                    var textFile = await StorageFile.CreateStreamedFileAsync($"Text Document {textNumber}.txt",
                                        async (textRequest) =>
                                        {
                                            try
                                            {
                                                using (var stream = textRequest.AsStreamForWrite())
                                                using (var streamWriter = new StreamWriter(stream))
                                                {
                                                    await streamWriter.WriteAsync(cbText.Text);
                                                }
                                                textRequest.Dispose();
                                            }
                                            catch (Exception)
                                            {
                                                textRequest.FailAndClose(StreamedFileFailureMode.Failed);
                                            }
                                        }, null);
                                    storageItems.Add(textFile);
                                    textNumber += 1;
                                    break;

                                case ClipboardBitmapItem cbBitmap:
                                    var bitmapFile = await StorageFile.CreateStreamedFileAsync($"Image {bitmapNumber}.txt",
                                        async (bitmapRequest) =>
                                        {
                                            try
                                            {
                                                using (var stream = bitmapRequest.AsStreamForWrite())
                                                {
                                                    cbBitmap.Bitmap.Position = 0;
                                                    await cbBitmap.Bitmap.CopyToAsync(stream);
                                                }
                                                bitmapRequest.Dispose();
                                            }
                                            catch (Exception)
                                            {
                                                bitmapRequest.FailAndClose(StreamedFileFailureMode.Failed);
                                            }
                                        }, null);
                                    storageItems.Add(bitmapFile);
                                    bitmapNumber += 1;
                                    break;

                                default:
                                    break;
                            }
                        }

                        request.SetData(storageItems);
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        deferral.Complete();
                    }
                });
                e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
            }

        }

        private void AssociatedObject_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            RemoveItemsCommand?.Execute(args.Items.OfType<ClipboardItem>().ToList());
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.DragOver -= AssociatedObject_DragOver;
                AssociatedObject.Drop -= AssociatedObject_Drop;
                AssociatedObject.DragItemsStarting -= AssociatedObject_DragItemsStarting;
                AssociatedObject.DragItemsCompleted -= AssociatedObject_DragItemsCompleted;
            }
        }



        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DropCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(ClipboardDragDropBehavior), new PropertyMetadata(null));



        public ICommand RemoveItemsCommand
        {
            get { return (ICommand)GetValue(RemoveItemsCommandProperty); }
            set { SetValue(RemoveItemsCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RemoveItemsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RemoveItemsCommandProperty =
            DependencyProperty.Register("RemoveItemsCommand", typeof(ICommand), typeof(ClipboardDragDropBehavior), new PropertyMetadata(null));


    }
}
