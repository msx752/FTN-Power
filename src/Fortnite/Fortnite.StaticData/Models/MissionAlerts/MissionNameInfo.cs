using Fortnite.Core.Interfaces;

namespace Fortnite.Static.Models.MissionAlerts
{
    public class MissionNameInfo : IMissionNameInfo
    {
        public MissionNameInfo()
        {

        }
        public MissionNameInfo(string emojiId, string name)
        {
            EmojiId = emojiId;
            Name = name;
        }

        public string Name { get; set; }
        public string EmojiId { get; set; }
    }
}