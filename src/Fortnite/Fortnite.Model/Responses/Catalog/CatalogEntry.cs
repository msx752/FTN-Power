using System.Collections.Generic;

namespace fortniteLib.Responses.Catalog
{
    public class CatalogEntry
    {
        public string offerId { get; set; }
        public string devName { get; set; }
        public string offerType { get; set; }
        public List<Price> prices { get; set; }
        public List<object> categories { get; set; }
        public int dailyLimit { get; set; }
        public int weeklyLimit { get; set; }
        public int monthlyLimit { get; set; }
        public List<object> appStoreId { get; set; }
        public List<object> requirements { get; set; }
        public List<object> metaInfo { get; set; }
        public string catalogGroup { get; set; }
        public int catalogGroupPriority { get; set; }
        public int sortPriority { get; set; }
        public string title { get; set; }
        public string shortDescription { get; set; }
        public string description { get; set; }
        public string displayAssetPath { get; set; }
        public List<object> itemGrants { get; set; }
        public MetaAssetInfo metaAssetInfo { get; set; }
        public List<object> fulfillmentIds { get; set; }
        public string matchFilter { get; set; }
        public double? filterWeight { get; set; }
        public GiftInfo giftInfo { get; set; }
        public bool? refundable { get; set; }
    }
}