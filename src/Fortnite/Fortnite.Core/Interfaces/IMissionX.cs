using Fortnite.Model.Enums;
using System;
using System.Collections.Generic;

namespace Fortnite.Core.Interfaces
{
    public interface IMissionX
    {
        DateTimeOffset availableUntil { get; set; }
        bool IsGroupMission { get; set; }
        bool IsStormShieldDefense { get; set; }
        List<IMissionItemX> Items { get; }
        Alert MissionCategory { get; set; }
        int MissionLevel { get; }
        IMissionNameInfo MissionNameInfo { get; set; }
        List<IAlertModifierItem> Modifiers { get; }
        DateTimeOffset NextRefresh { get; set; }
        int OrderNumber { get; set; }
        int Tier { get; set; }
        WorldName WorldName { get; set; }
    }
}