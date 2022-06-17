using GalaSoft.MvvmLight.Ioc;
using JumpPoint.Platform;
using JumpPoint.Platform.Interop;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Services;
using JumpPoint.Uwp.Helpers;
using JumpPoint.ViewModels;
using JumpPoint.ViewModels.Parameters;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.DragDrop;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JumpPoint.Uwp.Interactivity.Behaviors
{
    public class JumpPointViewerDragDropBehavior : Behavior<ListView>
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
                e.DataView.Contains(StandardDataFormats.StorageItems) &&
                Context != null && (Context.PathInfo.Type == AppPath.Drive || Context.PathInfo.Type == AppPath.Folder) &&
                Context.Item is DirectoryBase)
            {
                switch (e.Modifiers)
                {
                    case var _ when !e.Modifiers.HasControl() && !e.Modifiers.HasShift() && !e.Modifiers.HasAlt():
                    case var _ when e.Modifiers.HasControl() && !e.Modifiers.HasShift() && !e.Modifiers.HasAlt():
                        // No modifiers means copy. Using the Control modifier will always copy
                        e.AcceptedOperation = DataPackageOperation.Copy;
                        break;

                    case var _ when !e.Modifiers.HasControl() && e.Modifiers.HasShift() && !e.Modifiers.HasAlt():
                        // Using the Shift modifier will always move
                        e.AcceptedOperation = DataPackageOperation.Move;
                        break;

                    default:
                        e.AcceptedOperation = DataPackageOperation.None;
                        return;
                }
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

            if (e.DataView.Contains(StandardDataFormats.StorageItems) &&
                Context != null && (Context.PathInfo.Type == AppPath.Drive || Context.PathInfo.Type == AppPath.Folder) &&
                Context.Item is DirectoryBase destination)
            {
                switch (e.Modifiers)
                {
                    case var _ when !e.Modifiers.HasControl() && !e.Modifiers.HasShift() && !e.Modifiers.HasAlt():
                    case var _ when e.Modifiers.HasControl() && !e.Modifiers.HasShift() && !e.Modifiers.HasAlt():
                        // No modifiers means copy. Using the Control modifier will always copy
                        e.AcceptedOperation = DataPackageOperation.Copy;
                        e.Handled = true;
                        var toCopy = await ClipboardService.GetStorageItems(e.DataView);
                        CopyCommand?.Execute(new CopyToParameter(destination, toCopy));
                        break;

                    case var _ when !e.Modifiers.HasControl() && e.Modifiers.HasShift() && !e.Modifiers.HasAlt():
                        // Using the Shift modifier will always move
                        e.AcceptedOperation = DataPackageOperation.Move;
                        e.Handled = true;
                        var toMove = await ClipboardService.GetStorageItems(e.DataView);
                        MoveCommand?.Execute(new MoveToParameter(destination, toMove));
                        break;

                    default:
                        e.AcceptedOperation = DataPackageOperation.None;
                        break;
                }
            }
            else
            {
                e.AcceptedOperation = DataPackageOperation.None;
            }
            deferral.Complete();
        }

        private void AssociatedObject_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var items = e.Items.Cast<JumpPointItem>().ToList();
            if (items.Count > 0)
            {
                e.Data.SetDataProvider(StandardDataFormats.Text, (request) =>
                {
                    var deferral = request.GetDeferral();
                    try
                    {
                        var texts = items.Select(i => i.Path);
                        request.SetData(string.Join(SimpleIoc.Default.GetInstance<AppSettings>().CopyPathDelimiter.ToDelimiter(), texts));
                    }
                    catch (Exception)
                    {

                    }
                    finally
                    {
                        deferral.Complete();
                    }
                });

                var storageItems = items.OfType<StorageItemBase>().ToList();
                e.Data.SetDataProvider(StandardDataFormats.StorageItems, async (request) =>
                {
                    var deferral = request.GetDeferral();
                    try
                    {
                        var data = new List<IStorageItem>();
                        foreach (var item in storageItems)
                        {
                            switch (item)
                            {
                                case DirectoryBase directory:
                                    var storageFolder = await FileInterop.GetStorageFolder(directory);
                                    if (storageFolder != null)
                                    {
                                        data.Add(storageFolder);
                                    }
                                    break;

                                case FileBase file:
                                    var storageFile = await FileInterop.GetStorageFile(file);
                                    if (storageFile != null)
                                    {
                                        data.Add(storageFile);
                                    }
                                    break;
                            }
                        }
                        request.SetData(data);
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



        public ShellContextViewModelBase Context
        {
            get { return (ShellContextViewModelBase)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Context.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(ShellContextViewModelBase), typeof(JumpPointViewerDragDropBehavior), new PropertyMetadata(null));



        public ICommand CopyCommand
        {
            get { return (ICommand)GetValue(CopyCommandProperty); }
            set { SetValue(CopyCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CopyCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CopyCommandProperty =
            DependencyProperty.Register("CopyCommand", typeof(ICommand), typeof(JumpPointViewerDragDropBehavior), new PropertyMetadata(null));



        public ICommand MoveCommand
        {
            get { return (ICommand)GetValue(MoveCommandProperty); }
            set { SetValue(MoveCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MoveCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoveCommandProperty =
            DependencyProperty.Register("MoveCommand", typeof(ICommand), typeof(JumpPointViewerDragDropBehavior), new PropertyMetadata(null));



    }
}
