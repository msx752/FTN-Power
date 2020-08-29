using System;
using System.Collections.Generic;
using System.Text;

namespace Fortnite.Model.Responses.Catalog
{
   public class CatalogDataTransferFormat
    {
        public string PriceType { get; set; }
        public int? Price { get; set; }
        public int? dailyLimit { get; set; }
        public int? weeklyLimit { get; set; }
        public int? monthlyLimit { get; set; }
        public string EventLimit { get; set; }
        public string templateId { get; set; }
        public int? quantity { get; set; }
    }
}
