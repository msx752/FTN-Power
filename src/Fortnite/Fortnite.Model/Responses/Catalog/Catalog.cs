using Fortnite.Model.Enums;
using Fortnite.Model.Responses.Catalog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fortniteLib.Responses.Catalog
{
    public class Catalog
    {

        public int refreshIntervalHrs { get; set; }
        public int dailyPurchaseHrs { get; set; }
        public DateTime expiration { get; set; }
        public List<Storefront> storefronts { get; set; }
        public static List<Storefront> GetStorefrontType(Catalog catalogMapping, Func<Storefront, bool> typeName)
        {
            var p1 = catalogMapping
                .storefronts.Where(x => !string.IsNullOrWhiteSpace(x.name)).Where(typeName).ToList();
            //.FirstOrDefault(p => p.name == typeName.ToString());
            return p1;
        }

        /// <summary>
        /// convert json to jtoken
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JToken ToJToken(string json)
        {
            dynamic data = JsonConvert.DeserializeObject(json);
            JToken jtokenData = null;
            if (json.StartsWith("[") && json.EndsWith("]"))
            {
                JArray AssetArray = JArray.FromObject(data);
                jtokenData = AssetArray.Count() > 0 ? AssetArray[0] : null;
            }
            else if (json.StartsWith("{") && json.EndsWith("}"))
            {
                jtokenData = data;
            }

            return jtokenData != null && !string.IsNullOrEmpty(jtokenData.ToString()) ? jtokenData : null;
        }
        public static Dictionary<CatalogType, CatalogDataTransferFormat[]> GetSTWStoreSimplified(Catalog catalogMapping)
        {
            var sf = GetStorefrontType(catalogMapping, f => f.name == CatalogType.STWSpecialEventStorefront.ToString() || f.name == CatalogType.STWRotationalEventStorefront.ToString()).ToList();
            //var jsn = JsonConvert.SerializeObject(sf,Formatting.Indented);
            Dictionary<CatalogType, CatalogDataTransferFormat[]> pairs = new Dictionary<CatalogType, CatalogDataTransferFormat[]>
            {
                { CatalogType.STWSpecialEventStorefront, new CatalogDataTransferFormat[0] },
                { CatalogType.STWRotationalEventStorefront, new CatalogDataTransferFormat[0] }
            };

            JToken stwspecialEvent = ToJToken(JsonConvert.SerializeObject(sf.First(f => f.name == CatalogType.STWSpecialEventStorefront.ToString())));
            List<CatalogDataTransferFormat> obj = null;
            if (stwspecialEvent != null)
            {
                obj = new List<CatalogDataTransferFormat>();
                foreach (var currentState in stwspecialEvent["catalogEntries"])
                {
                    var o = new CatalogDataTransferFormat
                    {
                        PriceType = currentState["prices"]?.ToArray()?.FirstOrDefault()?["currencySubType"].Value<string>().Split(':')[1],
                        Price = currentState["prices"]?.ToArray()?.FirstOrDefault()?["finalPrice"].Value<int>(),
                        dailyLimit = currentState["dailyLimit"]?.Value<int>(),
                        weeklyLimit = currentState["weeklyLimit"]?.Value<int>(),
                        monthlyLimit = currentState["monthlyLimit"]?.Value<int>(),
                        EventLimit = (currentState["metaInfo"]?.ToArray()?.FirstOrDefault(x => string.Equals(x["key"].Value<string>(), "EventLimit"))?["value"]?.Value<string>()) ?? "",
                        templateId = currentState["itemGrants"]?.ToArray()?.FirstOrDefault()?["templateId"]?.Value<string>().Split(':')[1].Replace("sid_", "id_").Replace("cardpack_", ""),
                        quantity = currentState["itemGrants"]?.ToArray()?.FirstOrDefault()["quantity"]?.Value<int>(),
                    };
                    obj.Add(o);
                }
                pairs[CatalogType.STWSpecialEventStorefront] = obj.ToArray();
            }
            stwspecialEvent = ToJToken(JsonConvert.SerializeObject(sf.First(f => f.name == CatalogType.STWRotationalEventStorefront.ToString())));
            if (stwspecialEvent != null)
            {
                obj = new List<CatalogDataTransferFormat>();
                foreach (var currentState in stwspecialEvent["catalogEntries"])
                {
                    var o = new CatalogDataTransferFormat
                    {
                        PriceType = currentState["prices"]?.ToArray()?.FirstOrDefault()?["currencySubType"].Value<string>().Split(':')[1],
                        Price = currentState["prices"]?.ToArray()?.FirstOrDefault()?["finalPrice"].Value<int>(),
                        dailyLimit = currentState["dailyLimit"]?.Value<int>(),
                        weeklyLimit = currentState["weeklyLimit"]?.Value<int>(),
                        monthlyLimit = currentState["monthlyLimit"]?.Value<int>(),
                        EventLimit = "",
                        templateId = currentState["itemGrants"]?.ToArray()?.FirstOrDefault()?["templateId"]?.Value<string>().Split(':')[1].Replace("sid_", "id_").Replace("cardpack_", ""),
                        quantity = currentState["itemGrants"]?.ToArray()?.FirstOrDefault()["quantity"]?.Value<int>(),
                    };
                    obj.Add(o);
                }
                pairs[CatalogType.STWRotationalEventStorefront] = obj.ToArray();
            }
            obj = null;
            stwspecialEvent = null;
            sf = null;
            return pairs;
        }
    }
}