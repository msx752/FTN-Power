using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Serialize.Linq.Nodes;
using Fortnite.Core.Interfaces;
using System.Linq;
using Fortnite.Core.Services;
using FTNPower.Queue.Helpers;
using Fortnite.Core;
using Fortnite.Model.Enums;
using Fortnite.Static.Models.MissionAlerts;

namespace FTNPower.Queue.Controllers
{
    [BasicAuth]
    public class MissionController : Controller
    {
        public readonly IMissionService MissionService;
        public MissionController(IMissionService missionService)
        {
            MissionService = missionService;
        }
        [HttpGet]
        public string Index()
        {
            return "";
        }
        [HttpPost]
        [Produces("application/json")]
        public List<MissionX> Where([FromBody] ExpressionNode query)
        {
            if (!MissionService.IsWorldReady)
                return new List<MissionX>();

            if (query != null)
            {
                var expression = query.ToBooleanExpression<IMissionX>();
                var result = MissionService.MissionsList.AsQueryable().Where(expression.Compile()).Cast<MissionX>().ToList();
                return result;
            }
            else
            {
                return MissionService.MissionsList.AsQueryable().Cast<MissionX>().ToList();
            }
        }
        [HttpGet]
        [Produces("application/json")]
        public List<MissionX> Top10()
        {
            if (!MissionService.IsWorldReady)
                return new List<MissionX>();

            return MissionService.Top10MissionsList.Cast<MissionX>().ToList();
        }
        [HttpGet]
        [Produces("application/json")]
        public List<MissionX> WebhookMissions([FromQuery]bool getNewest = true)
        {
            if (!MissionService.IsWorldReady)
                return new List<MissionX>();

            var newest = MissionService.UpdateTime2;

            if (!getNewest)
                newest = MissionService.UpdateTime;

            List<IMissionX> topList = new List<IMissionX>();
            topList.AddRange(MissionService.MissionsList.Where(p =>
                p.HasMythicSurvivor() ||
                p.HasMythicHero() ||
                p.HasVBuck() ||
                p.HasLegendarySurvivor() ||
                p.HasLegendaryDefender() ||
                p.HasLegendaryHero() ||
                p.HasLegendaryShematic() ||
                p.Has4xEyeOfStorm() ||
                p.Has4xLightningInABottle() ||
                p.Has4xPureDropOfRain() ||
                p.Has4xStormShard() ||
                // p.Has4xSchematicXP() ||
                // p.Has4xHeroXP() ||
                // p.Has4xSurvivorXP() ||
                p.HasLegendaryAnyTransform()));

            if (topList.Count == 0)
                topList.AddRange(MissionService.MissionsList.Where(p => p.HasLegendaryPerkUp(WorldName.Twine_Peaks) || p.HasEpicPerkUp(WorldName.Twine_Peaks)));
            else
                topList.AddRange(MissionService.MissionsList.Where(p => p.HasLegendaryPerkUp(WorldName.Twine_Peaks) || p.HasEpicPerkUp(WorldName.Twine_Peaks)));
            topList = topList.Where(f => (f.HasAnyElementalAlert() == false || (f.HasAnyElementalAlert() == true && f.HasVBuck())) && (f.HasAnyAlert(Alert.StormCategory) == false || (f.HasAnyAlert(Alert.StormCategory) == true && f.HasVBuck() == true)) && f.availableUntil == newest)
                .Distinct().ToList();

            return topList.Cast<MissionX>().ToList();
        }
    }
}
