using Fortnite.Api;
using Fortnite.Core.Interfaces;
using Fortnite.Core.ModifiedModels;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.Catalog;
using Fortnite.Static.Models.MissionAlerts;
using Newtonsoft.Json;
using RestSharp;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;

namespace Fortnite.External.Api
{
    public class FortniteQueueApi : IFortniteQueueApi
    {
        public RestClient queueServiceApi = null;

        public FortniteQueueApi(string basicAuthToken , string host , int port )
        {
            queueServiceApi = new RestClient($"{host}:{port}")
            { CookieContainer = new CookieContainer() };
            queueServiceApi.AddOrUpdateDefaultParameter(new Parameter("Authorization", basicAuthToken, ParameterType.HttpHeader));
            queueServiceApi.AddOrUpdateDefaultParameter(new Parameter("Content-Type", "application/json", ParameterType.HttpHeader));
        }

        public IEnumerable<DailyLlama> DailyLlama()
        {
            var rRequest = new RestRequest($"Catalog/DailyLlama", Method.GET, DataFormat.Json);
            return ExecuteApi<List<DailyLlama>>(rRequest);
        }
        public Dictionary<CatalogType, CatalogDataTransferFormat[]> STWStoreSimplified()
        {
            var rRequest = new RestRequest($"Catalog/STWStoreSimplified", Method.GET, DataFormat.Json);
            return ExecuteApi<Dictionary<CatalogType, CatalogDataTransferFormat[]>>(rRequest);
        }

        public IEnumerable<IMissionX> MissionTop10()
        {
            var rRequest = new RestRequest($"Mission/Top10", Method.GET, DataFormat.Json);
            return ExecuteApi<List<MissionX>>(rRequest);
        }

        public IEnumerable<IMissionX> MissionWhere(Expression<Func<IMissionX, bool>> expression)
        {
            var rRequest = new RestRequest($"Mission/Where", Method.POST, DataFormat.Json);
            ExpressionNode queryNode = expression.ToExpressionNode();
            return ExecuteApi<List<MissionX>>(rRequest.AddJsonBody(queryNode));
        }
        public IEnumerable<IMissionX> WebhookMissions()
        {
            var rRequest = new RestRequest($"Mission/WebhookMissions", Method.GET, DataFormat.Json);
            return ExecuteApi<List<MissionX>>(rRequest);
        }
        private T ExecuteApi<T>(IRestRequest restRequest)
        {
            restRequest.JsonSerializer = new NewtonsoftJsonRestSerializer();
            var resp = queueServiceApi.Execute<T>(restRequest);
            if (resp.StatusCode != HttpStatusCode.OK)
                return default(T);
            var data = JsonConvert.DeserializeObject<T>(resp.Content, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore,
            });
            return data;
        }
    }
}