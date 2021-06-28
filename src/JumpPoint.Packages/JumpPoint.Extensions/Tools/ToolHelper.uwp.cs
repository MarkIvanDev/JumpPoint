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
    public static partial class ToolHelper
    {
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
