using JumpPoint.Platform.Models;
using JumpPoint.Platform.Services;
using JumpPoint.ViewModels.Helpers;
using NittyGritty.Commands;
using NittyGritty.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace JumpPoint.ViewModels.Standalone
{
    public class ClipboardManagerViewModel : ViewModelBase
    {
        public ClipboardManagerViewModel()
        {
            Items = new ObservableRangeCollection<ClipboardItem>();
        }

        public ObservableRangeCollection<ClipboardItem> Items { get; }

        private RelayCommand<IList<ClipboardItem>> _Drop;
        public RelayCommand<IList<ClipboardItem>> DropCommand => _Drop ?? (_Drop = new RelayCommand<IList<ClipboardItem>>(
            items =>
            {
                if (items is null) return;
                Items.AddRange(items);
            }));

        private RelayCommand<IList<ClipboardItem>> _RemoveItems;
        public RelayCommand<IList<ClipboardItem>> RemoveItemsCommand => _RemoveItems ?? (_RemoveItems = new RelayCommand<IList<ClipboardItem>>(
            items =>
            {
                if (items is null) return;
                Items.RemoveRange(items);
            }));

        private RelayCommand _Clear;
        public RelayCommand ClearCommand => _Clear ?? (_Clear = new RelayCommand(
            () =>
            {
                Items.Clear();
            }));

        private RelayCommand<ClipboardItem> _DeleteClipboardItem;
        public RelayCommand<ClipboardItem> DeleteClipboardItemCommand => _DeleteClipboardItem ?? (_DeleteClipboardItem = new RelayCommand<ClipboardItem>(
            item =>
            {
                Items.Remove(item);
            }));

        public override void LoadState(object parameter, Dictionary<string, object> state)
        {
            
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            
        }
    }
}
