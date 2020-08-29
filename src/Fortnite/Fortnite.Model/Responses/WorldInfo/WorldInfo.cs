using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    //missionAlerts[0].availableMissionAlerts[0].missionAlertRewards.tierGroupName
    public class WorldInfo
    {
        public WorldInfo()
        {
            theaters = new List<Theater>();
            missions = new List<WorldInfoMission>();
            missionAlerts = new List<MissionAlert>();
        }
        public List<Theater> theaters { get; set; }
        public List<WorldInfoMission> missions { get; set; }
        public List<MissionAlert> missionAlerts { get; set; }
    }
}