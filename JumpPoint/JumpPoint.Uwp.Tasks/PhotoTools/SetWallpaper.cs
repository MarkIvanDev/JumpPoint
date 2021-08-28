using JumpPoint.Extensions;
using JumpPoint.Extensions.Tools;
using JumpPoint.Platform.Extensions;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace JumpPoint.Uwp.Tasks.PhotoTools
{
    public sealed class SetWallpaper : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral = null;
        private AppServiceConnection connection = null;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += TaskInstance_Canceled;

            if (taskInstance.TriggerDetails is AppServiceTriggerDetails trigger)
            {
                connection = trigger.AppServiceConnection;
                connection.RequestReceived += Connection_RequestReceived;
                connection.ServiceClosed += Connection_ServiceClosed;
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var serviceDeferral = args.GetDeferral();
            var toastBuilder = new ToastContentBuilder();
            try
            {
                var payloads = await ToolHelper.GetPayloads(args.Request.Message);
                var results = new List<ToolResultPayload>();
                for (int i = 0; i < payloads.Count; i++)
                {
                    if (i == 0)
                    {
                        var result = await PhotoTool.SetWallpaper(payloads[0]);
                        results.Add(result);
                    }
                    else
                    {
                        results.Add(new ToolResultPayload
                        {
                            Result = ToolResult.Failed,
                            Path = payloads[i].Path,
                            Message = "Photo skipped. Only the first photo is set as wallpaper"
                        });
                    }
                }

                var data = await ToolHelper.GetData(results);
                await args.Request.SendResponseAsync(data.ToValueSet());

                if (results.Any(i => i.Result == ToolResult.Successful))
                {
                    toastBuilder
                        .AddText("Photo Tool - Set Wallpaper")
                        .AddText("Wallpaper successfully set")
                        .Show();
                }
                else
                {
                    toastBuilder
                        .AddText("Photo Tool - Set Wallpaper")
                        .AddText("Wallpaper was not set successfully")
                        .Show();
                }
            }
            catch (Exception ex)
            {
                toastBuilder
                    .AddText("Photo Tool - Set Wallpaper")
                    .AddText("There was an error in setting the wallpaper:")
                    .AddText(ex.Message)
                    .Show();
            }
            finally
            {
                serviceDeferral.Complete();
            }
        }

        private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            taskDeferral?.Complete();
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            taskDeferral?.Complete();
        }
    }
}
