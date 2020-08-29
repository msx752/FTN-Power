using System;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class AvailableMissionAlert
    {
        public string name { get; set; }
        public string categoryName { get; set; }
        public string spreadDataName { get; set; }
        public string missionAlertGuid { get; set; }
        public int tileIndex { get; set; }
        public DateTime availableUntil { get; set; }
        public int totalSpreadRefreshes { get; set; }
        public MissionRewards missionAlertRewards { get; set; }
        public MissionAlertModifiers missionAlertModifiers { get; set; }
    }
}