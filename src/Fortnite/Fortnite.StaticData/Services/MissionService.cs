using Fortnite.Api;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Core.Services;
using Fortnite.Core.Services.Events;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.WorldInfo;
using Fortnite.Static.Models.MissionAlerts;
using Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite.Static.Services
{

    public class MissionService : IMissionService
    {
        private readonly object _lock_IsWorldReady = new object();

        private bool _isWorldReady;

        private IEnumerable<IMissionX> _missionsList = new List<IMissionX>();
        private Thread th;

        private IEpicApi EpicApi { get; set; }
        public MissionService(IEpicApi epicApi, Func<IMissionServiceEventArgs, Task> missionCallback)
        {
            this.EpicApi = epicApi;
            this.WebhookCallback = missionCallback;
        }
        public bool IsWorldReady
        {
            get
            {
                lock (_lock_IsWorldReady)
                {
                    return _isWorldReady;
                }
            }

            set
            {
                lock (_lock_IsWorldReady)
                {
                    _isWorldReady = value;
                }
            }
        }

        public IEnumerable<IMissionX> MissionsList
        {
            get
            {
                return _missionsList;
            }

            set
            {
                _missionsList = value;
            }
        }

        public IEnumerable<IMissionX> Top10MissionsList
        {
            get;

            set;
        }

        public DateTimeOffset UpdateTime { get; private set; }
        public DateTimeOffset UpdateTime2 { get; private set; }

        public IEnumerable<IMissionX> GetEpicMaps()
        {
            return MissionsList.Where(p => p.AnyEpic()).Distinct();
        }
        public IEnumerable<IMissionX> Get4xPureDrop(WorldName world = WorldName.Twine_Peaks)
        {
            return MissionsList.Where(p => p.Has4xPureDropOfRain(world)).Distinct();
        }
        public IEnumerable<IMissionX> Get4xStormShard(WorldName world = WorldName.Twine_Peaks)
        {
            return MissionsList.Where(p => p.Has4xStormShard(world)).Distinct();
        }
        public IEnumerable<IMissionX> Get4xEyeOfStorm(WorldName world = WorldName.Twine_Peaks)
        {
            return MissionsList.Where(p => p.Has4xEyeOfStorm(world)).Distinct();
        }
        public IEnumerable<IMissionX> Get4xLightningInABottle(WorldName world = WorldName.Twine_Peaks)
        {
            return MissionsList.Where(p => p.Has4xLightningInABottle(world)).Distinct();
        }
        public IEnumerable<IMissionX> GetEpicPerk(WorldName world)
        {
            return MissionsList.Where(p => p.HasEpicPerkUp(world)).Distinct();
        }

        public IEnumerable<IMissionX> GetLegendaryMaps(WorldName world)
        {
            return MissionsList.Where(p => p.AnyLegendary()).Distinct();
        }
        public IEnumerable<IMissionX> GetMythicMaps(WorldName world)
        {
            return MissionsList.Where(p => p.AnyMythic()).Distinct();
        }
        public IEnumerable<IMissionX> GetLegendaryPerk(WorldName world)
        {
            return MissionsList.Where(p => p.HasLegendaryPerkUp(world)).Distinct();
        }

        public IEnumerable<IMissionX> GetLegendarySurvivors()
        {
            return MissionsList.Where(p => p.HasLegendarySurvivor()).Distinct();
        }

        public AllMissions GetMissions(WorldInfo world, IEnumerable<World> worlds)
        {
            AllMissions allmisions = new AllMissions();
            foreach (var map in worlds)
            {
                allmisions.missions.AddRange(world.missions.Where(p => p.theaterId == map.uniqueId));
                allmisions.missionAlerts.AddRange(world.missionAlerts.Where(p => p.theaterId == map.uniqueId));
            }
            return allmisions;
        }

        public IEnumerable<IMissionX> GetMythicSurvivors()
        {
            return MissionsList.Where(p => p.HasMythicSurvivor()).Distinct();
        }

        public IEnumerable<IMissionX> GetVbuckMaps()
        {
            return MissionsList.Where(p => p.HasVBuck()).Distinct();
        }

        public Task<IEnumerable<IMissionX>> GetWebhookMissions(bool getNewest = true)
        {
            return Task.Run(() =>
            {
                var newest = UpdateTime2;
                if (!getNewest)
                {
                    newest = UpdateTime;
                }

                List<IMissionX> topList = new List<IMissionX>();
                topList.AddRange(MissionsList.Where(p =>
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
                {
                    topList.AddRange(MissionsList.Where(p => p.HasLegendaryPerkUp(WorldName.Twine_Peaks) || p.HasEpicPerkUp(WorldName.Twine_Peaks)));
                }
                else
                {
                    topList.AddRange(MissionsList.Where(p => p.HasLegendaryPerkUp(WorldName.Twine_Peaks) || p.HasEpicPerkUp(WorldName.Twine_Peaks)));
                }
                topList = topList.Where(f => (f.HasAnyElementalAlert() == false || (f.HasAnyElementalAlert() == true && f.HasVBuck())) && (f.HasAnyAlert(Alert.StormCategory) == false || (f.HasAnyAlert(Alert.StormCategory) == true && f.HasVBuck() == true)) && f.availableUntil == newest)
                    .Distinct().ToList();

                return topList.AsEnumerable();
            });
        }

        public IEnumerable<World> GetWorlds(List<Theater> theaters)
        {
            return theaters
                 .Take(4)
                 .Select(p => new World
                 {
                     displayName = p.displayName,
                     tiles = p.tiles,
                     regions = p.regions,
                     uniqueId = p.uniqueId
                 });
        }

        public bool LoadWorldInfo(bool forceToLoad = false)
        {
            if (WebhookCallback == null && !forceToLoad)
            {
                return false;
            }
            return LoadWorld();
        }

        public IEnumerable<IMissionX> MissionMaping(List<World> worlds, AllMissions allMissions)
        {
            List<IMissionX> MissionXsList = new List<IMissionX>();
            try
            {
                for (int i = 0; i < allMissions.missionAlerts.Count; i++)
                {
                    var missionAlertsGroup = allMissions.missionAlerts[i].availableMissionAlerts
                        .Where(c => c.missionAlertModifiers != null)
                        .GroupBy(f => f.categoryName).ToList();
                    var theater = worlds[i];
                    var missionMap = allMissions.missions[i];
                    for (int j = 0; j < missionAlertsGroup.Count; j++)
                    {
                        var category = missionAlertsGroup[j].Key;
                        if (category == "DudebroCategory")
                        {
                            continue;
                        }
                        var mssinsAlert = missionAlertsGroup[j].ToList();
                        for (int k = 0; k < mssinsAlert.Count; k++)
                        {
                            IMissionX mssnx = new MissionX
                            {
                                WorldName = theater.displayName.ParseMapName(),
                                NextRefresh = missionMap.nextRefresh
                            };
                            mssnx.MissionCategory = ((Alert)Enum.Parse(typeof(Alert), category));
                            mssnx.availableUntil = mssinsAlert[k].availableUntil;
                            var currentMap = missionMap.availableMissions.First(p => p.tileIndex == mssinsAlert[k].tileIndex);
                            var mapName = currentMap.missionGenerator.Split('.')[1].Replace("MissionGen_", "");
                            mssnx.IsGroupMission = mapName.IndexOf("_Group", StringComparison.CurrentCultureIgnoreCase) > -1;

                            Regex r = new Regex("Mission_Select_T(\\d+)", RegexOptions.Singleline);
                            var m = r.Match(currentMap.missionRewards.tierGroupName);
                            if (m.Success)//mission level
                            {
                                mssnx.IsStormShieldDefense = false;
                                mssnx.Tier = byte.Parse(m.Groups[1].Value);
                            }
                            else
                            {
                                r = new Regex("Outpost_Select_Theater(\\d+)", RegexOptions.Singleline);
                                m = r.Match(currentMap.missionRewards.tierGroupName);
                                if (m.Success)//stormshield detected no energy
                                {
                                    mssnx.IsStormShieldDefense = true;
                                    mssnx.Tier = 0;
                                }
                                else
                                {
                                    r = new Regex("Mission_Select_Group_T(\\d+)", RegexOptions.Singleline);
                                    m = r.Match(currentMap.missionRewards.tierGroupName);
                                    if (m.Success)//group missions
                                    {
                                        mssnx.Tier = byte.Parse(m.Groups[1].Value);
                                    }
                                    else
                                    {
                                        throw new Exception("i think this is, not defined case");
                                    }
                                }
                            }
                            //debuff//2. emoji serverinden
                            bool hasMiniboss = false;
                            foreach (var item in mssinsAlert[k].missionAlertModifiers.items)
                            {
                                mssnx.Modifiers.Add(new AlertModifierItem() { ItemType = item.itemType });
                                if (item.itemType == "GameplayModifier:minibossenableprimarymissionitem")
                                {
                                    hasMiniboss = true;
                                }
                            }
                            mssnx.MissionNameInfo = Utils.GetMissionName(mapName, hasMiniboss);

                            foreach (var item in currentMap.missionRewards.items)
                            {
                                var ms1x = new MissionItemX
                                {
                                    AlertReward = false,
                                    quantity = item.quantity,
                                    ItemType = item.itemType
                                };
                                mssnx.Items.Add(ms1x);
                            }
                            for (int l = 0; l < mssinsAlert[k].missionAlertRewards.items.Count; l++)
                            {
                                var item = mssinsAlert[k].missionAlertRewards.items[l];
                                if (item.itemType.StartsWith("#"))
                                {
                                    continue;
                                }
                                else
                                {

                                }
                                var ms2x = new MissionItemX
                                {
                                    AlertReward = true,
                                    quantity = item.quantity,
                                    ItemType = item.itemType
                                };
                                mssnx.Items.Add(ms2x);
                            }
                            MissionXsList.Add(mssnx);
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                Global.Log.Exception(e2);
            }
            return MissionXsList.AsEnumerable();
        }
        public Func<IMissionServiceEventArgs, Task> WebhookCallback
        {
            get
            {
                return _webhookCallback;
            }

            private set
            {
                _webhookCallback = value;
            }
        }
        private Func<IMissionServiceEventArgs, Task> _webhookCallback = null;
        public void StartWebhookTimer()
        {
            th?.Abort();
            th = new Thread(OnTimedEvent)
            {
                Priority = ThreadPriority.Normal
            };
            if (this.WebhookCallback != null)
            {
                if (LoadWorld())
                {

                    th.Start();
                    // MyLogger.Log.Information("{lt}: WorldInfo Ready", "EpicAPI");
                }
                else
                {
                    // MyLogger.Log.Error("{lt}: WorldInfo has an issue, please check it", "EpicAPI");
                }
            }

        }

        public string ToJson()
        {
            string jsonMissions = JsonConvert.SerializeObject(MissionsList, Formatting.Indented);
            return jsonMissions;
        }

        public IEnumerable<IMissionX> TopMissions()
        {
            var list = MissionsList.Where(p =>
                      p.HasMythicSurvivor() ||
                      p.HasMythicHero() ||
                      p.HasVBuck() ||
                      p.HasLegendarySurvivor() ||
                      p.HasLegendaryHero() ||
                      p.IsFourStacks("Survivor Xp") ||
                      p.IsGroupMission(WorldName.Twine_Peaks) ||
                      p.IsFourStacks("Legendary Perk-Up") ||
                      p.IsFourStacks("Pure Drop Of Rain") ||
                      p.IsFourStacks("Lightning In A Bottle") ||
                      p.IsFourStacks("Eye Of The Storm") ||
                      p.IsFourStacks("Storm Shard") ||
                      p.IsFourStacks("Epic Perk-Up") ||
                      p.HasLegendaryShematic() ||
                      p.HasLegendaryDefender() ||
                      p.HasLegendaryAnyTransform() ||
                      p.HasEpicSurvivor() ||
                      p.HasEpicHero() ||
                      p.HasEpicAnyTransform() ||
                      p.IsFourStacks("Survivor Xp", WorldName.Canny_Valley) ||
                      p.IsGroupMission(WorldName.Canny_Valley) ||
                      p.IsFourStacks("Legendary Perk-Up", WorldName.Canny_Valley) ||
                      p.IsFourStacks("Pure Drop Of Rain", WorldName.Canny_Valley) ||
                      p.IsFourStacks("Lightning In A Bottle", WorldName.Canny_Valley) ||
                      p.IsFourStacks("Eye Of The Storm", WorldName.Canny_Valley) ||
                      p.IsFourStacks("Storm Shard", WorldName.Canny_Valley) ||
                      p.IsFourStacks("Epic Perk-Up", WorldName.Canny_Valley))
                        .Where(f => (f.HasAnyElementalAlert() == false || (f.HasAnyElementalAlert() == true && f.HasVBuck())))
                        .Distinct()
                        .OrderBy(f => f.OrderNumber);

            return list.Take(24);
        }

        private TimeSpan GetInterval(bool getMin = true)
        {
            if (getMin)
            {
                var fraction = UpdateTime - DateTimeOffset.UtcNow;
                return fraction;
            }
            else
            {
                var fraction = UpdateTime2 - DateTimeOffset.UtcNow;
                return fraction;
            }
        }

        private bool LoadWorld()
        {
            IsWorldReady = false;
            MissionsList = null;
            WorldInfo world = EpicApi.GetWorldInfo().Value;
            if (world != null)
            {
                // MyLogger.Log.Information("{lt}: WorldInfo is loading", "Service");
                List<World> worlds = GetWorlds(world.theaters).ToList();
                AllMissions allmisions = GetMissions(world, worlds);
                MissionsList = MissionMaping(worlds, allmisions);
                Top10MissionsList = TopMissions().Take(10);
                worlds = null;
                allmisions = null;
                SetCooldowns();
                IsWorldReady = true;
                Global.Log.Information("{lt}: WorldInfo is successfuly loaded", "Service");
                return true;
            }
            Global.Log.Error("{lt}: WorldInfo is not loaded", "Service");
            return false;
        }

        private async void OnTimedEvent()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(GetInterval().Add(new TimeSpan(0, 1, 0)));
                    if (LoadWorld())
                    {
                        if (WebhookCallback != null)
                        {
                            //  MyLogger.Log.Information("{lt}: Webhook of Missions is started", "Service");
                            var newMissions = await GetWebhookMissions();
                            await WebhookCallback.Invoke(new MissionServiceEventArgs(newMissions));
                        }
                    }
                    else
                    {
                        throw new Exception("Service: WorldInfo can not loaded successfuly.");
                    }
                }
                catch (Exception e)
                {
                    Global.Log.Exception(e, exceptionNote: $"Service is {this.GetType().Name}");
                }
            }
        }

        private void SetCooldowns()
        {
            var lsTime = MissionsList.Select(p => p.availableUntil).Distinct().OrderBy(p => p).ToList();
            if (lsTime.Count == 1)
            {
                lsTime.Add(lsTime[0]);
            }
            if (lsTime.Count != 2)
            {
                throw new Exception($"Service: expected count:2 now:{lsTime.Count} for missions [check GetInterval();]");
            }
            UpdateTime = lsTime[0];
            UpdateTime2 = lsTime[1];
        }
    }
}
