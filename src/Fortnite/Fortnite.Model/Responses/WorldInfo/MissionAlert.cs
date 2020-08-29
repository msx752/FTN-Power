using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class MissionAlert
    {
        public MissionAlert()
        {
            availableMissionAlerts = new List<AvailableMissionAlert>();
            missionAlertModifiers = new List<object>();

        }
        public string theaterId { get; set; }
        public List<AvailableMissionAlert> availableMissionAlerts { get; set; }
        public DateTime nextRefresh { get; set; }
        public List<object> missionAlertModifiers { get; set; }
    }

}