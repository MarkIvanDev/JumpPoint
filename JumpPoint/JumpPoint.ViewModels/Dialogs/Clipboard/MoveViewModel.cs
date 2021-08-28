using JumpPoint.Platform.Items.Storage;
using JumpPoint.Platform.Services;
using NittyGritty;
using NittyGritty.Models;
using NittyGritty.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.ViewModels.Dialogs.Clipboard
{
    public class MoveViewModel : ObservableObject
    {
        private readonly IDialogService dialogService;

        public MoveViewModel(IDialogService dialogService, DirectoryBase destination, IList<StorageItemBase> items)
        {
            this.dialogService = dialogService;
            Destination = destination;
            Items = items;
            ProgressInfo = new ProgressInfo();
        }

        public DirectoryBase Destination { get; }

        public IList<StorageItemBase> Items { get; }

        public ProgressInfo ProgressInfo { get; }

        private StorageItemBase _currentItem;

        public StorageItemBase CurrentItem
        {
            get { return _currentItem; }
            set { Set(ref _currentItem, value); }
        }

        private bool _canClose;

        public bool CanClose
        {
            get { return _canClose; }
            set { Set(ref _canClose, value); }
        }

        public async Task Start()
        {
            CanClose = false;
            ProgressInfo.Start();
            for (int i = 0; i < Items.Count; i++)
            {
                CurrentItem = Items[i];
                await StorageService.MoveItem(Destination, CurrentItem);
                ProgressInfo.Update(Items.Count, i + 1, string.Empty);
            }
            ProgressInfo.Stop();
            CanClose = true;
            dialogService.HideAll();
        }

    }
}
