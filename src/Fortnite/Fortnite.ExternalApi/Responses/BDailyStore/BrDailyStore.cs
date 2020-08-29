using System.Collections.Generic;

namespace Fortnite.External.Responses.BDailyStore
{
    public class BrDailyStore
    {
        public int lastUpdate { get; set; }
        public List<Datum> data { get; set; }
    }
}