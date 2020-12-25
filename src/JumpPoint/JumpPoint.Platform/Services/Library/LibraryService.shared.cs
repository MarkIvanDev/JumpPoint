using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Services
{
    public static partial class LibraryService
    {
        public static Task<IList<Library>> GetLibraries()
            => PlatformGetLibraries();
    }
}
