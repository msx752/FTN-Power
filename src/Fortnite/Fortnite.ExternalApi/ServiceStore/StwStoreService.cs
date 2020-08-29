using Fortnite.Api;
using Fortnite.External;
using Fortnite.External.ServiceStore.Events;
using fortniteLib.Responses.Catalog;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fortnite.External.ServiceStore
{
    public class StwStoreService : BaseStoreService<StwStoreEventArgs>
    {
        public IEpicApi Api { get; set; }

        public StwStoreService(IEpicApi epicApi) : base()
        {
            Api = epicApi;
        }

        public void StartServices(Func<StwStoreEventArgs, Task> stwStoreCallback = null)
        {
            StartWebhookTimer(async (now) =>
            {
                DateTime tomorrow = DateTime.UtcNow.Date.AddDays(1);
                int daysUntilTuesday = ((int)DayOfWeek.Thursday - (int)tomorrow.DayOfWeek + 7) % 7;
                DateTime nextTuesday = tomorrow.AddDays(daysUntilTuesday);
                var ts = nextTuesday - now;
                return await Task.FromResult(ts.Add(new TimeSpan(0, 6, 0)));
            }, stwStoreCallback, async (ss) =>
            {
                Global.Log.Information("{lt}: StwStore has been started", "Service");

                var filePath = await Api.SaveStwStoreToLocalAsync(true);
                var title = DateTimeOffset.UtcNow.SetStwStoreTitle();
                await ss.BaseStoreCallback(new StwStoreEventArgs(title, filePath));
                Global.Log.Information("{lt}: StwStore has been ended", "Service");
            });
            Global.Log.Information("{lt}: StwStore is Configured", "Service");
        }
    }
}