using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Extensions.Tools;
using JumpPoint.Platform.Extensions;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Services
{
    public static partial class ToolService
    {
        static Task<IList<Tool>> PlatformGetTools()
            => throw new NotImplementedException();

        static Task<IList<ToolResultPayload>> PlatformRun(Tool tool, IList<JumpPointItem> items)
            => throw new NotImplementedException();
    }
}
