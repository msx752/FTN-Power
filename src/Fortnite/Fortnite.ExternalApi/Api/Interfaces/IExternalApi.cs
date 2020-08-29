
using Fortnite.External.Responses.BDailyStore;
using System.Collections.Generic;

namespace Fortnite.External.Api.Interfaces
{
    public interface IExternalApi
    {
        KeyValuePair<string, BrDailyStore> GetBattleRoyaleDailyStore(string lang = "en");
    }
}