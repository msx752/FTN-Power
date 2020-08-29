using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class Tile
    {
        public string tileType { get; set; }
        public string zoneTheme { get; set; }
        public Requirements requirements { get; set; }
        public List<LinkedQuest> linkedQuests { get; set; }
        public int xCoordinate { get; set; }
        public int yCoordinate { get; set; }
        public List<MissionWeightOverride> missionWeightOverrides { get; set; }
        public List<MissionWeightOverride> difficultyWeightOverrides { get; set; }
        public bool canBeMissionAlert { get; set; }
        public TileTags tileTags { get; set; }
    }
}