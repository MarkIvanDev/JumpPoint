using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace JumpPoint.Extensions.AppLinkProviders
{
    public static class AppLinkProviderHelper
    {
        private static readonly List<string> appUriSchemes = new List<string> { "ms-appx", "ms-appx-web", "ms-appdata" };

        public static async Task<IList<AppLinkPayload>> GetPayloads(ValueSet data)
        {
            var payloads = new List<AppLinkPayload>();
            try
            {
                if (data.TryGetValue(nameof(AppLinkPayload), out var alp))
                {
                    var token = alp?.ToString();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var file = await SharedStorageAccessManager.RedeemTokenForFileAsync(token);
                        var text = await FileIO.ReadTextAsync(file);
                        payloads.AddRange(JsonConvert.DeserializeObject<List<AppLinkPayload>>(text));
                    }
                }
                return payloads;
            }
            catch (Exception)
            {
                return payloads;
            }
        }

        public static async Task<ValueSet> GetData(IList<AppLinkPayload> payloads)
        {
            var data = new ValueSet();
            try
            {
                if (payloads.Count > 0)
                {
                    var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(Path.GetRandomFileName(), CreationCollisionOption.GenerateUniqueName);
                    var json = JsonConvert.SerializeObject(payloads);
                    await file.WriteText(json);
                    var token = SharedStorageAccessManager.AddFile(file);
                    data.Add(nameof(AppLinkPayload), token);
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
            try
            {
                if (appUriSchemes.Contains(logoUri.Scheme))
                {
                    var file = await StorageFile.GetFileFromApplicationUriAsync(logoUri);
                    var stream = (await file.OpenReadAsync()).AsStream();
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
            catch (Exception)
            {
                return null;
            }
        }

    }
}
