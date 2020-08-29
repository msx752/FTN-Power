using Fortnite.Model.Responses.WorldInfo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fortnite.Core.Services
{
    public class AllMissions
    {
        public List<WorldInfoMission> missions { get; set; } = new List<WorldInfoMission>();
        public List<MissionAlert> missionAlerts { get; set; } = new List<MissionAlert>();
    }
}
