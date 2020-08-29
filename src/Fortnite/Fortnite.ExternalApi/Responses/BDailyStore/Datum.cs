namespace Fortnite.External.Responses.BDailyStore
{
    public class Datum
    {
        public string itemId { get; set; }
        public int lastUpdate { get; set; }
        public Store store { get; set; }
        public Item item { get; set; }
    }
}