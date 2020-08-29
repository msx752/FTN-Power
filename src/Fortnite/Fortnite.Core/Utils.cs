using Fortnite.Core.Interfaces;
using Fortnite.Core.ModifiedModels;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Model.Responses.WorldInfo;
using fortniteLib.Responses.Catalog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Fortnite.Core
{
    public static partial class Utils
    {
        /// <summary>
        /// https://stackoverflow.com/questions/630803/associating-enums-with-strings-in-c-sharp
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToDescriptionString(this Alert val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : "undefined";
        }

        public static int GetIntegerPower(double AccountPowerLevel)
        {
            var energy = (int)Math.Round(AccountPowerLevel, MidpointRounding.AwayFromZero);
            if (energy > 131)
            {
                energy = 131;
            }
            return energy;
        }
        public static int GetMissionLevel(this AvailableMissionAlert availableMissionAlert)
        {
            var mr = availableMissionAlert.missionAlertRewards;
            Regex r = new Regex("Mission_Select_T(\\d+)", RegexOptions.Singleline);
            var m = r.Match(mr.tierGroupName);
            if (m.Success)
            {
                var Tier = byte.Parse(m.Groups[1].Value);
                return Utils.GuesMissionLevel(Tier);
            }
            else
            {
                r = new Regex("Outpost_Select_Theater(\\d)", RegexOptions.Singleline);
                m = r.Match(mr.tierGroupName);
                if (m.Success)//stormshiel detected no energy
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
        }

        public static int GuesMissionLevel(int Tier)
        {
            if (Tier >= 25)
            {
                return 140;
            }
            else if (Tier >= 24)
            {
                return 132;
            }
            else if (Tier >= 23)
            {
                return 124;
            }
            else if (Tier >= 22)
            {
                return 116;
            }
            else if (Tier >= 21)
            {
                return 108;
            }
            else if (Tier >= 20)
            {
                return 100;
            }
            else if (Tier >= 19)
            {
                return 94;
            }
            else if (Tier >= 18)
            {
                return 88;
            }
            else if (Tier >= 17)
            {
                return 82;
            }
            else if (Tier >= 16)
            {
                return 76;
            }
            else if (Tier >= 15)
            {
                return 70;
            }
            else if (Tier >= 14)
            {
                return 64;
            }
            else if (Tier >= 13)
            {
                return 58;
            }
            else if (Tier >= 12)
            {
                return 52;
            }
            else if (Tier >= 11)
            {
                return 46;
            }
            else if (Tier >= 10)
            {
                return 40;
            }
            else if (Tier >= 9)
            {
                return 34;
            }
            else if (Tier >= 8)
            {
                return 28;
            }
            else if (Tier >= 7)
            {
                return 23;
            }
            else if (Tier >= 6)
            {
                return 19;
            }
            else if (Tier >= 5)
            {
                return 15;
            }
            else if (Tier >= 4)
            {
                return 9;
            }
            else if (Tier >= 3)
            {
                return 5;
            }
            else if (Tier >= 2)
            {
                return 3;
            }
            else if (Tier >= 1)
            {
                return 1;
            }
            else
            {
                return -2;
            }
        }

        public static ItemClass GetItemClass(string itemType)
        {
            ItemClass IClass = ItemClass.None;
            if (!Enum.TryParse<ItemClass>(itemType.Split(':')[0], out IClass))
            {
                throw new Exception($"undefined ItemClass :'{itemType}', please define in 'ItemClass' and add to 'GetItemTypeName();'");
            }
            return IClass;
        }

        public static bool IsLeaderAppropriate(string survivorSlotType, string survivorTemplateId)
        {
            switch (survivorSlotType)
            {
                case "squad_attribute_medicine_trainingteam":
                    return survivorTemplateId.Contains("managertrainer");
                    break;

                case "squad_attribute_synthesis_corpsofengineering":
                    return survivorTemplateId.Contains("managerengineer");
                    break;

                case "squad_attribute_synthesis_thethinktank":
                    return survivorTemplateId.Contains("managerinventor");
                    break;

                case "squad_attribute_medicine_emtsquad":
                    return survivorTemplateId.Contains("managerdoctor");
                    break;

                case "squad_attribute_arms_closeassaultsquad":
                    return survivorTemplateId.Contains("managermartialartist");
                    break;

                case "squad_attribute_scavenging_scoutingparty":
                    return survivorTemplateId.Contains("managerexplorer");
                    break;

                case "squad_attribute_arms_fireteamalpha":
                    return survivorTemplateId.Contains("managersoldier");
                    break;

                case "squad_attribute_scavenging_gadgeteers":
                    return survivorTemplateId.Contains("managergadgeteer");
                    break;

                default:
                    throw new Exception("new survivor squad is detected, please add it's conditions.");
                    break;
            }
        }

        public static IEnumerable<KeyValuePair<string, Item>> GetSurvivorList(this IQueryProfile qp)
        {
            var dt = qp.profileChanges.First().profile.items
                .Where(p => p.Value.templateId.IndexOf("Worker:") > -1);
            return dt;
        }

        public static string ToResearchUniqueName(this string str)
        {
            string val = str.Substring(0, str.Length);
            int indexOf_ = val.IndexOf('_');
            if (indexOf_ > -1)
                val = str.Substring(0, indexOf_);
            return val;
        }

        public static bool AnyEpic(this IMissionX m)
        {
            var count = m.Items.Any(f => f.Rarity == ItemRarity.Epic);
            return count;
        }

        public static bool AnyMythic(this IMissionX m)
        {
            var count = m.Items.Any(f => f.Rarity == ItemRarity.Mythic);
            return count;
        }

        public static bool AnyLegendary(this IMissionX m)
        {
            var count = m.Items.Any(f => f.Rarity == ItemRarity.Legendary);
            return count;
        }

        public static bool FindNormalItemById(this IMissionX m, string IdContains, WorldName worldName = WorldName.All)
        {
            if (worldName == WorldName.All)
            {
                var exsts = m.Items.Any(f => f.ItemType.Contains(IdContains, StringComparison.CurrentCultureIgnoreCase));
                return exsts;
            }
            else
            {
                if (m.WorldName != worldName)
                {
                    return false;
                }
                var exsts = m.Items.Any(f => f.AlertReward == false && f.ItemType.Contains(IdContains, StringComparison.CurrentCultureIgnoreCase));
                return exsts;
            }
        }

        public static bool HasAnyAlert(this IMissionX m, Alert alert)
        {
            return (m.MissionCategory == alert);
        }

        public static bool HasAnyElementalAlert(this IMissionX m)
        {
            return (m.MissionCategory == Alert.ElemementalZoneWaterCategory || m.MissionCategory == Alert.ElemementalZoneFireCategory || m.MissionCategory == Alert.ElemementalZoneNatureCategory);
        }

        public static bool HasAnyTransform(this IMissionX m, ItemRarity RarityType)
        {
            var count = m.Items.Any(f => f.Class == ItemClass.ConversionControl && f.Rarity == RarityType);
            return count;
        }

        public static bool HasEpicAnyTransform(this IMissionX m)
        {
            return m.HasAnyTransform(ItemRarity.Epic);
        }

        public static bool HasEpicHero(this IMissionX m)
        {
            return m.HasHero(ItemRarity.Epic);
        }

        public static bool HasEpicPerkUp(this IMissionX m, WorldName world)
        {
            var rslt = m.FindNormalItemById("Reagent_Alteration_Upgrade_VR", world);
            m.SetOrderNumber(rslt, 4);
            if (m.IsGroupMission)
            {
                m.SetOrderNumber(rslt, 3);
            }
            return rslt;
        }

        public static bool HasEpicSurvivor(this IMissionX m)
        {
            return m.HasSurvivor(ItemRarity.Epic);
        }

        public static bool HasHero(this IMissionX m, ItemRarity RarityType)
        {
            var count = m.Items.Any(f => f.Class == ItemClass.Hero && f.Rarity == RarityType);
            return count;
        }

        public static bool HasLegendaryAnyTransform(this IMissionX m)
        {
            return m.HasAnyTransform(ItemRarity.Legendary);
        }

        public static bool HasLegendaryDefender(this IMissionX m)
        {
            var count = m.Items.Any(f => f.Class == ItemClass.Defender && f.Rarity == ItemRarity.Legendary);
            m.SetOrderNumber(count, 2);
            return count;
        }

        public static bool HasLegendaryHero(this IMissionX m)
        {
            var rslt = m.HasHero(ItemRarity.Legendary);
            m.SetOrderNumber(rslt, 0);
            return rslt;
        }

        public static bool Has4xAny(this IMissionX m)
        {
            return m.IsGroupMission;
        }

        public static bool IsMissionReward(this IMissionX m, string IdContains)
        {
            var exsts = m.Items.Any(f => f.AlertReward == false && f.ItemType.Contains(IdContains, StringComparison.CurrentCultureIgnoreCase));
            return exsts;
        }

        public static bool Has4x(this IMissionX m, string IdContains, WorldName world = WorldName.Twine_Peaks)
        {
            if (!m.Has4xAny())
            {
                return false;
            }

            var rslt = m.FindNormalItemById(IdContains, world);
            if (!rslt)
            {
                return false;
            }

            rslt = m.IsMissionReward(IdContains);
            if (!rslt)
            {
                return false;
            }

            m.SetOrderNumber(rslt, 6);
            return true;
        }

        public static bool Has4xPureDropOfRain(this IMissionX m, WorldName world = WorldName.Twine_Peaks)
        {
            var rslt = m.Has4x("reagent_c_t01", world);
            return rslt;
        }

        public static bool Has4xLightningInABottle(this IMissionX m, WorldName world = WorldName.Twine_Peaks)
        {
            var rslt = m.Has4x("reagent_c_t02", world);
            return rslt;
        }

        public static bool Has4xEyeOfStorm(this IMissionX m, WorldName world = WorldName.Twine_Peaks)
        {
            var rslt = m.Has4x("reagent_c_t03", world);
            return rslt;
        }

        public static bool Has4xStormShard(this IMissionX m, WorldName world = WorldName.Twine_Peaks)
        {
            var rslt = m.Has4x("reagent_c_t04", world);
            return rslt;
        }

        public static bool HasLegendaryPerkUp(this IMissionX m, WorldName world)
        {
            var rslt = m.FindNormalItemById("Reagent_Alteration_Upgrade_SR", world);
            m.SetOrderNumber(rslt, 4);
            if (m.IsGroupMission)
            {
                m.SetOrderNumber(rslt, 3);
            }
            return rslt;
        }

        public static bool HasLegendaryShematic(this IMissionX m)
        {
            var count = m.Items.Any(f => f.Class == ItemClass.Schematic && f.Rarity == ItemRarity.Legendary);
            m.SetOrderNumber(count, 2);
            return count;
        }

        public static bool HasLegendarySurvivor(this IMissionX m)
        {
            var rslt = m.HasSurvivor(ItemRarity.Legendary);
            m.SetOrderNumber(rslt, 0);
            return rslt;
        }

        private static void SetOrderNumber(this IMissionX m, bool hasValue, int orderNumber)
        {
            if (hasValue)
            {
                if (m.OrderNumber > orderNumber)
                {
                    m.OrderNumber = orderNumber;
                }
            }
        }

        public static bool HasMythicHero(this IMissionX m)
        {
            var rslt = m.HasHero(ItemRarity.Mythic);
            m.SetOrderNumber(rslt, 0);
            return rslt;
        }

        public static bool HasMythicSurvivor(this IMissionX m)
        {
            var rslt = m.HasSurvivor(ItemRarity.Mythic);
            m.SetOrderNumber(rslt, 0);
            return rslt;
        }

        public static bool HasSurvivor(this IMissionX m, ItemRarity RarityType)
        {
            var exists = m.Items.Any(f => f.Class == ItemClass.Worker && f.Rarity == RarityType);
            return exists;
        }

        public static bool HasVBuck(this IMissionX m)
        {
            var rslt = m.FindNormalItemById("MtxGiveaway");
            if (!rslt)
            {
                rslt = m.FindNormalItemById("currency_mtxswap");
            }
            if (rslt)
            {
                m.SetOrderNumber(rslt, 0);
            }
            return rslt;
        }

        public static ItemRarity GetItemRarityType(string rarityLetters)
        {
            switch (rarityLetters)
            {
                case null:
                    return ItemRarity.None;

                case "c":
                    return ItemRarity.Common;

                case "uc":
                    return ItemRarity.UnCommon;

                case "r":
                    return ItemRarity.Rare;

                case "vr":
                    return ItemRarity.Epic;

                case "sr":
                    return ItemRarity.Legendary;

                case "uv":
                    return ItemRarity.Mythic;

                default:
                    throw new Exception("undefined rarity type :" + rarityLetters);
            }
        }

        public static bool IsFourStacks(this IMissionX m, string specificItemName, WorldName mapName = WorldName.Twine_Peaks)
        {
            var count = m.Items.FirstOrDefault(f => f.AlertReward = false && f.quantity == 4 && f.GetRealItemName().Value == specificItemName);
            if (count == null)
            {
                var grp = m.Items.GroupBy(p => p.AlertReward).FirstOrDefault(p => p.Key == false)?.Count(x => x.GetRealItemName().Value == specificItemName);
                if (grp.HasValue && grp == 4)
                {
                    return (m.WorldName == mapName);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return (m.WorldName == mapName);
            }
        }

        public static bool IsGroupMission(this IMissionX m, WorldName worldName)
        {
            return m.IsGroupMission2() && m.WorldName == worldName;
        }

        public static bool IsGroupMission2(this IMissionX m)
        {
            return m.IsGroupMission;
        }

        /// <summary>
        /// define image url instead of emojiId
        /// </summary>
        /// <param name="itemRarity"></param>
        /// <param name="itemType"></param>
        /// <param name="getEmoji"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GuessName(ItemRarity itemRarity, string itemType, bool getEmoji)
        {
            if (itemType.IndexOf("ceiling_") > -1)
            {
                return new KeyValuePair<string, string>("523877864253161472", string.Format("({0}) " + "Trap", itemRarity));
            }
            else if (itemType.IndexOf("outlander_") > -1)
            {
                return new KeyValuePair<string, string>("524033606163628068", string.Format("({0}) " + "Outlander", itemRarity));
            }
            else if (itemType.IndexOf("assault_") > -1)
            {
                return new KeyValuePair<string, string>("524033794471231491", string.Format("({0}) " + "Assault", itemRarity));
            }
            else if (itemType.StartsWith("defender"))
            {
                return new KeyValuePair<string, string>("524033181448536105", string.Format("({0}) " + "Defender", itemRarity));
            }
            else if (itemType.IndexOf("melee_spear") > -1)
            {
                return new KeyValuePair<string, string>("524244684197986337", string.Format("({0}) " + "Spear", itemRarity));
            }
            else if (itemType.IndexOf("melee_tool") > -1)
            {
                return new KeyValuePair<string, string>("523960181248491536", string.Format("({0}) " + "Tool", itemRarity));
            }
            else if (itemType.IndexOf("melee_sword") > -1)
            {
                return new KeyValuePair<string, string>("523959775222956043", string.Format("({0}) " + "Sword", itemRarity));
            }
            else if (itemType.IndexOf("commando_") > -1)
            {
                return new KeyValuePair<string, string>("524033606163628068", string.Format("({0}) " + "Hero", itemRarity));
            }
            else if (itemType.IndexOf("_hammer_") > -1)
            {
                return new KeyValuePair<string, string>("523960181248491536", string.Format("({0}) " + "Tool", itemRarity));
            }
            else if (itemType.IndexOf("melee_club") > -1)
            {
                return new KeyValuePair<string, string>("523960181248491536", string.Format("({0}) " + "Club", itemRarity));
            }
            else if (itemType.IndexOf("pistol_") > -1)
            {
                return new KeyValuePair<string, string>("523872350207213588", string.Format("({0}) " + "Pistol", itemRarity));
            }
            else if (itemType.IndexOf("_axe_") > -1)
            {
                return new KeyValuePair<string, string>("523960181248491536", string.Format("({0}) " + "Axe", itemRarity));
            }
            else if (itemType.StartsWith("floor"))
            {
                return new KeyValuePair<string, string>("523877864253161472", string.Format("({0}) " + "Trap", itemRarity));
            }
            else if (itemType.IndexOf("shotgun_") > -1)
            {
                return new KeyValuePair<string, string>("523872675509043213", string.Format("({0}) " + "Shotgun", itemRarity));
            }
            else if (itemType.IndexOf("wall_") > -1)
            {
                return new KeyValuePair<string, string>("523877864253161472", string.Format("({0}) " + "Trap", itemRarity));
            }
            else if (itemType.IndexOf("ninja_") > -1)
            {
                return new KeyValuePair<string, string>("524033606163628068", string.Format("({0}) " + "Ninja", itemRarity));
            }
            else if (itemType.IndexOf("constructor_") > -1)
            {
                return new KeyValuePair<string, string>("524033606163628068", string.Format("({0}) " + "Constructor", itemRarity));
            }
            else if (itemType.IndexOf("-") > 1)//mythic leader
            {
                return new KeyValuePair<string, string>("534172710578683914", string.Format("({0}) " + "Leader", itemRarity));
            }
            else if (itemType.StartsWith("manager"))//leader survivor
            {
                return new KeyValuePair<string, string>("534172710578683914", string.Format("({0}) " + "L. Survivor", itemRarity));
            }
            //Console.WriteLine($"Undefined mapping is detected (for [Rarity:{mi.GetRarityType()}]'{itemTypeName}' and define in 'ResourceTable')");//last state
            return new KeyValuePair<string, string>("524246128925999114", string.Format("({0}) " + "?", itemRarity));
        }

        public static string ShortName(string currentLongName)
        {
            if (string.IsNullOrWhiteSpace(currentLongName))
            {
                return currentLongName;
            }

            int max = 14;
            int dotLength = 2;
            if (currentLongName.Length < max)
            {
                return currentLongName;
            }

            Regex r = new Regex("^\\((\\w+)\\) (\\w+((\\s\\w+)+)?)$", RegexOptions.Singleline);//(Legendary) Survivor
            var m = r.Match(currentLongName);
            if (m.Success)
            {
                int nlen = max - m.Groups[1].Value.Length - 3;
                var nname = $"({m.Groups[1].Value}) {m.Groups[2].Value.Substring(0, nlen - dotLength)}..";
                return nname;
            }
            else
            {
                r = new Regex("^(\\w+(\\s?\\w+)+)$", RegexOptions.Singleline);//Lightning In A Bottle
                m = r.Match(currentLongName);
                if (m.Success)
                {
                    var nname = $"{m.Groups[1].Value.Substring(0, max - dotLength)}..";

                    return nname;
                }
                else
                {
                    r = new Regex("^(\\w+)\\s(\\w+(-)\\w+)$", RegexOptions.Singleline);//Legendary Perk-Up
                    m = r.Match(currentLongName);
                    if (m.Success)
                    {
                        var nname = $"{m.Groups[2].Value}";

                        return nname;
                    }
                    else
                    {
                        return currentLongName;//undefined case detected
                    }
                }
            }
        }

        public static List<DailyLlama> GetDailyLlamas(Catalog catalogMapping)
        {
            var catalogList = Catalog.GetStorefrontType(catalogMapping, f =>
                f.name == CatalogType.CardPackStorePreroll.ToString() ||
                f.name == CatalogType.CardPackStoreGameplay.ToString());
            List<CatalogEntry> centry = new List<CatalogEntry>();

            foreach (var category in catalogList)
            {
                var cg = category.catalogEntries.Where(g =>
                  g.devName == "Always.UpgradePack.01" ||
                  g.catalogGroup == "Shared" ||
                  g.devName.StartsWith("Event."));
                centry.AddRange(cg);
            }
            var l = centry.Select(f => new DailyLlama
            {
                Amount = (short)f.dailyLimit,
                Price = (short)(f.prices.First().currencyType == "GameItem" ? f.prices.First().regularPrice : f.prices.First().finalPrice),
                DevName = f.devName,
                Title = f.title,
                Description = f.description,
                GameItem = f.prices.First().currencyType == "GameItem"

            })
                   .Distinct(new DailyLlamaComparer())
                   .ToList();

            List<DailyLlama> lst = new List<DailyLlama>();
            lst.AddRange(l);
            return lst;
        }
    }
}