using Fortnite.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fortnite.Model.Responses.QueryProfile
{
    public static class Extension
    {
        public static SurvivorRarity GetSurvivorRarity(byte slotId, string name, string type)
        {
            if (slotId == 0 && name.IndexOf("-") > 1)
            {
                return SurvivorRarity.Mythic;
            }
            else if (slotId == 0 && type == "c")
            {
                return SurvivorRarity.UnCommon;
            }
            else if (slotId == 0 && type == "r")
            {
                return SurvivorRarity.Epic;
            }
            else if (slotId == 0 && type == "vr")
            {
                return SurvivorRarity.Legendary;
            }
            else if (slotId == 0 && type == "sr")
            {
                return SurvivorRarity.Mythic;
            }
            else if (type == "uc")
            {
                return SurvivorRarity.UnCommon;
            }
            else if (type == "c")
            {
                return SurvivorRarity.Common;
            }
            else if (type == "r")
            {
                return SurvivorRarity.Rare;
            }
            else if (type == "vr")
            {
                return SurvivorRarity.Epic;
            }
            else if (type == "sr")
            {
                return SurvivorRarity.Legendary;
            }
            else if (type == "ur")
            {
                return SurvivorRarity.Mythic;
            }
            return SurvivorRarity.None;
        }

        public static string GetItemType(this string templateId)
        {
            var split = templateId
                .Split('_')
                .First(f => f.Length == 2 | f.Length == 1);
            return split;
        }

        public static byte GetItemTier(this string templateId)
        {
            var splt = byte.Parse(templateId.Split(new string[] { "_t0" }, StringSplitOptions.None)[1]);
            return splt;
        }

        public static Task<string> GetItemName(string templateId)
        {
            return Task.Run(async () =>
            {
                var survivorType = templateId.GetItemType();

                var indexType = templateId.IndexOf($"_{survivorType}_");
                var name = templateId.Substring(0, indexType)
                    .TrimStart("Worker:".ToCharArray());

                int tier = templateId.GetItemTier();
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

        public static string GetZoneName(this Item item)
        {
            if (!item.templateId.StartsWith("Quest:outpostquest_"))
            {
                return null;
            }

            var nm = item.templateId.Split('_')[1].TrimStart('t');
            switch (nm)
            {
                case "1":
                    return "stonewood";

                case "2":
                    return "plankerton";

                case "3":
                    return "cannyvalley";

                case "4":
                    return "twinepeaks";

                default:
                    return null;
            }
        }

        public static short GetSSDLevel(this Item item)
        {
            if (!item.templateId.StartsWith("Quest:outpostquest_"))
            {
                return -1;
            }

            var lvl = item.templateId.Split('_')[2].TrimStart('l');
            return short.Parse(lvl);
        }

        public static Dictionary<string, IMissionCategory> ParseAlerts(this ClaimDataMap claimDataMap, params Alert[] alertNames)
        {
            Dictionary<string, IMissionCategory> dict = new Dictionary<string, IMissionCategory>();
            var prop = claimDataMap.GetType().GetProperties();
            foreach (var prp in prop)//"NewMissionIntroductionCategory"
            {
                var alertName = prp.Name
                   .Replace("NewMissionIntroductionCategory", "x1")
                    .Replace("Category", "Alert")
                    .Replace("ElemementalZone", "")
                    .Replace("AlertAlert", "Alert")
                    .Replace("x1", "NewMissionIntroductionCategory");

                var val = prp.GetValue(claimDataMap);
                if (val != null)
                {
                    var valMissionAlertGuids = val.GetType().GetProperty("missionAlertGuids");
                    var listGuids = (List<string>)valMissionAlertGuids.GetValue(val);
                    if (listGuids[0] != "")
                    {
                        dict.Add(alertName, (IMissionCategory)val);
                    }
                    else if (alertName == "NewMissionIntroductionCategory")
                    {
                        dict.Add(alertName, (IMissionCategory)val);
                    }
                }
                else
                {
                }
            }
            return dict;
        }
    }
}