using Fortnite.Model.Enums;

namespace Fortnite.Core.Interfaces
{
    public interface ISurvivorX
    {
        string AssetId { get; set; }
        bool IsLeader { get; }
        bool IsLeaderAppropriate { get; set; }
        byte Level { get; set; }
        string Name { get; }
        string Personality { get; set; }
        double Power { get; }
        SurvivorRarity Rarity { get; set; }
        string SetBonus { get; set; }
        byte SlotId { get; set; }
        string SquadId { get; set; }
        byte Tier { get; set; }

        string ToString();
    }
}