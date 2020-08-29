using Fortnite.Core.Interfaces;
using Fortnite.Localization;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Static.Models;
using Fortnite.Static.Models.Survivors;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fortnite.Static
{
    public static class SurvivorStaticData
    {
        public static Dictionary<string, AssetMap> AssetBot2 = null;

        private static List<double> eneryTable = null;

        private static Dictionary<SurvivorRarity, BaseXp> ObjectsBaseXpTable = null;

        private static Dictionary<int, Dictionary<ObjectType, Dictionary<SurvivorRarity, EvolutionRequirement>>> ObjectsEvolutionTable = null;

        private static List<int> ResearchTable = null;

        public static Task<double> CalcEnergyByFORT(int calculatedFortPoints)
        {
            return Task.Run(() =>
            {
                double returnLevel = 1;
                int selectedIndex = eneryTable.FindLastIndex(f => calculatedFortPoints >= f);
                if (selectedIndex < 0)
                {
                    return returnLevel;
                }
                else
                {
                    returnLevel = selectedIndex + 1;
                }
                var selectedFortPoints = eneryTable[selectedIndex];
                if (calculatedFortPoints > selectedFortPoints)
                {
                    var myResult = calculatedFortPoints - selectedFortPoints;
                    var currentResult = eneryTable[selectedIndex + 1] - eneryTable[selectedIndex];
                    double percent = myResult / currentResult;
                    percent = Math.Round(percent, 2, MidpointRounding.AwayFromZero);
                    returnLevel += percent + 0.01;
                }
                else
                {
                }
                return returnLevel;
            });
        }

        public static Dictionary<string, int> CalcResourceRequirements(this ISurvivorX survivor, int maxLevel = 50)
        {
            var requiredResources = new Dictionary<string, int>();
            int requiredLevel = maxLevel - survivor.Level;
            if (requiredLevel > 0)
            {
                var survivorBaseXp = ObjectsBaseXpTable[survivor.Rarity].Survivors;
                int requiredSurvivorXp = survivorBaseXp * (maxLevel * (maxLevel - 1) - survivor.Level * (survivor.Level - 1)) / 2;
                if (requiredSurvivorXp > 0)
                {
                    requiredResources.Add("personnelxp", requiredSurvivorXp);
                }

                for (int i = survivor.Tier + 1; i <= 5; i++)
                {
                    EvolutionRequirement evolutionRequirements = new EvolutionRequirement();
                    if (survivor.IsLeader)
                    {
                        if (ObjectsEvolutionTable[i][ObjectType.LeaderSurvivors]
                            .ContainsKey(survivor.Rarity))
                        {
                            evolutionRequirements =
                                ObjectsEvolutionTable[i][ObjectType.LeaderSurvivors][
                                    survivor.Rarity];
                        }
                    }
                    else
                    {
                        if (ObjectsEvolutionTable[i][ObjectType.Survivors]
                            .ContainsKey(survivor.Rarity))
                        {
                            evolutionRequirements =
                                ObjectsEvolutionTable[i][ObjectType.Survivors][
                                    survivor.Rarity];
                        }
                    }

                    foreach (var resource in evolutionRequirements.Resource)
                    {
                        if (requiredResources.ContainsKey(resource.Key))
                        {
                            requiredResources[resource.Key] += resource.Value;
                        }
                        else
                        {
                            requiredResources.Add(resource.Key, resource.Value);
                        }
                    }
                }
            }
            return requiredResources;
        }

        public static Task<int> CalcSurvivorFORTs(this IEnumerable<IGrouping<string, ISurvivorX>> gqp)
        {
            return Task.Run(async () =>
            {
                IEnumerable<ISurvivorSquad> ssq = await gqp.GetSurvivorSquads();
                var total2 = (int)ssq.Sum(f => f.SquadBonus);
                return total2;
            });
        }

        public static string Get4xGroupMissionImage()
        {
            //[34] = GroupMissionsMini (527095018176118794)//white
            //[35] = GroupMissionsMini2 (527098396876472321)//black
            //[] = GroupMissionsMini3 (582013941559984199)//cyan
            return "<:582013941559984199:582013941559984199>";
        }

        public static Task<Dictionary<string, IMissionCategory>> GetAlerts(this IQueryProfile qp, params Alert[] alertNames)
        {
            return Task.Run(() =>
            {
                var p = qp.profileChanges.First()
                    .profile.stats.First()
                    .Value.mission_alert_redemption_record.claimDataMap;
                var list = p.ParseAlerts();

                for (int i = 0; i < list.Count; i++)
                {
                    var key = list.Keys.ToArray()[i];
                    if (alertNames.Any() && !alertNames.Contains((Alert)Enum.Parse(typeof(Alert), key)))
                    {
                        list.Remove(key);
                        i--;
                        continue;
                    }
                }

                return list;
            });
        }

        public static string GetEmojiId(string templateId)
        {
            if (AssetBot2.ContainsKey(templateId))
            {
                return AssetBot2[templateId].EmojiId;
            }
            else
            {
                if (templateId.StartsWith("eventcurrency_") && templateId != "eventcurrency_scaling")
                {
                    return "522901892725211146";
                }
                return "";
            }
        }

        public static string GetEmojiName(string templateId, string language)
        {
            try
            {
                var name = Global.DIManager.Services.GetRequiredService<IJsonStringLocalizer>().GetAssetRegistryTranslation(templateId, language);

                return name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static double[] GetLevel_EvoluationConstants(SurvivorRarity type, bool IsLeaderSurvivor)
        {
            double[] list = new double[2];
            switch (type)
            {
                case SurvivorRarity.Common:
                    list = new double[] { 1, 5 };
                    if (IsLeaderSurvivor)
                    {
                        list = new double[] { 0, 0 };
                    }

                    break;

                case SurvivorRarity.UnCommon:
                    list = new double[] { 1.08, 6.35 };
                    if (IsLeaderSurvivor)
                    {
                        list = new double[] { 1, 5 };
                    }

                    break;

                case SurvivorRarity.Rare:
                    list = new double[] { 1.245, 7 };
                    if (IsLeaderSurvivor)
                    {
                        list = new double[] { 1.08, 6.35 };
                    }

                    break;

                case SurvivorRarity.Epic:
                    list = new double[] { 1.374, 8 };
                    if (IsLeaderSurvivor)
                    {
                        list = new double[] { 1.245, 7 };
                    }

                    break;

                case SurvivorRarity.Legendary:
                    list = new double[] { 1.5, 9 };
                    if (IsLeaderSurvivor)
                    {
                        list = new double[] { 1.374, 8 };
                    }

                    break;

                case SurvivorRarity.Mythic:
                    list = new double[] { 1.645, 9.85 };
                    if (IsLeaderSurvivor)
                    {
                        list = new double[] { 1.5, 9 };
                    }

                    break;
            }
            return list;
        }

        public static Task<List<Resource>> GetResources(this IQueryProfile qp, string languageName)
        {
            return Task.Run(() =>
            {
                var accresource = "AccountResource:";
                var dt = qp.profileChanges.FirstOrDefault().profile.items
                    .Where(p => p.Value.templateId.IndexOf(accresource) > -1);

                var svrs = dt.Select(f => new Resource
                {
                    id = f.Value.templateId.Replace(accresource, ""),
                    quantity = f.Value.quantity,
                    img = $"{GetEmojiId(f.Value.templateId.Replace(accresource, ""))}",
                    name = $"{GetEmojiName(f.Value.templateId.Replace(accresource, ""), languageName)}"
                });
                return svrs.Where(f => !string.IsNullOrWhiteSpace(f.img)).OrderBy(p => p.id).ToList();
            });
        }

        public static List<IGrouping<string, SSDInfo>> GetSSDs(this IQueryProfile qp, string filteredByZoneId = null)
        {
            var qq = qp.profileChanges.First().profile.items.Where(f => f.Value.templateId.StartsWith("Quest:outpostquest_")).OrderBy(f => f.Value.GetZoneName()).ThenByDescending(x => x.Value.GetSSDLevel()).Select(f => new SSDInfo { Zone = f.Value.GetZoneName(), Level = f.Value.GetSSDLevel() }).ToList();
            if (filteredByZoneId != null)
            {
                switch (filteredByZoneId)
                {
                    case "33A2311D4AE64B361CCE27BC9F313C8B"://stonewood
                        qq = qq.Where(f => f.Zone == "stonewood").ToList();
                        break;

                    case "D477605B4FA48648107B649CE97FCF27"://plankerton
                        qq = qq.Where(f => f.Zone == "plankerton").ToList();
                        break;

                    case "E6ECBD064B153234656CB4BDE6743870"://cannyvalley
                        qq = qq.Where(f => f.Zone == "cannyvalley").ToList();
                        break;

                    case "D9A801C5444D1C74D1B7DAB5C7C12C5B"://twinepeaks
                        qq = qq.Where(f => f.Zone == "twinepeaks").ToList();
                        break;
                }
            }
            return qq.GroupBy(f => f.Zone).ToList();
        }

        public static Task<string> GName(this string templateId)
        {
            return Task.Run(() =>
            {
                var survivorType = templateId.GType().Result;

                var indexType = templateId.IndexOf($"_{survivorType}_");
                var name = templateId.Substring(0, indexType)
                    .TrimStart("Worker:".ToCharArray());

                int tier = templateId.GTier().Result;
                var indexTier = templateId.IndexOf($"_t0{tier}");
                var SpecialName = "";

                if (indexTier - (indexType + 4) > -1)
                {
                    SpecialName = "-" + templateId.Substring(indexType + 4, indexTier - (indexType + 4));
                }

                var newName = name.Replace("manager", "m-");

                return $"{newName}{SpecialName}";
            });
        }

        public static Task<byte> GTier(this string templateId)
        {
            return Task.Run(() =>
            {
                var splt = byte.Parse(templateId.Split(new string[] { "_t0" }, StringSplitOptions.None)[1]);
                return splt;
            });
        }

        public static Task<string> GType(this string templateId)
        {
            return Task.Run(() =>
            {
                var split = templateId
                    .Split('_')
                    .First(f => f.Length == 2 | f.Length == 1);
                return split;
            });
        }

        public static string InvinsibleImage()
        {
            return "<:524298550436429824:524298550436429824>";
        }

        public static void LoadStatics()
        {
            eneryTable = new List<double>()
            {
                0,5,8,27,58,75,93,107,120,131,142,167,193,
                219,245,311,378,429,480,583,685,748,810,904,
                998,1066,1134,1202,1225,1248,1272,1314,1357,
                1399,1423,1447,1470,1570,1670,1770,1930,2089,
                2248,2320,2391,2463,2539,2615,2692,2790,2889,
                2988,3043,3099,3154,3247,3339,3432,3522,3613,
                3703,3811,3920,4028,4098,4168,4238,4357,4475,
                4593,4704,4816,4927,5022,5117,5212,5307,5402,
                5497,5609,5721,5833,5992,6151,6310,6470,6633,
                6788,6973,7154,7331,7484,7633,7782,7920,8058,
                8196,8431,8667,8902,9078,9253,9428,9597,9669,
                9728,9802,9868,9971,10082,10189,10296,10411,10503,
                10678,10702,10925,11000,11140,11266,11466,11649,
                11864,12046,12176,12320,12465,12606,12724,12838.5,
                12959,13055,13248,13419,13589,13760,13897,14035,
                14173,14315
            };

            ResearchTable = new List<int>()
            {
                1,1,1,1,1,1,1,1,1,8,
                1,1,1,1,1,1,1,1,1,12,
                1,1,1,1,1,1,1,1,1,18,
                1,1,1,1,1,1,1,1,1,26,
                1,1,1,1,1,1,1,1,1,34,
                1,1,1,1,1,1,1,1,1,40,
                1,1,1,1,1,1,1,1,1,46,
                1,1,1,1,1,1,1,1,1,52,
                1,1,1,1,1,1,1,1,1,58,
                1,1,1,1,1,1,1,1,1,62,
                1,1,1,1,1,1,1,1,1,66,
                1,1,1,1,1,1,1,1,1,70
            };
            AssetBot2 = new Dictionary<string, AssetMap>()
            {
                {"null" ,new AssetMap(){ EmojiId="524246128925999114", StoredName="?"}},
                {"reagent_evolverarity_r" ,new AssetMap(){ EmojiId="520572428192055296",StoredName="Rare Flux", Url="" }},
                {"reagent_alteration_ele_nature" ,new AssetMap(){ EmojiId="520572424195014666",StoredName="Amp-Up", Url="" }},
                {"reagent_people" ,new AssetMap(){ EmojiId="520572425294053388",StoredName="Training Manual", Url="" }},
                {"reagent_c_t02" ,new AssetMap(){ EmojiId="520572424153202698",StoredName="Lightning In A Bottle", Url="" }},
                {"specialcurrency_daily" ,new AssetMap(){ EmojiId="520572432935813120",StoredName="Daily Coins", Url="" }},
                {"peopleresource" ,new AssetMap(){ EmojiId="520672077045432321",StoredName="People", Url="" }},
                {"reagent_alteration_upgrade_sr" ,new AssetMap(){ EmojiId="520572424157396992",StoredName="Legendary Perk-Up", Url="" }},
                {"reagent_alteration_ele_fire" ,new AssetMap(){ EmojiId="520572423850950670",StoredName="Fire-Up", Url="" }},
                {"reagent_c_t03" ,new AssetMap(){ EmojiId="520572432139026433",StoredName="Eye Of The Storm", Url="" }},
                {"reagent_evolverarity_vr" ,new AssetMap(){ EmojiId="520572424740274187",StoredName="Epic Flux", Url="" }},
                {"reagent_c_t01" ,new AssetMap(){ EmojiId="520572424685617154",StoredName="Pure Drop Of Rain", Url="" }},
                {"reagent_alteration_upgrade_uc" ,new AssetMap(){ EmojiId="520572424291352576",StoredName="Uncommon Perk-Up", Url="" }},
                {"reagent_alteration_generic" ,new AssetMap(){ EmojiId="520572424383889408",StoredName="Re-Perk", Url="" }},
                {"reagent_evolverarity_sr" ,new AssetMap(){ EmojiId="520572426761928714",StoredName="Legenday Flux", Url="" }},
                {"reagent_traps" ,new AssetMap(){ EmojiId="520572425554100224",StoredName="Trap Designs", Url="" }},
                {"reagent_alteration_ele_water" ,new AssetMap(){ EmojiId="520572423800881153",StoredName="Frost-Up", Url="" }},
                {"reagent_alteration_upgrade_vr" ,new AssetMap(){ EmojiId="520572424178237450",StoredName="Epic Perk-Up", Url="" }},
                {"reagent_c_t04" ,new AssetMap(){ EmojiId="520572424849457172",StoredName="Storm Shard", Url="" }},
                {"eventcurrency_snowballs" ,new AssetMap(){ EmojiId="522901892725211146",StoredName="Event Tickets", Url="" }},
                {"eventcurrency_spring" ,new AssetMap(){ EmojiId="522901892725211146",StoredName="Event Tickets", Url="" }},
                {"eventcurrency_summer" ,new AssetMap(){ EmojiId="522901892725211146",StoredName="Event Tickets", Url="" }},
                {"voucher_basicpack" ,new AssetMap(){ EmojiId="523865023773409282",StoredName="Tiny Llama", Url="" }},
                {"reagent_weapons" ,new AssetMap(){ EmojiId="520572424639741953",StoredName="Weapon Designs", Url="" }},
                {"personnelxp" ,new AssetMap(){ EmojiId="520572424257798144",StoredName="Survivor Xp", Url="" }},
                {"heroxp" ,new AssetMap(){ EmojiId="520572424178237440",StoredName="Hero Xp", Url="" }},
                {"schematicxp" ,new AssetMap(){ EmojiId="520572424224243732",StoredName="Schematic Xp", Url="" }},
                {"improvised" ,new AssetMap(){ EmojiId="524196475786493953",StoredName="Active Powercell", Url="" }},
                {"eventcurrency_scaling" ,new AssetMap(){ EmojiId="524040663868768256",StoredName="Gold", Url="" }},
                {"eventscaling" ,new AssetMap(){ EmojiId="524040663868768256",StoredName="Gold", Url="" }},
                {"campaign_event_currency" ,new AssetMap(){ EmojiId="522901892725211146",StoredName="Event Tickets", Url="" }},
                {"reagent_alteration_upgrade_r" ,new AssetMap(){ EmojiId="520572424287289344",StoredName="Rare Perk-Up", Url="" }},
                {"MtxGiveaway" ,new AssetMap(){ EmojiId="523870428192571402",StoredName="V-Bucks", Url="" }},
                {"currency_mtxswap" ,new AssetMap(){ EmojiId="523870428192571402",StoredName="V-Bucks", Url="" }},
                {"ore_malachite" ,new AssetMap(){ EmojiId="523879090772508684",StoredName="Malachite", Url="" }},
                {"crystal_shadowshard" ,new AssetMap(){ EmojiId="523879161668829200",StoredName="Shadowshard", Url="" }},
                {"ore_obsidian" ,new AssetMap(){ EmojiId="523878792205303809",StoredName="Obsidian", Url="" }},
                {"ceiling_electric_single_{0}" ,new AssetMap(){ EmojiId="523875436032294954",StoredName="({0}) Ceiling Zapper", Url="" }},
                {"pistol_autoheavy_{0}_ore" ,new AssetMap(){ EmojiId="523877112474501141",StoredName="({0}) Pistol", Url="" }},
                {"edged_axe_medium_{0}_ore" ,new AssetMap(){ EmojiId="523877324610076674",StoredName="({0}) AXE", Url="" }},
                {"shotgun_standard_{0}_ore" ,new AssetMap(){ EmojiId="523872675509043213",StoredName="({0}) Shotgun", Url="" }},
                {"assault_semiauto_{0}_ore" ,new AssetMap(){ EmojiId="523872675718758410",StoredName="({0}) Assault", Url="" }},
                {"trap_core_consumable_{0}" ,new AssetMap(){ EmojiId="523877864253161472",StoredName="({0}) Trap", Url="" }},
                {"ranged_pistol_consumable_{0}" ,new AssetMap(){ EmojiId="523872350207213588",StoredName="({0}) Pistol", Url="" }},
                {"ranged_sniper_consumable_{0}" ,new AssetMap(){ EmojiId="523872377747144736",StoredName="({0}) Sniper", Url="" }},
                {"crystal_quartz" ,new AssetMap(){ EmojiId="523940369050763267",StoredName="Quartz Crystal", Url="" }},
                {"ore_copper" ,new AssetMap(){ EmojiId="523941247136694275",StoredName="Copper", Url="" }},
                {"workerbasic_{0}" ,new AssetMap(){ EmojiId="523872595544899584",StoredName="({0}) Survivor", Url="" }},
                {"pistol_boltrevolver_{0}_ore" ,new AssetMap(){ EmojiId="523958167714004995",StoredName="({0}) Pistol", Url="" }},
                {"sniper_auto_{0}_ore" ,new AssetMap(){ EmojiId="523958708804517899",StoredName="({0}) Automatic Sniper", Url="" }},
                {"shotgun_auto_{0}_ore" ,new AssetMap(){ EmojiId="523959220643823638",StoredName="({0}) Automatic Shotgun", Url="" }},
                {"melee_tool_consumable_{0}" ,new AssetMap(){ EmojiId="523960181248491536",StoredName="({0}) Tool", Url="" }},
                {"melee_sword_consumable_{0}" ,new AssetMap(){ EmojiId="523959775222956043",StoredName="({0}) Sword", Url="" }},
                {"blunt_light_rocketbat_{0}_ore" ,new AssetMap(){ EmojiId="523961261621248015",StoredName="({0}) Baseball Bat", Url="" }},
                {"blunt_medium_{0}_ore" ,new AssetMap(){ EmojiId="523961261600538625",StoredName="({0}) Wrench", Url="" }},
                {"defenderassault_basic_{0}" ,new AssetMap(){ EmojiId="523961783749443605",StoredName="({0}) Assault Defender", Url="" }},
                {"defenderpistol_basic_{0}" ,new AssetMap(){ EmojiId="523961601972240413",StoredName="({0}) Pistol Defender", Url="" }},
                {"ranged_shotgun_consumable_{0}" ,new AssetMap(){ EmojiId="524032771572760624",StoredName="({0}) Shotgun", Url="" }},
                {"defendersniper_basic_m_{0}" ,new AssetMap(){ EmojiId="524033181448536105",StoredName="({0}) Sniper Defender", Url="" }},
                {"defendersniper_basic_{0}" ,new AssetMap(){ EmojiId="524033354622828555",StoredName="({0}) Sniper Defender", Url="" }},
                {"commando_grenadegun_{0}" ,new AssetMap(){ EmojiId="524033606163628068",StoredName="({0}) Hero", Url="" }},
                {"ranged_assault_consumable_{0}" ,new AssetMap(){ EmojiId="524033794471231491",StoredName="({0}) Ranged Assault", Url="" }},
                {"ranged_smg_consumable_{0}" ,new AssetMap(){ EmojiId="524033794471231491",StoredName="({0}) Ranged SMG", Url="" }},
                {"sniper_amr_{0}_ore" ,new AssetMap(){ EmojiId="524070084449140736",StoredName="({0}) Sniper", Url="" }},
                {"melee_spear_consumable_{0}" ,new AssetMap(){ EmojiId="524244684197986337",StoredName="({0}) Spear", Url="" }},
                {"melee_scythe_consumable_{0}" ,new AssetMap(){ EmojiId="525385114096762882",StoredName="({0}) Scythe", Url="" }},
                {"sniper_standard_{0}_ore" ,new AssetMap(){ EmojiId="525385114096762882",StoredName="({0}) Sniper", Url="" }},
                {"sniper_boltaction_{0}_ore" ,new AssetMap(){ EmojiId="525385114096762882",StoredName="({0}) Sniper", Url="" }},
                {"blunt_light_{0}_ore" ,new AssetMap(){ EmojiId="525385114096762882",StoredName="({0}) Club", Url="" }},
                {"edged_sword_medium_laser_{0}_ore" ,new AssetMap(){ EmojiId="523959775222956043",StoredName="({0}) Sword", Url="" }},
                {"ore_silver" ,new AssetMap(){ EmojiId="524500953156550667",StoredName="Silver", Url="" }},
                {"crystal_sunbeam" ,new AssetMap(){ EmojiId="524527322771095563",StoredName="Sunbeam", Url="" }},
                {"elementalzonefireenableitem" ,new AssetMap(){ EmojiId="525113560750096395",StoredName="empty", Url="" }},
                {"elementalzonenatureenableitem" ,new AssetMap(){ EmojiId="525113560813273128",StoredName="empty", Url="" }},
                {"elementalzonewaterenableitem" ,new AssetMap(){ EmojiId="525113560666341391",StoredName="empty", Url="" }},
                {"gm_basehusk_ondeath_explode" ,new AssetMap(){ EmojiId="525113560917999626",StoredName="empty", Url="" }},
                {"gm_basehusk_ondmgdealt_metalcorrosion" ,new AssetMap(){ EmojiId="525113560808947712",StoredName="empty", Url="" }},
                {"gm_constructor_buildcost_buff" ,new AssetMap(){ EmojiId="525113632149864448",StoredName="empty", Url="" }},
                {"gm_constructor_damage_buff" ,new AssetMap(){ EmojiId="525126858497327105",StoredName="empty", Url="" }},
                {"gm_enemy_hideonminimap" ,new AssetMap(){ EmojiId="525113560767135754",StoredName="empty", Url="" }},
                {"gm_enemy_ondeath_applyspeedmods" ,new AssetMap(){ EmojiId="525113560704090122",StoredName="empty", Url="" }},
                {"gm_enemy_ondeath_areaheal" ,new AssetMap(){ EmojiId="525113560502632451",StoredName="empty", Url="" }},
                {"gm_enemy_ondeath_spawndamagepool" ,new AssetMap(){ EmojiId="525113560586780673",StoredName="empty", Url="" }},
                {"gm_enemy_ondeath_spawnenemyrangeresistpool" ,new AssetMap(){ EmojiId="525113560804622370",StoredName="empty", Url="" }},
                {"gm_enemy_ondeath_spawnplayerslowpool" ,new AssetMap(){ EmojiId="525113560636981249",StoredName="empty", Url="" }},
                {"gm_enemy_ondmgdealt_lifeleech" ,new AssetMap(){ EmojiId="525113560385323009",StoredName="empty", Url="" }},
                {"gm_enemy_ondmgdealt_slowdownfoe" ,new AssetMap(){ EmojiId="525113560796233738",StoredName="empty", Url="" }},
                {"gm_enemy_ondmgreceived_speedbuff" ,new AssetMap(){ EmojiId="525113560875925514",StoredName="empty", Url="" }},
                {"gm_enemy_onhitweakenbuildings" ,new AssetMap(){ EmojiId="525113560934776833",StoredName="empty", Url="" }},
                {"gm_hero_tech_buff" ,new AssetMap(){ EmojiId="525113631692685324",StoredName="empty", Url="" }},
                {"gm_ninja_abilityrate_buff" ,new AssetMap(){ EmojiId="525113632170967040",StoredName="empty", Url="" }},
                {"gm_ninja_damage_buff" ,new AssetMap(){ EmojiId="525113632183287820",StoredName="empty", Url="" }},
                {"gm_ninja_jumpheight_buff" ,new AssetMap(){ EmojiId="525113632112246784",StoredName="empty", Url="" }},
                {"gm_ninja_sword_damagebuff" ,new AssetMap(){ EmojiId="525113631940280331",StoredName="empty", Url="" }},
                {"gm_outlander_damage_buff" ,new AssetMap(){ EmojiId="525113632300990464",StoredName="empty", Url="" }},
                {"gm_outlander_tech_buff" ,new AssetMap(){ EmojiId="525113632397328394",StoredName="empty", Url="" }},
                {"gm_player_assaultrifle_damage_buff" ,new AssetMap(){ EmojiId="525113631789023254",StoredName="empty", Url="" }},
                {"gm_player_axesscythesdamage_buff" ,new AssetMap(){ EmojiId="525113632078430219",StoredName="empty", Url="" }},
                {"gm_player_blunt_damagebuff" ,new AssetMap(){ EmojiId="525113632309379073",StoredName="empty", Url="" }},
                {"gm_player_energy_damagebuff" ,new AssetMap(){ EmojiId="525127130997194752",StoredName="empty", Url="" }},
                {"gm_player_explosive_damagebuff" ,new AssetMap(){ EmojiId="525113632397328384",StoredName="empty", Url="" }},
                {"gm_player_meleeknockback_buff" ,new AssetMap(){ EmojiId="525113632078561291",StoredName="empty", Url="" }},
                {"gm_player_ondmgdealt_lifeleech" ,new AssetMap(){ EmojiId="525113632296665098",StoredName="empty", Url="" }},
                {"gm_player_onshielddestroyed_aoe" ,new AssetMap(){ EmojiId="525128342417178624",StoredName="empty", Url="" }},
                {"gm_player_pistol_damagebuff" ,new AssetMap(){ EmojiId="525113632195870733",StoredName="empty", Url="" }},
                {"gm_player_shotgun_damagebuff" ,new AssetMap(){ EmojiId="525113632347127808",StoredName="empty", Url="" }},
                {"gm_player_sniperrifle_damagebuff" ,new AssetMap(){ EmojiId="525113632376487956",StoredName="empty", Url="" }},
                {"gm_player_spearsworddamage_buff" ,new AssetMap(){ EmojiId="525113632124567563",StoredName="empty", Url="" }},
                {"gm_soldier_abilityrate_buff" ,new AssetMap(){ EmojiId="525113632431013911",StoredName="empty", Url="" }},
                {"gm_soldier_assaultrifle_buff" ,new AssetMap(){ EmojiId="525113632233750538",StoredName="empty", Url="" }},
                {"gm_soldier_damage_buff" ,new AssetMap(){ EmojiId="525127781118377994",StoredName="empty", Url="" }},
                {"gm_trap_buff" ,new AssetMap(){ EmojiId="525113632246333463",StoredName="empty", Url="" }},
                {"minibossenableprimarymissionitem" ,new AssetMap(){ EmojiId="525113560897028097",StoredName="empty", Url="" }},
                {"squad_attribute_scavenging_gadgeteers" ,new AssetMap(){ EmojiId="520576809322938368",StoredName="Gadgeteers", Url="" }},
                {"squad_attribute_scavenging_scoutingparty" ,new AssetMap(){ EmojiId="520576809532653589",StoredName="Scouting party", Url="" }},
                {"squad_attribute_synthesis_thethinktank" ,new AssetMap(){ EmojiId="520576809536847893",StoredName="The think tank", Url="" }},
                {"squad_attribute_arms_fireteamalpha" ,new AssetMap(){ EmojiId="520576809562144774",StoredName="Fire team alpha", Url="" }},
                {"squad_attribute_arms_closeassaultsquad" ,new AssetMap(){ EmojiId="520576809671065610",StoredName="Close assault", Url="" }},
                {"squad_attribute_synthesis_corpsofengineering" ,new AssetMap(){ EmojiId="520576809905946647",StoredName="Corps of engineering", Url="" }},
                {"squad_attribute_medicine_trainingteam" ,new AssetMap(){ EmojiId="520576809935437834",StoredName="Training Team", Url="" }},
                {"squad_attribute_medicine_emtsquad" ,new AssetMap(){ EmojiId="520576810442948608",StoredName="EMT Squad", Url="" }},
            };
            //################### object base xp table
            ObjectsBaseXpTable = new Dictionary<SurvivorRarity, BaseXp>()
            {
                {SurvivorRarity.Common, new BaseXp() { Survivors=100, Defenders=50 }},
                {SurvivorRarity.UnCommon, new BaseXp() { Survivors=150, Defenders=75 }},
                {SurvivorRarity.Rare, new BaseXp() { Survivors=200, Defenders=100 }},
                {SurvivorRarity.Epic, new BaseXp() { Survivors=250, Defenders=125 }},
                {SurvivorRarity.Legendary, new BaseXp() { Survivors=300, Defenders=150 }},
                {SurvivorRarity.Mythic, new BaseXp() { Survivors=400, Heroes=300, Defenders=150 }},
            };

            //############################################################## evolution table
            /////////######### survivors
            ///
            /// USE CUSTOM METHOD FOR USING INDEPENDET CHANGES
            int currentStar = 2;
            //star 2
            ObjectsEvolutionTable = new Dictionary<int, Dictionary<ObjectType, Dictionary<SurvivorRarity, EvolutionRequirement>>>
            {
                { currentStar, new Dictionary<ObjectType, Dictionary<SurvivorRarity, EvolutionRequirement>>() }
            };

            ObjectsEvolutionTable[currentStar].Add(ObjectType.Survivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());
            ObjectsEvolutionTable[currentStar].Add(ObjectType.LeaderSurvivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());

            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Common, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 500 }, { "reagent_c_t01", 3 }, { "reagent_people", 0 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.UnCommon, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 1000 }, { "reagent_c_t01", 4 }, { "reagent_people", 0 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.UnCommon, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 1000 }, { "reagent_c_t01", 6 }, { "reagent_people", 0 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Rare, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 1500 }, { "reagent_c_t01", 6 }, { "reagent_people", 1 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Rare, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 1500 }, { "reagent_c_t01", 9 }, { "reagent_people", 1 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 2000 }, { "reagent_c_t01", 8 }, { "reagent_people", 2 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 2000 }, { "reagent_c_t01", 13 }, { "reagent_people", 2 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 2500 }, { "reagent_c_t01", 10 }, { "reagent_people", 3 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 2500 }, { "reagent_c_t01", 18 }, { "reagent_people", 3 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 7500 }, { "reagent_c_t01", 12 }, { "reagent_people", 4 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 3000 }, { "reagent_c_t01", 23 }, { "reagent_people", 3 } },
            });
            //star 3
            currentStar++;
            ObjectsEvolutionTable.Add(currentStar, new Dictionary<ObjectType, Dictionary<SurvivorRarity, EvolutionRequirement>>());

            ObjectsEvolutionTable[currentStar].Add(ObjectType.Survivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());
            ObjectsEvolutionTable[currentStar].Add(ObjectType.LeaderSurvivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());

            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.UnCommon, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 2500 }, { "reagent_c_t01", 8 }, { "reagent_people", 1 }, { "reagent_c_t02", 3 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.UnCommon, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 1875 }, { "reagent_c_t01", 12 }, { "reagent_people", 1 }, { "reagent_c_t02", 4 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Rare, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 3750 }, { "reagent_c_t01", 12 }, { "reagent_people", 3 }, { "reagent_c_t02", 4 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Rare, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 3250 }, { "reagent_c_t01", 18 }, { "reagent_people", 3 }, { "reagent_c_t02", 6 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 5000 }, { "reagent_c_t01", 16 }, { "reagent_people", 5 }, { "reagent_c_t02", 6 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 4625 }, { "reagent_c_t01", 26 }, { "reagent_people", 5 }, { "reagent_c_t02", 8 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 6250 }, { "reagent_c_t01", 20 }, { "reagent_people", 8 }, { "reagent_c_t02", 8 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 6000 }, { "reagent_c_t01", 36 }, { "reagent_people", 8 }, { "reagent_c_t02", 10 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 7500 }, { "reagent_c_t01", 24 }, { "reagent_people", 10 }, { "reagent_c_t02", 10 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 7375 }, { "reagent_c_t01", 46 }, { "reagent_people", 8 }, { "reagent_c_t02", 12 } },
            });

            //star 4
            currentStar++;
            ObjectsEvolutionTable.Add(currentStar, new Dictionary<ObjectType, Dictionary<SurvivorRarity, EvolutionRequirement>>());

            ObjectsEvolutionTable[currentStar].Add(ObjectType.Survivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());
            ObjectsEvolutionTable[currentStar].Add(ObjectType.LeaderSurvivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());

            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Rare, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 6000 }, { "reagent_c_t01", 18 }, { "reagent_people", 6 }, { "reagent_c_t02", 8 }, { "reagent_c_t03", 4 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Rare, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 4750 }, { "reagent_c_t01", 27 }, { "reagent_people", 6 }, { "reagent_c_t02", 12 }, { "reagent_c_t03", 6 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 8000 }, { "reagent_c_t01", 24 }, { "reagent_people", 9 }, { "reagent_c_t02", 12 }, { "reagent_c_t03", 6 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 6875 }, { "reagent_c_t01", 39 }, { "reagent_people", 9 }, { "reagent_c_t02", 16 }, { "reagent_c_t03", 8 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 10000 }, { "reagent_c_t01", 30 }, { "reagent_people", 13 }, { "reagent_c_t02", 16 }, { "reagent_c_t03", 8 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 9000 }, { "reagent_c_t01", 54 }, { "reagent_people", 13 }, { "reagent_c_t02", 20 }, { "reagent_c_t03", 10 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 12000 }, { "reagent_c_t01", 36 }, { "reagent_people", 17 }, { "reagent_c_t02", 20 }, { "reagent_c_t03", 20 } },
            });

            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 11125 }, { "reagent_c_t01", 69 }, { "reagent_people", 13 }, { "reagent_c_t02", 24 }, { "reagent_c_t03", 12 } },
            });
            //star 5
            currentStar++;
            ObjectsEvolutionTable.Add(currentStar, new Dictionary<ObjectType, Dictionary<SurvivorRarity, EvolutionRequirement>>());

            ObjectsEvolutionTable[currentStar].Add(ObjectType.Survivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());
            ObjectsEvolutionTable[currentStar].Add(ObjectType.LeaderSurvivors, new Dictionary<SurvivorRarity, EvolutionRequirement>());

            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 11000 }, { "reagent_c_t01", 32 }, { "reagent_people", 12 }, { "reagent_c_t02", 18 }, { "reagent_c_t03", 12 }, { "reagent_c_t04", 6 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Epic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 9500 }, { "reagent_c_t01", 52 }, { "reagent_people", 12 }, { "reagent_c_t02", 24 }, { "reagent_c_t03", 16 }, { "reagent_c_t04", 8 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 13750 }, { "reagent_c_t01", 40 }, { "reagent_people", 18 }, { "reagent_c_t02", 24 }, { "reagent_c_t03", 16 }, { "reagent_c_t04", 8 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Legendary, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 12500 }, { "reagent_c_t01", 72 }, { "reagent_people", 18 }, { "reagent_c_t02", 30 }, { "reagent_c_t03", 20 }, { "reagent_c_t04", 10 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.Survivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 16500 }, { "reagent_c_t01", 48 }, { "reagent_people", 23 }, { "reagent_c_t02", 30 }, { "reagent_c_t03", 30 }, { "reagent_c_t04", 10 } },
            });
            ObjectsEvolutionTable[currentStar][ObjectType.LeaderSurvivors].Add(SurvivorRarity.Mythic, new EvolutionRequirement()
            {
                Resource = new Dictionary<string, int>() { { "personnelxp", 15500 }, { "reagent_c_t01", 92 }, { "reagent_people", 18 }, { "reagent_c_t02", 36 }, { "reagent_c_t03", 24 }, { "reagent_c_t04", 12 } },
            });
            ///////////########
            //##########################################
        }
        public static double TeamBonus(this ISurvivorSquad squad, ISurvivorX survivor)
        {
            if (survivor.IsLeader)
            {
                return 0;
            }
            bool hasteambonus = false;
            bool hasLeader = squad.Leader != null;
            if (hasLeader)
            {
                hasteambonus = survivor.Personality == squad.Leader.Personality;
            }
            if (hasteambonus)
            {
                switch (squad.Leader.Rarity)
                {
                    case SurvivorRarity.Common:
                        return 0;
                        break;

                    case SurvivorRarity.UnCommon:
                        return 2;
                        break;

                    case SurvivorRarity.Rare:
                        return 3;
                        break;

                    case SurvivorRarity.Epic:
                        return 4;
                        break;

                    case SurvivorRarity.Legendary:
                        return 5;
                        break;

                    case SurvivorRarity.Mythic:
                        return 8;
                        break;
                }
            }
            else if (squad.Leader?.Rarity == SurvivorRarity.Mythic && survivor.Personality != squad.Leader?.Personality)
            {
                return -2;
            }
            return 0;
        }
    }
}