using Fortnite.Core.Services.Events;
using fortniteLib.Responses.Catalog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fortnite.Core.Services
{
    public interface ICatalogService
    {
        Catalog Catalog { get; set; }
        Func<ICatalogServiceEventArgs, Task> CatalogCallback { get; }
        bool IsCatalogReady { get; set; }
        DateTimeOffset UpdateTime { get; }

        bool LoadCatalogInfo(bool forceToLoad = false);
        void StartCatalogTimer();
    }
}
