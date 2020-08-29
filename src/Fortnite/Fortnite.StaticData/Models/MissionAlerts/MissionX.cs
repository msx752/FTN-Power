using Fortnite.Api;
using Fortnite.Core.Interfaces;
using Fortnite.Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fortnite.Static.Models.MissionAlerts
{
    public class MissionX : IMissionX
    {
        public int OrderNumber { get; set; } = 99999;
        public bool IsStormShieldDefense { get; set; }
        public WorldName WorldName { get; set; }
        public DateTimeOffset NextRefresh { get; set; }
        public Alert MissionCategory { get; set; }
        public int Tier { get; set; } = -9;

        public int MissionLevel
        {
            get
            {
                return Core.Utils.GuesMissionLevel(Tier);
            }
        }
        public MissionX()
        {
            Items = new List<IMissionItemX>();
            Modifiers = new List<IAlertModifierItem>();
        }
        public bool IsGroupMission { get; set; }
        public DateTimeOffset availableUntil { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<IMissionNameInfo, MissionNameInfo>))]
        public IMissionNameInfo MissionNameInfo { get; set; }
        [JsonConverter(typeof(ConcreteListTypeConverter<IMissionItemX, MissionItemX>))]
        public List<IMissionItemX> Items { get; set; } = new List<IMissionItemX>();
        [JsonConverter(typeof(ConcreteListTypeConverter<IAlertModifierItem, AlertModifierItem>))]
        public List<IAlertModifierItem> Modifiers { get; set; }
    }
}