using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using JumpPoint.Extensions.Tools;
using JumpPoint.Platform;
using JumpPoint.Platform.Extensions;
using NittyGritty;
using NittyGritty.Commands;
using NittyGritty.Models;
using NittyGritty.Platform.Payloads;
using NittyGritty.ViewModels;

namespace JumpPoint.ViewModels.Hosted
{
    public class HashToolViewModel : ViewModelBase
    {
        private IProtocolForResultsPayload protocolForResultsPayload = null;

        public HashToolViewModel()
        {

        }

        private HashFunction _hashFunction;

        public HashFunction HashFunction
        {
            get { return _hashFunction; }
            set { Set(ref _hashFunction, value); }
        }

        private Collection<HashItem> _hashItems;

        public Collection<HashItem> HashItems
        {
            get { return _hashItems ?? (_hashItems = new Collection<HashItem>()); }
            set { Set(ref _hashItems, value); }
        }

        private AsyncRelayCommand _Compute;
        public AsyncRelayCommand ComputeCommand => _Compute ?? (_Compute = new AsyncRelayCommand(
            async () =>
            {
                if (HashFunction != HashFunction.Unknown)
                {
                    foreach (var item in HashItems)
                    {
                        var progress = new Progress<double?>(value =>
                        {
                            var currentValue = value.GetValueOrDefault();
                            if (currentValue == 0 || currentValue - item.Progress.GetValueOrDefault() > 0.01)
                            {
                                item.Progress = value;
                            }
                        });
                        item.Hash = await HashTool.ComputeHash(HashFunction, item.Payload, progress);
                    }
                }
            }));

        public override async void LoadState(object parameter, Dictionary<string, object> state)
        {
            IsLoading = true;
            if (parameter is IProtocolForResultsPayload payload)
            {
                protocolForResultsPayload = payload;
                var query = QueryString.Parse(payload.Uri.Query.TrimStart('?'));
                HashFunction = query.TryGetValue("function", out var fn) && Enum.TryParse<HashFunction>(fn, true, out var function) ?
                    function : HashFunction.Unknown;
                if (HashFunction != HashFunction.Unknown)
                {
                    var toolPayload = await ToolHelper.GetPayloads(payload.Data);
                    HashItems = new Collection<HashItem>(toolPayload.Select(i => new HashItem { Payload = i }).ToList());
                }
            }
            IsLoading = false;
            await ComputeCommand.TryExecute();
        }

        public override void SaveState(Dictionary<string, object> state)
        {
            
        }
    }

    public class HashItem : ObservableObject
    {

        private ToolPayload _payload;

        public ToolPayload Payload
        {
            get { return _payload; }
            set { Set(ref _payload, value); }
        }

        private double? _progress;

        public double? Progress
        {
            get { return _progress; }
            set { Set(ref _progress, value); }
        }

        private string _hash;

        public string Hash
        {
            get { return _hash; }
            set { Set(ref _hash, value); }
        }

    }
}
