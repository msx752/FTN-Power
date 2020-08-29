namespace Fortnite.External.Responses.BDailyStore
{
    public class Item
    {
        public string name { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string rarity { get; set; }
        public string costmeticId { get; set; }
        public Images images { get; set; }
        public Backpack backpack { get; set; }
        public string obtained_type { get; set; }
        public Ratings ratings { get; set; }
    }
}