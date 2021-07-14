using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using JumpPoint.Extensions.Tools;
using JumpPoint.Platform.Items;

namespace JumpPoint.Platform.Extensions
{
    public static partial class ToolManager
    {
        public static event EventHandler ExtensionCollectionChanged;

        public static async Task<IList<Tool>> GetTools()
            => await PlatformGetTools();

        public static async Task<IList<ToolResultPayload>> Run(Tool tool, IList<JumpPointItem> items)
            => await PlatformRun(tool, items);

        public static ToolPayloadType ToToolPayloadType(this JumpPointItemType itemType)
        {
            switch (itemType)
            {
                case JumpPointItemType.Drive:
                    return ToolPayloadType.Drive;

                case JumpPointItemType.Folder:
                    return ToolPayloadType.Folder;

                case JumpPointItemType.File:
                    return ToolPayloadType.File;

                case JumpPointItemType.Workspace:
                    return ToolPayloadType.Workspace;

                case JumpPointItemType.AppLink:
                    return ToolPayloadType.AppLink;

                case JumpPointItemType.Library:
                    return ToolPayloadType.Library;

                case JumpPointItemType.Unknown:
                default:
                    return ToolPayloadType.Unknown;
            }
        }

    }

}
