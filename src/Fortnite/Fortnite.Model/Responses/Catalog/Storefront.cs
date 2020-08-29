using System.Collections.Generic;

namespace fortniteLib.Responses.Catalog
{
    public class Storefront
    {
        public string name { get; set; }
        public List<CatalogEntry> catalogEntries { get; set; }
    }
}