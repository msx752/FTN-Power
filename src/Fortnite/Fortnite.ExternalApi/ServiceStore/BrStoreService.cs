using Fortnite.External.Api.Interfaces;
using Fortnite.External.Responses.BDailyStore;
using Fortnite.External.ServiceStore.Events;
using System;
using System.Threading.Tasks;

namespace Fortnite.External.ServiceStore
{
    public class BrStoreService : BaseStoreService<BrDailyStoreEventArgs>
    {
        public IExternalApi Api { get; set; }

        public BrStoreService(IExternalApi exApi) : base()
        {
            Api = exApi;
        }

        public void StartServices(Func<BrDailyStoreEventArgs, Task> brDailyStoreCallback = null)
        {
            StartWebhookTimer(async (now) =>
            {
                var ts = DateTime.UtcNow.Date.AddDays(1) - now;
                return await Task.FromResult(ts.Add(new TimeSpan(0, 4, 0)));
            }, brDailyStoreCallback, async ss =>
             {
                 Global.Log.Information("{lt}: BrDailyStore has been started", "Service");
                 var brStore = Api.GetBattleRoyaleDailyStore();
                 var brImg = await brStore.Value?.GetBrDailyImageAsync();
                 await ss.BaseStoreCallback(new BrDailyStoreEventArgs(brStore.Value?.GetBrDailyTitle(), brStore.Value?.GetBrDailyImageName()));
                 Global.Log.Information("{lt}: BrDailyStore has been ended", "Service");
             });
            Global.Log.Information("{lt}: BrDailyStore is Configured", "Service");
        }
    }
}