using Fortnite.Core.Interfaces;
using Fortnite.Core.ModifiedModels;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.Catalog;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Fortnite.Api
{

    public interface IFortniteQueueApi
    {
        Dictionary<CatalogType, CatalogDataTransferFormat[]> STWStoreSimplified();
        IEnumerable<DailyLlama> DailyLlama();
        IEnumerable<IMissionX> WebhookMissions();
        IEnumerable<IMissionX> MissionWhere(Expression<Func<IMissionX, bool>> expression);
        IEnumerable<IMissionX> MissionTop10();
    }

}
