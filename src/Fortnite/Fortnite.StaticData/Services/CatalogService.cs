using Fortnite.Api;
using Fortnite.Core.Services;
using Fortnite.Core.Services.Events;
using Fortnite.Model.Enums;
using fortniteLib.Responses.Catalog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite.Static.Services
{

    public class CatalogService : ICatalogService
    {
        private readonly object _lock_catalog = new object();
        private readonly object _lock_IsCatalogReady = new object();
        private Catalog _catalog = new Catalog();
        private bool _isCatalogReady;
        private Thread th;

        public CatalogService(IEpicApi epicApi, Func<ICatalogServiceEventArgs, Task> catalogWebhook = null)
        {
            this.EpicApi = epicApi;
            this.CatalogCallback = catalogWebhook;

        }

        public Catalog Catalog
        {
            get
            {
                lock (_lock_catalog)
                {
                    return _catalog;
                }
            }

            set
            {
                lock (_lock_catalog)
                {
                    _catalog = value;
                }
            }
        }

        private IEpicApi EpicApi { get; set; }

        public bool IsCatalogReady
        {
            get
            {
                lock (_lock_IsCatalogReady)
                {
                    return _isCatalogReady;
                }
            }

            set
            {
                lock (_lock_IsCatalogReady)
                {
                    _isCatalogReady = value;
                }
            }
        }
        public Func<ICatalogServiceEventArgs, Task> CatalogCallback
        {
            get
            {
                return _catalogCallback;
            }
            private set
            {
                _catalogCallback = value;
            }
        }
        private Func<ICatalogServiceEventArgs, Task> _catalogCallback = null;
        public DateTimeOffset UpdateTime { get; private set; }

        public bool LoadCatalogInfo(bool forceToLoad = false)
        {
            if (CatalogCallback == null && !forceToLoad)
            {
                return false;
            }
            return LoadCatalog();
        }

        public void StartCatalogTimer()
        {
            th?.Abort();
            th = new Thread(OnTimedEvent)
            {
                Priority = ThreadPriority.Normal
            };
            if (this.CatalogCallback != null)
            {
                if (LoadCatalog())
                {
                    th.Start();
                    //     MyLogger.Log.Information("{lt}: Catalog Ready", "EpicAPI");
                }
                else
                {
                    // MyLogger.Log.Error("{lt}: Catalog has an issue, please check it", "EpicAPI");
                }
            }
        }

        private TimeSpan GetInterval()
        {
            SetCooldowns();
            var fraction = UpdateTime - DateTimeOffset.UtcNow;
            return fraction;
        }

        private bool LoadCatalog()
        {
            IsCatalogReady = false;
            Catalog catalog = EpicApi.GetCatalog().Value;
            if (catalog != null)
            {

                // MyLogger.Log.Information("{lt}: Catalog is loading", "Service");
                Catalog = catalog;
                SetCooldowns();
                IsCatalogReady = true;
                Global.Log.Information("{lt}: Catalog is successfuly loaded", "Service");
                return true;
            }
            Global.Log.Error("{lt}: Catalog is not loaded", "Service");
            return false;
        }

        private async void OnTimedEvent()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(GetInterval().Add(new TimeSpan(0, 0, 10)));
                    if (LoadCatalog())
                    {
                        if (CatalogCallback != null)
                        {
                            Global.Log.Information("{lt}: Webhook of Catalog is started", "Service");
                            await CatalogCallback.Invoke(new CatalogServiceEventArgs(this.Catalog, CatalogType.CardPackStorePreroll, CatalogType.CardPackStoreGameplay));
                        }
                    }
                    else
                    {
                        throw new Exception("Service: Catalog can not loaded successfuly.");
                    }
                }
                catch (Exception e)
                {
                    //  MyLogger.Log.Exception(e, exceptionNote: $"Service is {this.GetType().Name}");
                }
            }
        }
        //now always once a day
        private void SetCooldowns()
        {
            /*if (this.Catalog.storefronts != null)
            {
                UpdateTime = Catalog.expiration;
            }
            else
            {*/
            UpdateTime = DateTimeOffset.UtcNow.AddDays(1).UtcDateTime.Date;
            //}
        }
    }
}
