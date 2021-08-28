using JumpPoint.Extensions;
using JumpPoint.Extensions.AppLinkProviders;
using JumpPoint.Platform.Extensions;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace JumpPoint.Uwp.Tasks.AppLinkProviders
{
    public sealed class LocalProvider : IBackgroundTask
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
            }
        }

        private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var serviceDeferral = args.GetDeferral();
            var toastBuilder = new ToastContentBuilder();
            try
            {
                var payloads = LocalAppLinkProvider.GetPayloads();
                var data = await AppLinkProviderHelper.GetData(payloads);
                if (data.ContainsKey(nameof(AppLinkPayload)))
                {
                    await args.Request.SendResponseAsync(data.ToValueSet());

                    toastBuilder
                        .AddText("App Link Provider - Local")
                        .AddText("App links successfully sent")
                        .Show();
                }
                else
                {
                    toastBuilder
                        .AddText("App Link Provider - Local")
                        .AddText("There was a problem sending the app links")
                        .Show();
                }
            }
            catch (Exception ex)
            {
                toastBuilder
                    .AddText("App Link Provider - Local")
                    .AddText("There was an error in fetching app links:")
                    .AddText(ex.Message)
                    .Show();
            }
            finally
            {
                serviceDeferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            taskDeferral?.Complete();
        }
    }
}
