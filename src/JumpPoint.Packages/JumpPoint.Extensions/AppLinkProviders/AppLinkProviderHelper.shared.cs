using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Extensions.AppLinkProviders
{
    public static partial class AppLinkProviderHelper
    {
        public static async Task<IList<AppLinkPayload>> GetPayloads(IDictionary<string, object> data)
        {
            var payloads = new List<AppLinkPayload>();
            try
            {
                if (data.TryGetValue(nameof(AppLinkPayload), out var alp))
                {
                    var items = await IOHelper.ReadItems<AppLinkPayload>(alp?.ToString());
                    payloads.AddRange(items);
                }
                return payloads;
            }
            catch (Exception)
            {
                return payloads;
            }
        }

        public static async Task<IDictionary<string, object>> GetData(IList<AppLinkPayload> payloads)
        {
            var data = new Dictionary<string, object>();
            try
            {
                if (payloads.Count > 0)
                {
                    data.Add(nameof(AppLinkPayload), await IOHelper.WriteItems(payloads));
                }
                return data;
            }
            catch (Exception)
            {
                return data;
            }
        }

        public static async Task<byte[]> GetLogo(Uri logoUri)
        {
            var stream = await IOHelper.GetStream(logoUri);
            if (stream != null)
            {
                using (var memory = new MemoryStream())
                {
                    stream.Position = 0;
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
            else
            {
                return null;
            }
        }

    }
}
