using Fortnite.Core.ResponseModels;
using Fortnite.Core.Services;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Model.Responses.WorldInfo;
using fortniteLib.Responses.Catalog;
using fortniteLib.Responses.Pvp;
using Global;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite.Api
{
    public class EpicApi : BaseApi, IEpicApi
    {
        public EpicApi() : base()
        {

        }

        public KeyValuePair<string, Catalog> GetCatalog()
        {
            var rRequest = new RestRequest("fortnite/api/storefront/v2/catalog", Method.GET, DataFormat.Json)
                .AddQueryParameter("rvn", "-1");

            var result = RequestExecute(fortniteapiClient, rRequest);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new KeyValuePair<string, Catalog>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
            }

            return new KeyValuePair<string, Catalog>(result.StatusCode.ToString(), CastContent<Catalog>(result));
        }

        public Task<KeyValuePair<string, IQueryProfile>> GetPVEProfileById(string epicId)
        {
            return Task.Run(() =>
            {
                var daa = GetUserNameById(epicId);
                if (daa.Value == null)
                {
                    return new KeyValuePair<string, IQueryProfile>(daa.Key, null);
                }

                var rslt = GetEpicProfile(epicId);
                return new KeyValuePair<string, IQueryProfile>(daa.Value.displayName, rslt.Value);
            });
        }
        public Task<KeyValuePair<string, IQueryProfile>> GetPVEProfileByName(string userName)
        {
            return Task.Run(() =>
            {
                var daa = GetUserIdByName(userName);
                if (daa.Value == null)
                {
                    return new KeyValuePair<string, IQueryProfile>(daa.Key, null);
                }

                var rslt = GetEpicProfile(daa.Value.id);
                return new KeyValuePair<string, IQueryProfile>(daa.Value.displayName, rslt.Value);
            });
        }

        public Task<KeyValuePair<string, BattleRoyaleStats>> GetPVPProfileById(string epicId)
        {
            return Task.Run(() =>
            {
                var daa = GetUserNameById(epicId);
                if (daa.Value == null)
                {
                    return new KeyValuePair<string, BattleRoyaleStats>(daa.Key, null);
                }

                var rslt = GetEpicPVPProfile_V2(epicId, DateTime.UtcNow);
                return new KeyValuePair<string, BattleRoyaleStats>(daa.Value.displayName, rslt.Value);
            });
        }
        public Task<KeyValuePair<string, BattleRoyaleStats>> GetPVPProfileByName(string userName)
        {
            return Task.Run(() =>
            {
                var daa = GetUserIdByName(userName);
                if (daa.Value == null)
                {
                    return new KeyValuePair<string, BattleRoyaleStats>(daa.Key, null);
                }

                var rslt = GetEpicPVPProfile_V2(daa.Value.id, DateTime.UtcNow);
                return new KeyValuePair<string, BattleRoyaleStats>(daa.Value.displayName, rslt.Value);
            });
        }

        public KeyValuePair<string, PveCurrentGameInfo> GetUserGameInfo(string epicId)
        {

            var rRequest = new RestRequest($"fortnite/api/matchmaking/session/findPlayer/{epicId}", Method.GET, DataFormat.Json);

            var result = RequestExecute(fortniteapiClient, rRequest);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new KeyValuePair<string, PveCurrentGameInfo>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
            }

            var lstt = CastContent<List<PveCurrentGameInfo>>(result);
            return new KeyValuePair<string, PveCurrentGameInfo>(result.StatusCode.ToString(), lstt?.FirstOrDefault());

        }
        public KeyValuePair<string, Lookup> GetUserIdByName(string userName)
        {
            var rRequest = new RestRequest($"account/api/public/account/displayName/{userName}", Method.GET, DataFormat.Json);

            var result = RequestExecute(accountClient, rRequest);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new KeyValuePair<string, Lookup>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
            }

            return new KeyValuePair<string, Lookup>(result.ResponseStatus.ToString(), CastContent<Lookup>(result));
        }

        //http://localhost:5000/account/api/public/account?accountId=fdd5fcfff9d64230a2a33e1782743415
        public KeyValuePair<string, Lookup> GetUserNameById(string epicId)
        {
            var result = GetUserNameByIds(epicId);
            if (result.Value == null)
            {
                return new KeyValuePair<string, Lookup>(result.Key, null);
            }

            return new KeyValuePair<string, Lookup>(result.Key, result.Value?.FirstOrDefault());
        }
        public KeyValuePair<string, List<Lookup>> GetUserNameByIds(params string[] epicIds)
        {
            var rRequest = new RestRequest($"account/api/public/account", Method.GET, DataFormat.Json);
            foreach (var item in epicIds)
            {
                rRequest.AddQueryParameter("accountId", item);
            }

            var result = RequestExecute(oauthClient, rRequest);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new KeyValuePair<string, List<Lookup>>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
            }

            return new KeyValuePair<string, List<Lookup>>(result.StatusCode.ToString(), CastContent<List<Lookup>>(result));
        }

        public string GetVersion()
        {
            var rRequest = new RestRequest($"fortnite/api/game/v2/enabled_features", Method.GET, DataFormat.Json);
            var result = RequestExecute(fortniteapiClient, rRequest);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            return result.Headers.FirstOrDefault(f => f.Name.Equals("X-EpicGames-McpVersion", StringComparison.InvariantCultureIgnoreCase))?.Value?.ToString();
        }

        public KeyValuePair<string, WorldInfo> GetWorldInfo()
        {
            var rRequest = new RestRequest($"fortnite/api/game/v2/world/info", Method.GET, DataFormat.Json);
            var result = RequestExecute(fortniteapiClient, rRequest);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new KeyValuePair<string, WorldInfo>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
            }

            return new KeyValuePair<string, WorldInfo>(result.ResponseStatus.ToString(), CastContent<WorldInfo>(result));
        }
        private KeyValuePair<string, IQueryProfile> GetEpicProfile(string epicId)
        {
            var rRequest = new RestRequest($"fortnite/api/game/v2/profile/{epicId}/public/QueryPublicProfile", Method.POST, DataFormat.Json);
            rRequest.AddQueryParameter("profileId", "campaign");
            rRequest.AddQueryParameter("rvn", "-1");
            rRequest.AddJsonBody(new { });
            var result = RequestExecute(fortniteapiClient, rRequest);

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return new KeyValuePair<string, IQueryProfile>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
            }

            return new KeyValuePair<string, IQueryProfile>(result.ResponseStatus.ToString(), (IQueryProfile)CastContent<QueryProfile>(result));
        }

        private KeyValuePair<string, BattleRoyaleStats> GetEpicPVPProfile_V2(string epicId, DateTime end_time)
        {
            var rRequest = new RestRequest($"statsproxy/api/statsv2/account/{epicId}", Method.GET, DataFormat.Json);
            var result = RequestExecute(statproxyClient, rRequest);

            if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new KeyValuePair<string, BattleRoyaleStats>($"{result.StatusCode}", new BattleRoyaleStats() { accountId = epicId, IsPrivate = true });
            }

            return new KeyValuePair<string, BattleRoyaleStats>(result.ResponseStatus.ToString(), CastContent<BattleRoyaleStats>(result));
        }
        public Task StartVerifier()
        {
            return Task.Run(() =>
            {
                base.SetIdentity(DIManager.Services.EpicApiConfigs().UserName, DIManager.Services.EpicApiConfigs().Password);
          
                Global.Log.Information("{lt}: Static Files Ready", "EpicAPI");
                int c = 0;
                while (true)
                {
                    if (c > 10)
                    {
                        //  MyLogger.Log.Fatal("{lt}: AUTH LOGIN SIGNIFICANT ISSUE.", "EpicAPI");
                        return Task.FromException(new Exception("Epic Api can not be initiated, please check it"));
                    }
                    try
                    {
                        c++;

                        Task.Delay(4000);
                        var token = FTNToken();
                        if (token != null)
                        {
                          //  MyLogger.Log.Information("{lt}: Successfully LoggedIn..", "EpicAPI");
                            this.Authorization = token;
                            this.ReceivedBearerToken = true;
                            //   MyLogger.Log.Information("{lt}: Ready", "EpicAPI");

                            base.StartFortniteTokenVerificator();
                            break;
                        }
                        else
                        {
                          //  MyLogger.Log.Fatal("{lt}: AUTH LOGIN CRITICAL ERROR.", "EpicAPI");
                        }
                    }
                    catch (Exception e1)
                    {
                        continue;
                        //    throw e;
                    }
                }
                return Task.CompletedTask;
            });
        }

    }
}