using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Extensions
{
    public static partial class IOHelper
    {
        public static async Task<IList<T>> ReadItems<T>(string token)
            => await PlatformReadItems<T>(token);

        public static async Task<string> WriteItems<T>(IList<T> items)
            => await PlatformWriteItems<T>(items);

        public static async Task<Stream> GetStream(Uri uri)
            => await PlatformGetStream(uri);
    }
}
