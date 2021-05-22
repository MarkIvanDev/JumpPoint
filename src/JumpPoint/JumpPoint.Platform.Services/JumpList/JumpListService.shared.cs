using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JumpPoint.Platform.Services
{
    public static partial class JumpListService
    {

        public static async Task Initialize()
            => await PlatformInitialize();

    }
}
