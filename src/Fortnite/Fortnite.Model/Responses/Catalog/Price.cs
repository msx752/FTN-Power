using System;

namespace fortniteLib.Responses.Catalog
{
    public class Price
    {
        public string currencyType { get; set; }
        public string currencySubType { get; set; }
        public int regularPrice { get; set; }
        public int finalPrice { get; set; }
        public string saleType { get; set; }
        public DateTime saleExpiration { get; set; }
        public int basePrice { get; set; }
    }
}