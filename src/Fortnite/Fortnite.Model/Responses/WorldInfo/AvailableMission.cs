using System;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class AvailableMission
    {
        public string missionGuid { get; set; }
        public MissionRewards missionRewards { get; set; }
        public string missionGenerator { get; set; }
        public MissionDifficultyInfo missionDifficultyInfo { get; set; }
        public int tileIndex { get; set; }
        public DateTime availableUntil { get; set; }
        public BonusMissionRewards bonusMissionRewards { get; set; }
    }
}