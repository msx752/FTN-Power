using Fortnite.External.Api.Interfaces;
using Fortnite.External.Responses.BDailyStore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fortnite.External.Api
{
    public class ExternalApi : BaseExternalApi, IExternalApi
    {
        public ExternalApi()
        {
        }

        public KeyValuePair<string, BrDailyStore> GetBattleRoyaleDailyStore(string lang = "en")
        {
            try
            {
                var result = DoGet(fortniteapiExternal,
                 $"store/get");
                if (result.ErrorMessage != null)
                {
                    Console.WriteLine(result.ErrorMessage);
                    return new KeyValuePair<string, BrDailyStore>(null, null);
                }
                var rslt = GetResponseJsonObject<BrDailyStore>(result);
                return rslt;
            }
            catch (Exception e)
            {
                return new KeyValuePair<string, BrDailyStore>(null, null);
            }
        }

        private KeyValuePair<string, T> GetResponseJsonObject<T>(ExternalResponse result) where T : class
        {
            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                return new KeyValuePair<string, T>($"error[{(int)result.StatusCode}]:{result.ErrorMessage}", null);
            else return new KeyValuePair<string, T>("success", JsonConvert.DeserializeObject<T>(result.Data));
        }
    }
}