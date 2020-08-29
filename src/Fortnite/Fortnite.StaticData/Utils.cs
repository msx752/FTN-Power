using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Static;
using Fortnite.Static.Models.MissionAlerts;
using Fortnite.Static.Models.Survivors;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Fortnite.Static
{
    public static partial class Utils
    {
        public static string GetImage(string assetId, bool getEmoji = true)
        {
            if (SurvivorStaticData.AssetBot2.ContainsKey(assetId))
            {
                var storedAlertModifier = SurvivorStaticData.AssetBot2[assetId];
                if (getEmoji)
                {
                    return storedAlertModifier.EmojiId;
                }
                else
                {
                    return storedAlertModifier.Url;
                }
            }
            else
            {
                return "524246128925999114";
            }
        }

        public static Task<IEnumerable<IGrouping<string, ISurvivorX>>> GetSurvivors(this IQueryProfile qp)
        {
            return Task.Run(() =>
            {
                var dt = qp.GetSurvivorList()
                    .Where(p => p.Value.attributes.squad_id != "");

                List<ISurvivorX> survivors = new List<ISurvivorX>();
                foreach (var survivor in dt)
                {
                    ISurvivorX newSv = new SurvivorX
                    {
                        Name = Model.Responses.QueryProfile.Extension.GetItemName(survivor.Value.templateId).Result,
                        Tier = survivor.Value.templateId.GetItemTier(),
                        Level = (byte)survivor.Value.attributes.level,
                        SlotId = (byte)survivor.Value.attributes.squad_slot_idx
                    };
                    newSv.Rarity = Model.Responses.QueryProfile.Extension.GetSurvivorRarity(newSv.SlotId, newSv.Name,
                        survivor.Value.templateId.GetItemType());
                    newSv.SquadId = survivor.Value.attributes.squad_id;

                    newSv.IsLeaderAppropriate = Core.Utils.IsLeaderAppropriate(newSv.SquadId, survivor.Value.templateId);
                    newSv.Personality = survivor.Value.attributes.personality;
                    newSv.SetBonus = survivor.Value.attributes.set_bonus;
                    survivors.Add(newSv);
                }
                return survivors.GroupBy(f => f.SquadId);
            });
        }

        public static Task<IEnumerable<ISurvivorSquad>> GetSurvivorSquads(this IEnumerable<IGrouping<string, ISurvivorX>> gqp)
        {
            return Task.Run(() =>
            {
                List<ISurvivorSquad> ssq = new List<ISurvivorSquad>();
                foreach (var svvrGrp in gqp)
                {
                    ssq.Add(LoadSquad(svvrGrp.Key, svvrGrp));
                }
                return ssq.AsEnumerable();
            });
        }

        public static ISurvivorSquad LoadSquad(string svvrgrpKey, IEnumerable<ISurvivorX> svvrs)
        {
            ISurvivorSquad survivorSquad = new SurvivorSquad
            {
                SquadName = svvrgrpKey,
                Survivors = svvrs,
                TeamBonuses = new List<double>(),
                SetBonuses = new SetBonusCalculator()
            };
            foreach (var i in survivorSquad.Survivors)
            {
                double bonus = survivorSquad.TeamBonus(i);
                survivorSquad.TeamBonuses.Add(bonus);
                survivorSquad.SetBonuses.Increase(i.SetBonus);
            }
            return survivorSquad;
        }

        public static void DefineAsset(this IMissionItemX mix)
        {
            mix.Class = Core.Utils.GetItemClass(mix.ItemType);
            var RegexType = mix.Class.ToString();
            switch (mix.Class)
            {
                case ItemClass.Schematic:
                    var m0 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m0.Success)
                    {
                        var nName = $"{m0.Groups[2].Value}_{{0}}_ore";
                        mix.Rarity = Core.Utils.GetItemRarityType(m0.Groups[3].Value);
                        mix.AssetId = nName;
                    }
                    else
                    {
                        m0 = MissionsStaticData.ItemRegexTable[$"{RegexType}2"].Match(mix.ItemType);
                        if (m0.Success)
                        {
                            var nName = $"{m0.Groups[2].Value}_{{0}}";
                            mix.Rarity = Core.Utils.GetItemRarityType(m0.Groups[3].Value);
                            mix.AssetId = nName;
                        }
                        else
                        {
                            goto default;
                        }
                    }
                    break;

                case ItemClass.CardPack:

                    var m1 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m1.Success)
                    {
                        if (m1.Groups.Count == 7)
                        {
                            if (m1.Groups[6].Value != "")
                            {
                                mix.AssetId = $"{m1.Groups[2].Value}_{m1.Groups[6].Value}";
                            }
                            else
                            {
                                mix.AssetId = $"{m1.Groups[2].Value}";
                            }
                        }
                        else
                        {
                            mix.AssetId = m1.Groups[2].Value;
                        }
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                case ItemClass.AccountResource:
                    var m2 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m2.Success)
                    {
                        mix.AssetId = m2.Groups[2].Value;
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                case ItemClass.ConversionControl:
                    var m3 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m3.Success)
                    {
                        var nName = $"{m3.Groups[2].Value}_consumable_{{0}}";
                        mix.Rarity = Core.Utils.GetItemRarityType(m3.Groups[3].Value);
                        mix.AssetId = nName;
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                case ItemClass.Worker:
                    var m4 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (!m4.Success)
                    {
                        m4 = MissionsStaticData.ItemRegexTable[$"{RegexType}2"].Match(mix.ItemType);
                    }
                    if (m4.Success)
                    {
                        var nName = $"{m4.Groups[2].Value}_{{0}}";
                        if (nName.StartsWith("manager"))
                        {
                            if (m4.Groups[3].Value == "c")
                            {
                                mix.Rarity = Core.Utils.GetItemRarityType("uc");
                            }
                            else if (m4.Groups[3].Value == "uc")
                            {
                                mix.Rarity = Core.Utils.GetItemRarityType("r");
                            }
                            else if (m4.Groups[3].Value == "r")
                            {
                                mix.Rarity = Core.Utils.GetItemRarityType("vr");
                            }
                            else if (m4.Groups[3].Value == "vr")
                            {
                                mix.Rarity = Core.Utils.GetItemRarityType("sr");
                            }
                            else if (m4.Groups[3].Value == "sr")
                            {
                                mix.Rarity = Core.Utils.GetItemRarityType("uv");
                            }
                            else
                            {
                                throw new Exception($"undefined item rarity for:{mix.ItemType}({nName}) define in case ItemClass.Worker:");
                            }
                        }
                        else
                        {
                            mix.Rarity = Core.Utils.GetItemRarityType(m4.Groups[3].Value);
                        }
                        mix.AssetId = nName;
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                case ItemClass.Defender:
                    var m5 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m5.Success)
                    {
                        var nName = $"{m5.Groups[2].Value}_{{0}}";
                        mix.Rarity = Core.Utils.GetItemRarityType(m5.Groups[3].Value);
                        mix.AssetId = nName;
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                case ItemClass.Hero:
                    var m6 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m6.Success)
                    {
                        var nName = $"{m6.Groups[2].Value}_{{0}}";
                        mix.Rarity = Core.Utils.GetItemRarityType(m6.Groups[3].Value);
                        mix.AssetId = nName;
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                case ItemClass.Currency:
                    var m7 = MissionsStaticData.ItemRegexTable[RegexType].Match(mix.ItemType);
                    if (m7.Success)
                    {
                        var nName = m7.Groups[2].Value;
                        mix.AssetId = nName;
                    }
                    else
                    {
                        goto default;
                    }

                    break;

                default:
                    Console.WriteLine($"E#Undefined regex format is detected (for '{mix.ItemType}' and define in 'ItemRegexTable')##");
                    break;
            }
        }

        public static void DefineAsset(this IAlertModifierItem aModifier)
        {
            aModifier.Class = Core.Utils.GetItemClass(aModifier.ItemType);
            string key = aModifier.Class.ToString();
            switch (aModifier.Class)
            {
                case ItemClass.GameplayModifier:
                    var m0 = MissionsStaticData.ItemRegexTable[key].Match(aModifier.ItemType);
                    if (m0.Success)
                    {
                        aModifier.IsPositiveMutation = false;
                        var typename = m0.Groups[2].Value;
                        if (typename.IndexOf("_") == -1)
                        {
                            aModifier.AssetId = typename;
                        }
                        else
                        {
                            aModifier.AssetId = m0.Groups[0].Value.Replace(m0.Groups[1].Value, "");
                        }
                    }
                    else
                    {
                        aModifier.IsPositiveMutation = true;
                        try
                        {
                            var m1 = MissionsStaticData.ItemRegexTable[$"{key}2"].Match(aModifier.ItemType);
                            if (m1.Success)
                            {
                                aModifier.AssetId = m1.Groups[0].Value.Replace(m1.Groups[1].Value, "");
                            }
                            else
                            {
                                throw new Exception($"Undefined1 mapping is detected (for '{aModifier.ItemType}' and define in 'ResourceTable')");
                            }
                        }
                        catch (Exception e)

                        {
                            Console.WriteLine($"Undefined2 mapping is detected (for '{aModifier.ItemType}' and define in 'ResourceTable')");
                        }
                    }
                    break;
            }
        }

        public static IMissionNameInfo GetMissionName(string mapName, bool hasMiniboss)
        {
            Regex r = new Regex("(\\w\\d)_(\\w+)_C", RegexOptions.Singleline);
            var mc = r.Match(mapName);
            if (mc.Success)
            {
                var n1 = mc.Groups[2].Value;
                var nresult = "";
                if (n1.Split('_').Length == 1)
                {
                    nresult = n1;
                }
                else
                {
                    nresult = n1.Split('_')[1];
                }
                return MatchRealMapName(nresult, hasMiniboss);
            }
            else
            {
                r = new Regex("(\\w+)_Group_C", RegexOptions.Singleline);
                mc = r.Match(mapName);
                if (mc.Success)
                {
                    return MatchRealMapName(mc.Groups[1].Value, hasMiniboss);
                }
                else
                {
                    r = new Regex("(\\w+)_Group_(\\w+)_C", RegexOptions.Singleline);
                    mc = r.Match(mapName);
                    if (mc.Success)
                    {
                        return MatchRealMapName(mc.Groups[1].Value, hasMiniboss);
                    }
                    else
                    {
                        r = new Regex("(\\w+)_C", RegexOptions.Singleline);
                        mc = r.Match(mapName);
                        if (mc.Success)
                        {
                            return MatchRealMapName(mc.Groups[1].Value, hasMiniboss);
                        }
                        else
                        {
                            throw new Exception("undefined regex for :" + mapName);
                        }
                    }
                }
            }
        }

        private static IMissionNameInfo MatchRealMapName(string dbName, bool hasMiniboss)
        {
            IMissionNameInfo nameInfo = null;
            switch (dbName)
            {
                case "EtSurvivors":
                case "EvacuateTheSurvivors":
                    nameInfo = new MissionNameInfo("581265620952285194", "Rescue the Survivors");
                    break;

                case "RetrieveTheData":
                case "RtD":
                    nameInfo = new MissionNameInfo("581265842805932071", "Retrieve the Data");
                    break;

                case "EtShelter":
                    nameInfo = new MissionNameInfo("581265621027651584", "Evacuate the Shelter");
                    break;

                case "DestroyTheEncampments":
                case "DtE":
                    nameInfo = new MissionNameInfo("581265621031977000", "Destroy the Encampments");
                    break;

                case "Cat1FtS":
                case "1Gate":
                    nameInfo = new MissionNameInfo("581265620578992143", "Fight the Storm");
                    break;

                case "3Gates":
                    nameInfo = new MissionNameInfo("581265620771799266", "Category 3 Storm");
                    break;

                case "2Gates":
                    nameInfo = new MissionNameInfo("581265620709015574", "Category 2 Storm");
                    break;

                case "4Gates":
                    nameInfo = new MissionNameInfo("581265621015330816", "Category 4 Storm");
                    break;

                case "LtB":
                    if (!hasMiniboss)
                    {
                        nameInfo = new MissionNameInfo("524064527965487104", "Launch the Rocket");
                    }
                    else
                    {
                        nameInfo = new MissionNameInfo("581265842679971840", "Ride the Lightning");
                    }
                    break;

                case "RideTheLightning":
                case "RtL":
                    nameInfo = new MissionNameInfo("581265842679971840", "Ride the Lightning");
                    break;

                case "DtB":
                    nameInfo = new MissionNameInfo("581265620922793984", "Deliver the Bomb");
                    break;

                case "RtS":
                    nameInfo = new MissionNameInfo("581265620826587148", "Repair the Shelter");
                    break;

                case "BuildtheRadarGrid":
                    nameInfo = new MissionNameInfo("581265620591443979", "Build the Radar Grid");
                    break;

                case "DUDEBRO":
                    nameInfo = new MissionNameInfo("524246128925999114", "DUDEBRO?");
                    break;

                case "RefuelTheBase":
                    nameInfo = new MissionNameInfo("581265620927119366", "Refuel the Homebase");
                    break;

                default:
                    nameInfo = new MissionNameInfo("524246128925999114", "UNKNOWN_" + dbName);
                    break;
            }

            return nameInfo;
        }
        public static int BR_Placetop1(this JObject PlayerStats, MatchType matchType, Platform platform = Platform.all, bool rankedStats = false)
        {
            int total = (int)PlayerStats.BR_Stats(StatType.placetop1, matchType, platform, rankedStats);
            return total;
        }
        public static double BR_Stats(this JObject PlayerStats, StatType statType, MatchType matchType, Platform platform, bool rankedStats = false)
        {
            var profileType = PlayerStats.Properties().ToList();
            var properties = PlayerStats.Properties().ToList();
            if (rankedStats)
            {
                if (platform != Platform.all)
                {
                    properties = properties.Where(f => f.Name.StartsWith($"br_{statType}_") && f.Name.Contains($"_{platform}_") && (f.Name.Contains($"_default{matchType.ToString()}") || f.Name.Contains($"_deimos_{matchType.ToString()}"))).ToList();
                }
                else
                {
                    properties = properties.Where(f => f.Name.StartsWith($"br_{statType}_") && (f.Name.Contains($"_{Platform.gamepad}_") || f.Name.Contains($"_{Platform.keyboardmouse}_")) && (f.Name.Contains($"_default{matchType.ToString()}") || f.Name.Contains($"_deimos_{matchType.ToString()}"))).ToList();
                }
            }
            else
            {
                if (platform != Platform.all)
                {
                    properties = properties.Where(f => f.Name.StartsWith($"br_{statType}_") && f.Name.Contains($"_{platform}_") && !(f.Name.Contains($"_default{matchType.ToString()}") || f.Name.Contains($"_deimos_{matchType.ToString()}"))).ToList();
                }
                else
                {
                    properties = properties.Where(f => f.Name.StartsWith($"br_{statType}_") && (f.Name.Contains($"_{Platform.gamepad}_") || f.Name.Contains($"_{Platform.keyboardmouse}_")) && !(f.Name.Contains($"_default{matchType.ToString()}") || f.Name.Contains($"_deimos_{matchType.ToString()}"))).ToList();
                }
            }
            var total = 0;
            foreach (var property in properties)
            {
                var val = property.Value.Value<int>();
                total += val;
            }
            return total;
        }
        public static int AmountOfMythicSchematics(this IQueryProfile qp)
        {
            var mythic_schematics = qp.profileChanges.First().profile.items
               .Where(p => p.Value.templateId.IsMythicSchematic())
               .Select(f => f.Value.templateId)
               .Count();
            return mythic_schematics;
        }
        private static bool IsMythicSchematic(this string templateId)
        {
            Regex r = new Regex("Schematic:sid_(.*)_stormking_sr_(.*)", RegexOptions.Singleline);
            var m = r.Match(templateId);
            var b = m.Success;
            return b;
        }
        public static bool DoneEliteFrostnite2019(this IQueryProfile qp)
        {
            var elite_frostnite_mission = qp.profileChanges.First().profile.items
               .FirstOrDefault(p => p.Value.templateId.IsEliteFrostineMission2019());
            if (elite_frostnite_mission.Value == null)
            {
                return false;
            }
            else
            {
                return elite_frostnite_mission.Value.attributes.completion_s11_holdfastquest_seasonelite_knight > 0;
            }
        }
        private static bool IsEliteFrostineMission2019(this string templateId)
        {
            var b = templateId.Contains("s11_holdfastquest_seasonelite_knight", StringComparison.InvariantCultureIgnoreCase);
            return b;
        }
        public static Task<int> CalcResearchFORTs(this IQueryProfile qp)
        {
            return Task.Run(() =>
            {
                var rsrsc = qp.profileChanges.First().profile.items
                    .Where(p => p.Value.templateId.IndexOf("Stat:") > -1 && p.Value.templateId.IndexOf("_phoenix") == -1);
                var rsrcgrp = rsrsc.GroupBy(f => f.Value.templateId.ToResearchUniqueName());

                var totalResource = rsrcgrp.Sum(f => f.Sum(x => x.Value.quantity));
                return totalResource;
            });
        }

    }
}