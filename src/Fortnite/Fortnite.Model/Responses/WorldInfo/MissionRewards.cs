using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class MissionRewards
    {
        public string tierGroupName { get; set; }
        public List<MissionItem> items { get; set; }
    }

    public class MissionAlertModifiers
    {
        public string tierGroupName { get; set; }
        public List<MissionItem> items { get; set; }
    }
}