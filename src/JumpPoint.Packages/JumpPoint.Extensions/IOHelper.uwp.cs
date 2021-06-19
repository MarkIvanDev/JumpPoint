using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JumpPoint.Extensions
{
    public static class IOHelper
    {

        public static async Task WriteText(this StorageFile file, string text)
        {
            int retryAttempts = 3;
            const int ERROR_ACCESS_DENIED = unchecked((int)0x80070005);
            const int ERROR_SHARING_VIOLATION = unchecked((int)0x80070020);
            const int ERROR_UNABLE_TO_REMOVE_REPLACED = unchecked((int)0x80070497);

            // Application now has read/write access to the picked file.
            while (retryAttempts > 0)
            {
                try
                {
                    retryAttempts--;
                    await FileIO.WriteTextAsync(file, text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                    break;
                }
                catch (Exception ex) when ((ex.HResult == ERROR_ACCESS_DENIED) ||
                                           (ex.HResult == ERROR_SHARING_VIOLATION) ||
                                           (ex.HResult == ERROR_UNABLE_TO_REMOVE_REPLACED))
                {
                    // This might be recovered by retrying, otherwise let the exception be raised.
                    // The app can decide to wait before retrying.
                }
            }
        }

    }
}
