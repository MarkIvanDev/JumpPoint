using Humanizer;
using JumpPoint.Platform.Models;
using NittyGritty.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace JumpPoint.Platform.Services
{
    public static partial class JumpListService
    {

        private static JumpList jumpList = null;

        static async Task PlatformInitialize()
        {
            if (JumpList.IsSupported())
            {
                jumpList = await JumpList.LoadCurrentAsync();
                jumpList.SystemGroupKind = JumpListSystemGroupKind.None;

                // Initialize Task group
                var tasks = jumpList.Items.Where(i => string.IsNullOrEmpty(i.GroupName)).ToList();
                foreach (var item in tasks)
                {
                    jumpList.Items.Remove(item);
                }
                jumpList.Items.Add(GetItem(ProtocolPath.Dashboard));
                jumpList.Items.Add(GetItem(ProtocolPath.Favorites));
                jumpList.Items.Add(GetItem(ProtocolPath.Drives));
                jumpList.Items.Add(GetItem(ProtocolPath.CloudDrives));
                jumpList.Items.Add(GetItem(ProtocolPath.Workspaces));
                jumpList.Items.Add(GetItem(ProtocolPath.AppLinks));
                jumpList.Items.Add(GetItem(ProtocolPath.Settings));

                await Save();
            }
        }

        static JumpListItem GetItem(ProtocolPath path)
        {
            switch (path)
            {
                case ProtocolPath.Dashboard:
                case ProtocolPath.Settings:
                case ProtocolPath.Favorites:
                case ProtocolPath.Drives:
                case ProtocolPath.CloudDrives:
                case ProtocolPath.Workspaces:
                case ProtocolPath.AppLinks:
                    var text = path.ToString();
                    var item = JumpListItem.CreateWithArguments(
                        new QueryString
                        {
                            { nameof(PathInfo.Type), text },
                            { nameof(PathInfo.Path), text }
                        }.ToString(), path.Humanize());
                    item.Description = text;
                    item.Logo = new Uri($@"ms-appx:///Assets/Icons/Path/{text}.png");
                    return item;

                case ProtocolPath.Open:
                case ProtocolPath.Drive:
                case ProtocolPath.Folder:
                case ProtocolPath.Workspace:
                case ProtocolPath.Cloud:
                case ProtocolPath.Properties:
                case ProtocolPath.Unknown:
                default:
                    return null;
            }
        }

        static async Task Save()
        {
            if (jumpList != null)
            {
                try
                {
                    await jumpList.SaveAsync();
                }
                catch (Exception)
                {

                }
            }
        }

    }
}
