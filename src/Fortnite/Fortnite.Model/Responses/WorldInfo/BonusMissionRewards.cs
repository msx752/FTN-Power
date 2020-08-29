using Fortnite.Model.Responses.QueryProfile;
using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class BonusMissionRewards
    {
        public string tierGroupName { get; set; }
        public List<Item> items { get; set; }
    }
}