using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Model.Enums;
using Fortnite.Static;
using Microsoft.Extensions.DependencyInjection;
using Global;
using System;
using System.Collections.Generic;
using Fortnite.Localization;

namespace Fortnite.Static.Models.MissionAlerts
{
    public class MissionItemX : IMissionItemX
    {
        public MissionItemX()
        {
            Rarity = ItemRarity.None;
            AssetType = AssetType.Undefined;
            AssetId = "?";
        }

        private string itemType = null;
        private string assetId;

        public string ItemType
        {
            get
            {
                return itemType;
            }
            set
            {
                itemType = value;
                this.DefineAsset();
                if (itemType.Contains("worker:manager", StringComparison.InvariantCultureIgnoreCase) &&
                    Rarity == ItemRarity.Legendary &&
                    itemType.Contains("_sr_", StringComparison.InvariantCultureIgnoreCase))
                {
                    Rarity = ItemRarity.Mythic;
                }
            }
        }

        public KeyValuePair<string, string> RealName
        {
            get
            {
                if (itemType == null)
                {
                    return new KeyValuePair<string, string>(null, null);
                }

                return GetRealItemName();
            }
        }

        public int quantity { get; set; }

        /// <summary>
        /// if the tierGroupName is 'MissionSelect' then false
        /// </summary>
        public bool AlertReward { get; set; }

        public string AssetId
        {
            get
            {
                return assetId;
            }
            set
            {
                assetId = value;
            }
        }

        public AssetType AssetType { get; set; }
        public ItemRarity Rarity { get; set; }

        public ItemClass Class { get; set; }
        public KeyValuePair<string, string> GetRealItemName(bool shortName = false, string lang = "EN", bool getEmoji = true)
        {
            IMissionItemX mi = this;
            var Translate = DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
            var name = Translate.GetAssetRegistryTranslation(mi.ItemType.Split(':')[1].Replace("sid_", "wid_"), lang)
                .Replace("Legendary PERK", "PERK")
            .Replace("Uncommon PERK", "PERK");
            if (name == "")
            {
                name = Translate.GetBotTranslation(mi.ItemType.Split(':')[1].Replace("sid_", "wid_"), lang);
                if (name == "")
                {
                    mi.AssetType = AssetType.Registry;
                    name = Translate.GetAssetRegistryTranslation(mi.ItemType.Split(':')[1].Replace("zcp_", "").Replace("cck_", ""), lang);
                }
                else
                {
                    mi.AssetType = AssetType.Bot;
                }
            }
            else
            {
                mi.AssetType = AssetType.Registry;
            }
            if (mi.Rarity != ItemRarity.None)
            {
                if (SurvivorStaticData.AssetBot2.ContainsKey(mi.AssetId))
                {
                    var storedAsset = SurvivorStaticData.AssetBot2[mi.AssetId];

                    var displayName = string.Format(storedAsset.StoredName, mi.Rarity);
                    if (name != "")
                    {
                        displayName = name;
                    }
                    else
                    {
                        mi.AssetType = AssetType.Bot;
                    }
                    if (shortName)
                    {
                        displayName = Core.Utils.ShortName(displayName);
                    }
                    return new KeyValuePair<string, string>(storedAsset.EmojiId, displayName);
                }
                else
                {
                    var easyName = Core.Utils.GuessName(mi.Rarity, mi.AssetId, getEmoji);

                    if (name != "")
                    {
                        easyName = new KeyValuePair<string, string>(easyName.Key, name);
                    }
                    else
                    {
                        mi.AssetType = AssetType.Guess;
                    }
                    if (shortName)
                    {
                        easyName = new KeyValuePair<string, string>(easyName.Key, Core.Utils.ShortName(name == "" ? easyName.Value : name));
                    }
                    if (mi.Rarity == ItemRarity.Mythic)
                    {
                        easyName = new KeyValuePair<string, string>(easyName.Key, $"(Mythic) {easyName.Value}");
                    }
                    return easyName;
                }
            }
            else
            {
                if (SurvivorStaticData.AssetBot2.ContainsKey(mi.AssetId))
                {
                    var storedAsset = SurvivorStaticData.AssetBot2[mi.AssetId];
                    var stName = storedAsset.StoredName;
                    if (name != "")
                    {
                        stName = name;
                    }
                    else
                    {
                        mi.AssetType = AssetType.Bot;
                    }

                    if (shortName)
                    {
                        stName = Core.Utils.ShortName(stName);
                    }
                    return new KeyValuePair<string, string>(getEmoji ? storedAsset.EmojiId : storedAsset.Url, stName);
                }
                else
                {
                    //Console.WriteLine($"Undefined mapping is detected (for '{itemTypeName}' and define in 'ResourceTable')");
                    mi.AssetType = AssetType.Undefined;
                    return new KeyValuePair<string, string>("524246128925999114", name == "" ? "?" : name);
                }
            }
        }

    }
}