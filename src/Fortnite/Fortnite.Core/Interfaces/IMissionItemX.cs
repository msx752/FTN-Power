using Fortnite.Model.Enums;
using System.Collections.Generic;

namespace Fortnite.Core.Interfaces
{
    public interface IMissionItemX
    {
        bool AlertReward { get; set; }
        string AssetId { get; set; }
        AssetType AssetType { get; set; }
        ItemClass Class { get; set; }
        string ItemType { get; set; }
        int quantity { get; set; }
        ItemRarity Rarity { get; set; }
        KeyValuePair<string, string> RealName { get; }
        KeyValuePair<string, string> GetRealItemName(bool shortName = false, string lang = "EN", bool getEmoji = true);
    }
}