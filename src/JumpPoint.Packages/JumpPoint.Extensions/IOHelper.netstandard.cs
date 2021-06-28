using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Extensions
{
    public static partial class IOHelper
    {
        static Task<IList<T>> PlatformReadItems<T>(string token)
            => throw new NotImplementedException();

        static Task<string> PlatformWriteItems<T>(IList<T> items)
            => throw new NotImplementedException();

        static Task<Stream> PlatformGetStream(Uri uri)
            => throw new NotImplementedException();
    }
}
