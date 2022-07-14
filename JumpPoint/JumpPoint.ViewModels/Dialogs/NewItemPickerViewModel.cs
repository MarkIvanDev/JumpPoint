using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Extensions;
using NittyGritty;

namespace JumpPoint.ViewModels.Dialogs
{
    public class NewItemPickerViewModel : ObservableObject
    {
        public NewItemPickerViewModel()
        {

        }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private Collection<NewItem> _newItems;

        public Collection<NewItem> NewItems
        {
            get { return _newItems; }
            set { Set(ref _newItems, value); }
        }

        private NewItem _newItem;

        public NewItem NewItem
        {
            get { return _newItem; }
            set
            {
                Set(ref _newItem, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid => NewItem != null;

        public async Task Initialize()
        {
            IsLoading = true;
            var newItems = await NewItemManager.GetNewItems();
            var supportedTools = newItems.Where(i => i.IsAvailable && i.IsEnabled);
            NewItems = new Collection<NewItem>(newItems.Where(i => i.IsAvailable && i.IsEnabled).ToList());
            IsLoading = false;
        }
    }
}
