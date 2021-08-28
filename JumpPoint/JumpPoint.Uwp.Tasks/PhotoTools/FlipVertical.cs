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
    public sealed class FlipVertical : IBackgroundTask
    {
        const int WINCODEC_ERR_COMPONENTNOTFOUND = unchecked((int)0x88982F50);

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
                foreach (var payload in payloads)
                {
                    var result = await PhotoTool.FlipVertical(payload);
                    results.Add(result);
                }

                var data = await ToolHelper.GetData(results);
                await args.Request.SendResponseAsync(data.ToValueSet());

                toastBuilder
                    .AddText("Photo Tool - Flip Vertical")
                    .AddText($"{results.Count(i => i.Result == ToolResult.Successful)}/{results.Count} Picture(s) successfully flipped")
                    .Show();
            }
            catch (Exception ex)
            {
                if (ex.HResult == WINCODEC_ERR_COMPONENTNOTFOUND)
                {
                    toastBuilder
                        .AddText("Photo Tool - Flip Vertical")
                        .AddText("Error: File format's codec or component is not supported")
                        .Show();
                }
                else
                {
                    toastBuilder
                        .AddText("Photo Tool - Flip Vertical")
                        .AddText("There was an error during flipping:")
                        .AddText(ex.Message)
                        .Show();
                }
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
