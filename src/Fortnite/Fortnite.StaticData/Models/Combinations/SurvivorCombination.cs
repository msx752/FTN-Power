using Fortnite.Model.Enums;
using Newtonsoft.Json;

namespace Fortnite.Static.Models.Combinations
{
    public class SurvivorCombination
    {
        public byte? Level { get; set; }
        public string Name { get; set; }
        public string Personality { get; set; }
        public string SetBonus { get; set; }
        public short SlotId { get; set; } = 0;
        public string SlotType { get; set; }
        public byte? Tier { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return $"{Name}"/*|{Type}|{Tier}|{SlotType}|{SurvivorType}"*/;
        }

        [JsonIgnore]
        public SurvivorRarity? SurvivorType
        {
            get
            {
                return GetSurvivorType(this);
            }
        }

        public static SurvivorRarity? GetSurvivorType(SurvivorCombination survivorCombination)
        {
            var Name = survivorCombination.Name;
            var SlotId = survivorCombination.SlotId;
            var Type = survivorCombination.Type;
            if (SlotId == 0 && Name.Split('-').Length > 2)
            {
                return SurvivorRarity.Mythic;
            }
            else if (SlotId == 0 && Type == "c")
            {
                return SurvivorRarity.UnCommon;
            }
            else if (SlotId == 0 && Type == "r")
            {
                return SurvivorRarity.Epic;
            }
            else if (SlotId == 0 && Type == "vr")
            {
                return SurvivorRarity.Legendary;
            }
            else if (SlotId == 0 && Type == "sr")
            {
                return SurvivorRarity.Mythic;
            }
            else if (Type == "uc")
            {
                return SurvivorRarity.UnCommon;
            }
            else if (Type == "c")
            {
                return SurvivorRarity.Common;
            }
            else if (Type == "r")
            {
                return SurvivorRarity.Rare;
            }
            else if (Type == "vr")
            {
                return SurvivorRarity.Epic;
            }
            else if (Type == "sr")
            {
                return SurvivorRarity.Legendary;
            }
            else if (Type == "ur")
            {
                return SurvivorRarity.Mythic;
            }
            return null;
        }
    }
}