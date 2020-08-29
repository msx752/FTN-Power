namespace Fortnite.Static.Models
{
    public class AssetMap
    {
        public string Id { get; internal set; }
        public string EmojiId { get; internal set; }
        public string Url { internal get; set; }
        private string _storedName = null;

        public string StoredName
        {
            get { return _storedName; }
            internal set { _storedName = value; }
        }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}