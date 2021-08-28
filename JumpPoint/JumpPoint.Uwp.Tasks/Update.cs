using JumpPoint.Platform.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace JumpPoint.Uwp.Tasks
{
    public sealed class Update : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            await JumpListService.Initialize();

            deferral.Complete();
        }
    }
}
