using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Extensions.Tools
{
    public static partial class ToolHelper
    {

        public static async Task<IList<ToolPayload>> GetPayloads(IDictionary<string, object> data)
        {
            var payloads = new List<ToolPayload>();
            try
            {
                if (data.TryGetValue(nameof(ToolPayload), out var tp))
                {
                    var items = await IOHelper.ReadItems<ToolPayload>(tp?.ToString());
                    payloads.AddRange(items);
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

        public static async Task<IList<ToolResultPayload>> GetResults(IDictionary<string, object> data)
        {
            var payloads = new List<ToolResultPayload>();
            try
            {
                if (data.TryGetValue(nameof(ToolResultPayload), out var tp))
                {
                    var items = await IOHelper.ReadItems<ToolResultPayload>(tp?.ToString());
                    payloads.AddRange(items);
                }
                else if (data.TryGetValue(nameof(ToolResultPayload.Result), out var r) &&
                    data.TryGetValue(nameof(ToolResultPayload.Path), out var p) &&
                    data.TryGetValue(nameof(ToolResultPayload.Message), out var m))
                {
                    payloads.Add(new ToolResultPayload
                    {
                        Result = Enum.TryParse<ToolResult>(r?.ToString(), true, out var result) ? result : ToolResult.Unknown,
                        Path = p?.ToString(),
                        Message = m?.ToString()
                    });
                }
                return payloads;
            }
            catch (Exception)
            {
                return payloads;
            }
        }

        public static async Task<IDictionary<string, object>> GetData(IList<ToolPayload> payloads)
        {
            var data = new Dictionary<string, object>();
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
                    data.Add(nameof(ToolPayload), await IOHelper.WriteItems(payloads));
                }
                return data;
            }
            catch (Exception)
            {
                return data;
            }
        }

        public static async Task<IDictionary<string, object>> GetData(IList<ToolResultPayload> results)
        {
            var data = new Dictionary<string, object>();
            try
            {
                if (results.Count == 1)
                {
                    data.Add(nameof(ToolResultPayload.Result), results[0].Result.ToString());
                    data.Add(nameof(ToolResultPayload.Path), results[0].Path);
                    data.Add(nameof(ToolResultPayload.Message), results[0].Message);
                }
                else if (results.Count > 1)
                {
                    data.Add(nameof(ToolResultPayload), await IOHelper.WriteItems(results));
                }
                return data;
            }
            catch (Exception)
            {
                return data;
            }
        }

    }
}
