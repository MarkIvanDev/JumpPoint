using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace JumpPoint.Extensions.Tools
{
    public static class ToolHelper
    {
        public static async Task<IList<ToolPayload>> GetPayloads(ValueSet data)
        {
            var payloads = new List<ToolPayload>();
            try
            {
                if (data.TryGetValue(nameof(ToolPayload), out var tp))
                {
                    var token = tp?.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                        var text = await FileIO.ReadTextAsync(file);
                        payloads.AddRange(JsonConvert.DeserializeObject<List<ToolPayload>>(text));
                    }
                }
                else if (data.TryGetValue(nameof(ToolPayload.ItemType), out var it) &&
                    data.TryGetValue(nameof(ToolPayload.Path), out var p) &&
                    data.TryGetValue(nameof(ToolPayload.Token), out var t))
                {
                    payloads.Add(new ToolPayload
                    {
                        ItemType = Enum.TryParse<ToolPayloadType>(it?.ToString(), true, out var itemType) ? itemType : ToolPayloadType.Unknown,
                        Path = p?.ToString(),
                        Token = t?.ToString()
                    });
                }
                return payloads;
            }
            catch (Exception)
            {
                return payloads;
            }
        }

        public static async Task<IList<ToolResultPayload>> GetResults(ValueSet data)
        {
            var payloads = new List<ToolResultPayload>();
            try
            {
                if (data.TryGetValue(nameof(ToolResultPayload), out var tp))
                {
                    var token = tp?.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                        var text = await FileIO.ReadTextAsync(file);
                        payloads.AddRange(JsonConvert.DeserializeObject<List<ToolResultPayload>>(text));
                    }
                }
                else if (data.TryGetValue(nameof(ToolResultPayload.Result), out var r) &&
                    data.TryGetValue(nameof(ToolResultPayload.Path), out var p))
                {
                    payloads.Add(new ToolResultPayload
                    {
                        Result = Enum.TryParse<ToolResult>(r?.ToString(), true, out var result) ? result : ToolResult.Unknown,
                        Path = p?.ToString()
                    });
                }
                return payloads;
            }
            catch (Exception)
            {
                return payloads;
            }
        }

        public static async Task<ValueSet> GetData(IList<ToolPayload> payloads)
        {
            var data = new ValueSet();
            try
            {
                if (payloads.Count == 1)
                {
                    data.Add(nameof(ToolPayload.ItemType), payloads[0].ItemType.ToString());
                    data.Add(nameof(ToolPayload.Path), payloads[0].Path);
                    data.Add(nameof(ToolPayload.Token), payloads[0].Token);
                }
                else
                {
                    var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
                    var json = JsonConvert.SerializeObject(payloads);
                    await file.WriteText(json);
                    var token = SharedStorageAccessManager.AddFile(file);
                    data.Add(nameof(ToolPayload), token);
                }
                return data;
            }
            catch (Exception)
            {
                return data;
            }
        }

        public static async Task<ValueSet> GetData(IList<ToolResultPayload> results)
        {
            var data = new ValueSet();
            try
            {
                if (results.Count == 1)
                {
                    data.Add(nameof(ToolResultPayload.Result), results[0].Result.ToString());
                    data.Add(nameof(ToolResultPayload.Path), results[0].Path);
                }
                else if (results.Count > 1)
                {
                    var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
                    var json = JsonConvert.SerializeObject(results);
                    await file.WriteText(json);
                    var token = SharedStorageAccessManager.AddFile(file);
                    data.Add(nameof(ToolResultPayload), token);
                }
                return data;
            }
            catch (Exception)
            {
                return data;
            }
        }

        public static async Task<StorageFile> GetFile(ToolPayload payload)
        {
            try
            {
                if (payload.ItemType != ToolPayloadType.File) return null;

                var file = !string.IsNullOrEmpty(payload.Token) ?
                    await SharedStorageAccessManager.RedeemTokenForFileAsync(payload.Token) :
                    await StorageFile.GetFileFromPathAsync(payload.Path);
                return file;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
