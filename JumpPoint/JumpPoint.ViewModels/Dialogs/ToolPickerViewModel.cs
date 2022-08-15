using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items;
using JumpPoint.Platform.Services;
using NittyGritty;
using NittyGritty.Collections;

namespace JumpPoint.ViewModels.Dialogs
{
    public class ToolPickerViewModel : ObservableObject
    {

        public ToolPickerViewModel(IList<JumpPointItem> items)
        {
            Items = items;
        }

        public IList<JumpPointItem> Items { get; }

        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set { Set(ref _isLoading, value); }
        }

        private Collection<Group<string, Tool>> _tools;

        public Collection<Group<string, Tool>> Tools
        {
            get { return _tools; }
            set { Set(ref _tools, value); }
        }

        private Tool _tool;

        public Tool Tool
        {
            get { return _tool; }
            set
            {
                Set(ref _tool, value);
                RaisePropertyChanged(nameof(IsValid));
            }
        }

        public bool IsValid
        {
            get { return Tool != null; }
        }

        public async Task Initialize()
        {
            IsLoading = true;
            var tools = await ToolService.GetTools();
            var supportedTools = new List<Tool>();
            foreach (var tool in tools)
            {
                if (tool.IsAvailable && tool.IsEnabled && tool.IsSupported(Items))
                {
                    supportedTools.Add(tool);
                }
            }
            Tools = new Collection<Group<string, Tool>>(supportedTools
                .GroupBy(t => t.Group)
                .Select(g => new Group<string, Tool>(g.Key, g.ToList()))
                .ToList());
            IsLoading = false;
        }

    }
}
