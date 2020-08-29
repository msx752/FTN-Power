namespace FTNPower.Image.Api.Service.Models
{
    public class AssetDat
    {
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string HashedId { get; set; }
        public string ClassType { get; set; }
        public string Id { get; set; }
        public string ImageName { get; set; }

        public string Rarity { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}