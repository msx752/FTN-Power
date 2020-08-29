using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite.External.ServiceStore
{
    public class BaseStoreService<T> where T : EventArgs
    {
        private Thread th;
        private Func<BaseStoreService<T>, Task> timedEventAction = null;

        public BaseStoreService()
        {
        }

        public Func<T, Task> BaseStoreCallback { get; private set; }
        public Func<DateTime, Task<TimeSpan>> CheckPeriod { get; private set; }

        public void StartWebhookTimer(Func<DateTime, Task<TimeSpan>> checkPeriod, Func<T, Task> storeCallback, Func<BaseStoreService<T>, Task> action)
        {
            if (storeCallback != null)
            {
                th?.Abort();
                BaseStoreCallback = storeCallback;
                CheckPeriod = checkPeriod;
                timedEventAction = action;
                th = new Thread(OnTimedEvent);
                th.Priority = ThreadPriority.Normal;
#if RELEASE
                th.Start();
#endif
            }
        }

        private async void OnTimedEvent()
        {
            while (true)
            {
                try
                {
                    var ts = await CheckPeriod(DateTime.UtcNow);
                    Global.Log.Information("{lt}: {ServiceName} refresh period is {ServiceRefreshTime}", "Service", typeof(T).Name.Replace("EventArgs", ""), ts.ToString());
                    await Task.Delay(ts.Add(new TimeSpan(0, 1, 0)));
                    if (BaseStoreCallback != null)
                    {
                        await timedEventAction(this);
                    }
                }
                catch (Exception e)
                {
                    Global.Log.Error($"{{lt}}: {typeof(T).Name.Replace("EventArgs", "")}; {e}", "Service");
                }
            }
        }
    }
}