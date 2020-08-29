namespace Fortnite.Model.Responses.WorldInfo
{
    public class MissionItem
    {
        private string _itemType;
        public int quantity { get; set; }

        public string itemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value;
            }
        }
    }
}