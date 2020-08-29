using System;
using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class WorldInfoMission
    {
        public string theaterId { get; set; }
        public List<AvailableMission> availableMissions { get; set; }
        public DateTime nextRefresh { get; set; }
    }
}