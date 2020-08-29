using FModel.Methods;
using FModel.Methods.AESManager;
using FModel.Methods.Assets;
using FModel.Methods.PAKs;
using FModel.Methods.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FortnitePakManager
{
    public class AssetDat
    {
        private string id;
        private string displayName;

        public string Description { get; set; }
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                var temp = value;
                if (temp == null)
                    temp = "";
                temp = temp.Replace(" Token", "");
                displayName = temp;
            }
        }
        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                var temp = value;
                if (temp == null)
                    temp = "";
                if (temp.StartsWith("Voucher_Generic_Worker_"))
                {
                    temp = temp.Replace("Voucher_Generic_Worker_", "workerbasic_", StringComparison.InvariantCultureIgnoreCase) + "_t01";
                    ClassType = "Survivor";
                }
                else if (temp.StartsWith("Voucher_Generic_Schematic_"))
                {
                    temp = temp.Replace("Voucher_Generic_Schematic_", "Schematic_", StringComparison.InvariantCultureIgnoreCase);
                    ClassType = "Schematic";
                }

                if (temp.StartsWith("wid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Weapon";
                    temp = temp.Replace("wid_", "id_", StringComparison.InvariantCultureIgnoreCase);
                }
                else if (temp.StartsWith("tid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Trap";
                    temp = temp.Replace("tid_", "id_", StringComparison.InvariantCultureIgnoreCase);
                }
                else if (temp.StartsWith("ZCP_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Reagent";
                    temp = temp.Replace("ZCP_", "", StringComparison.InvariantCultureIgnoreCase);
                }
                else if (temp.StartsWith("hid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Hero";
                }
                else if (temp.StartsWith("did_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Defender";
                }
                else if (temp.StartsWith("worker_", StringComparison.InvariantCultureIgnoreCase) ||
                    temp.StartsWith("manager", StringComparison.InvariantCultureIgnoreCase) ||
                    temp.StartsWith("WorkerHalloween_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Survivor";
                }
                else if (temp.StartsWith("eid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Emote";
                }
                else if (temp.StartsWith("glider_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Glider";
                }
                else if (temp.StartsWith("wrap_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Wrap";
                }
                else if (temp.StartsWith("bid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Backpak";
                }
                else if (temp.StartsWith("kit_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Kit";
                }
                else if (temp.StartsWith("gm_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "GameModifier";
                }
                else if (temp.StartsWith("cid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Cosmetic";
                }
                else if (temp.StartsWith("pickaxe_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Pickaxe";
                }
                else if (temp.StartsWith("trails_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Trail";
                }
                else if (temp.StartsWith("Reagent_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Reagent";
                }
                else if (temp.StartsWith("eventcurrency_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "EventCurrency";
                }
                else if (temp.StartsWith("Voucher_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Voucher";
                }
                else if (temp.StartsWith("cck_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Consumable";
                }
                else if (temp.StartsWith("spid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Spray";
                }
                else if (temp.StartsWith("tt_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "TT";
                }
                else if (temp.StartsWith("aid_", StringComparison.InvariantCultureIgnoreCase))
                {
                    ClassType = "Alteration";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(ClassType))
                        ClassType = "";
                }
                id = temp;
            }
        }
        public string ClassType { get; set; }
        public string ImageName { get; set; }
        public string HashedId { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string PNGName
        {
            get
            {
                if (this.ImageName == null)
                {
                    return null;
                }
                else
                {
                    var index = this.ImageName.LastIndexOf("/") + 1;
                    var iname = this.ImageName.Substring(index);
                    return iname;
                }
            }
        }

        public string Rarity { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class SpecialExport2
    {
        public List<AssetDat> assetData = new List<AssetDat>();
        public List<string> Exported = new List<string>();
        public List<string> Errors = new List<string>();
        public SpecialExport2()
        {
        }

        public static string GetJtokenValue(JArray jArray, string rootName, params string[] IndexNameOfSubElements)
        {
            JToken selectedJtoken = jArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), rootName));
            if (selectedJtoken != null)
            {
                JToken assetPathName = selectedJtoken;
                int index = 0;
                do
                {
                    assetPathName = assetPathName[IndexNameOfSubElements[index]];
                    index++;
                }
                while (index < IndexNameOfSubElements.Length);

                if (selectedJtoken.Equals(assetPathName))
                {
                    throw new Exception($"error on getting {rootName}");
                }

                var displayName = assetPathName.Value<string>().Replace("\"", "'");
                return displayName;
            }
            throw new Exception($"error on getting {rootName}");
        }

        public SpecialExport2 ExportAll()
        {
            SpecialExport(
                "Heroes",
                "Heroes/",
                new string[] { "Commando/ItemDefinition/", "Constructor/ItemDefinition/", "Ninja/ItemDefinition/", "Outlander/ItemDefinition/" }
                );

            SpecialExport(
                "Traps",
                "Items/Traps/",
                new string[] { "Ceiling/", "Floor/", "Wall/" }
                );

            SpecialExport(
                "Weapons",
                "Items/Weapons/",
                new string[] { "Ranged/", "Melee/", }
                );

            SpecialExport(
                "Survivors",
                "Items/Workers/",
                new string[] { "", "Managers/", },
                f => f.IndexOf("/trainers/", StringComparison.InvariantCultureIgnoreCase) > -1 || f.IndexOf("/uniquemanagers/", StringComparison.InvariantCultureIgnoreCase) > -1
                );

            SpecialExport(
                "Alterations_v2",
                "Items/Alteration_v2/",
                new string[] { "AttributeAlterations/", "ConditionalAlterations/", "GameplayAlterations/" },
                f => f.IndexOf("ParentAssets/", StringComparison.InvariantCultureIgnoreCase) > -1,
                x => x.IndexOf("/AID_", StringComparison.InvariantCultureIgnoreCase) > -1
                );

            SpecialExport(
                "CardPacks",
                "Items/CardPacks/",
                new string[] { "", "ZoneRewards/", },
                f => f.IndexOf("test/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("specificrewards/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("genericrewards/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("founders/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("expeditions/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("events/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("choicepacks/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("caches/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("achievements/", StringComparison.InvariantCultureIgnoreCase) > -1
                );

            SpecialExport(
                "Mutations",
                "Items/GameplayModifiers/",
                new string[] { "Mutations/", }
                );

            SpecialExport(
                "Cosmetics",
               "Athena/Items/Cosmetics/",
                new string[] { "Characters/", "Dances/", "Gliders/", "ItemWraps/", "Pickaxes/", "Sprays/", "Contrails/", "Backpacks/", },
                f => f.IndexOf("Emoji/", StringComparison.InvariantCultureIgnoreCase) > -1
                );

            SpecialExport(
                "STW Resources",
                "Items/PersistentResources/",
                new string[] { "", }
                );

            SpecialExport(
                "STW Hero Abilities",
                "Abilities/Player/",
                new string[] { "Constructor/Perks/", "Ninja/Perks/", "Outlander/Perks/", "Commando/Perks/", "Perks/Leader/", "Constructor/Actives/", "Ninja/Actives/", "Outlander/Actives/", "Commando/Actives/", },
                f => f.IndexOf("fx/", StringComparison.InvariantCultureIgnoreCase) > -1 ||
                f.IndexOf("gc/", StringComparison.InvariantCultureIgnoreCase) > -1,
                x => x.IndexOf("/Kit_", StringComparison.InvariantCultureIgnoreCase) > -1
                );

            SpecialExport(
                "Conversion Controls",
                 "Items/DirectedAcquisition/ConversionControl/",
                new string[] { "Defender/Consumable/", "Hero/Consumable/", "Manager/Consumable/", "Melee/Consumable/", "Ranged/Consumable/", "Trap/Consumable/", "Weapon/Consumable/", "Worker/Consumable/", }
                );

            SpecialExport(
                "STW Defenders",
                "Items/Defenders/",
                new string[] { "" }
                );

            Console.WriteLine("ALL DONE!!!!!!");
            return this;
        }

        public SpecialExport2 Init()
        {
            onStart();
            onLoad();
            LoadRecent_CDNIMGIDS();
            LoadRecent_RECENTEXPORTS();
            LoadRecent_ERRORS();
            return this;
        }

        private void DrawLargeSmallImage(string texturePath, string fileName)
        {
            string savePath = Settings.FOutput_Path + @$"\FTNPower\im\{fileName}.png";
            if (File.Exists(savePath))
            {
                return;
            }
            using (Stream image = AssetsUtility.GetStreamImageFromPath(texturePath))
            {
                if (image != null)
                {
                    using (Image img = ImagesUtility.GetImageSource(image))
                    {
                        img.Save(savePath);
                    }
                }
            }
        }

        private AssetDat GenerateAssetDat(JArray propertiesArray, string nameId)
        {
            AssetDat dat = new AssetDat();
            dat.Id = nameId;
            if (nameId.StartsWith("Kit_"))//stw heroe abilities
            {
                dat.DisplayName = GetJtokenValue(propertiesArray, "DisplayName", "tag_data", "source_string");
                JToken descriptions = propertiesArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "Description"));
                JToken desctDatas = null;
                if (descriptions != null)
                {
                    desctDatas = descriptions["tag_data"]["data"];
                }

                if (descriptions == null || (descriptions != null && desctDatas.Count() == 0) || (descriptions != null && desctDatas.All(f => f["source_string"].Value<string>() == "")))
                {
                    var tooltip = propertiesArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "Tooltip"));

                    if (tooltip == null ||
                        (tooltip != null && tooltip["tag_data"] == null) ||
                        (tooltip != null && tooltip["tag_data"] != null && tooltip["tag_data"]["import"].Value<string>() == ""))
                    {
                        dat.Description = dat.DisplayName;
                    }
                    else
                    {
                        var ttData = tooltip["tag_data"]["import"].Value<string>().TrimEnd('C').TrimEnd('_');
                        var allQuery = AssetsUtility.GetPakReaderQuery(ttData);
                        /*
                         * there is an error to get this data * 
                            TT_Ninja_PointyFury,
                            TT_Commando_Blitz,
                            TT_Perk_L_SlowYourRoll_T01,
                         */
                        foreach (var key in allQuery)
                        {
                            var jsonData = AssetsUtility.GetAssetJsonDataByPath(key);
                            if (jsonData != null && AssetsUtility.IsValidJson(jsonData))
                            {
                                JToken AssetMainToken2 = AssetsUtility.ConvertJson2Token(jsonData);
                                if (AssetMainToken2 == null)
                                    continue;
                                JToken desc_tt = null;
                                do
                                {
                                    try
                                    {
                                        desc_tt = AssetMainToken2["properties"].FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "Description"));
                                        if (desc_tt != null) break;
                                    }
                                    catch (Exception e)
                                    {
                                        //do not trace error just search description
                                    }

                                    AssetMainToken2 = AssetMainToken2.Next;
                                } while (AssetMainToken2 != null);
                                if (desc_tt == null)
                                {
                                    throw new Exception($"desc_tt is null");
                                }

                                dat.Description = desc_tt["tag_data"]["source_string"].Value<string>().Replace("\"", "'");
                                break;
                            }
                            else
                            {
                                throw new Exception($"{ttData} is not found");
                            }
                        }
                    }
                }
                else
                {
                    dat.Description = desctDatas.First["source_string"].Value<string>();
                }
                JToken IconRush = propertiesArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "IconBrush"));
                if (string.IsNullOrWhiteSpace(IconRush["tag_data"]["struct_type"]["properties"].FirstOrDefault(c => string.Equals(c["name"].Value<string>(), "ResourceObject"))["tag_data"]["outer_import"].Value<string>()))
                {
                    throw new Exception("Icon doesnt find for: stw heroe abilities");
                }
                else
                {
                    var picture = IconRush["tag_data"]["struct_type"]["properties"].FirstOrDefault(c => string.Equals(c["name"].Value<string>(), "ResourceObject"))["tag_data"]["outer_import"].Value<string>();
                    var pictureUrl = FoldersUtility.FixFortnitePath(picture);
                    dat.ImageName = pictureUrl;
                }
            }
            else
            {
                if (nameId.IndexOf("_Defender") == -1)
                {
                    dat.DisplayName = GetJtokenValue(propertiesArray, "DisplayName", "tag_data", "source_string");
                }
                else
                {
                    dat.DisplayName = "Defender";
                }
                dat.Description = GetJtokenValue(propertiesArray, "Description", "tag_data", "source_string");
                dat.ImageName = GetItemImage(propertiesArray);
                try
                {
                    dat.Rarity = GetJtokenValue(propertiesArray, "Rarity", "tag_data").Replace("EFortRarity::", "");
                }
                catch (Exception)
                {
                    dat.Rarity = null;
                }
            }
            return dat;
        }

        private void GenerateClass(string path, string[] Dirs, Func<string, bool> ignorePaths = null, Func<string, bool> selectByFilter = null)
        {
            for (int i = 0; i < Dirs.Length; i++)
            {
                string selectedPath = path + Dirs[i];
                var allQuery = AssetsUtility.GetPakReaderQuery(selectedPath);
                if (allQuery.Count == 0)
                {
                    throw new Exception("there is no any data with this query");
                }
                if (ignorePaths != null)
                    allQuery = allQuery.Except(allQuery.Where(ignorePaths).ToList()).ToList();
                if (selectByFilter != null)
                    allQuery = allQuery.Where(selectByFilter).ToList();
                Generatejson(allQuery);
            }
        }

        private void Generatejson(List<string> allQuery)
        {
            allQuery = allQuery.Except(Exported).Except(Errors).ToList();
            foreach (var key in allQuery)
            {
                try
                {
                    var name = Path.GetFileName(key);

                    var jsonData = AssetsUtility.GetAssetJsonDataByPath(key);
                    if (jsonData != null && AssetsUtility.IsValidJson(jsonData))
                    {
                        JToken AssetMainToken = AssetsUtility.ConvertJson2Token(jsonData);
                        if (AssetMainToken == null)
                            continue;
                        JArray AssetProperties = AssetMainToken["properties"].Value<JArray>();
                        var dat = GenerateAssetDat(AssetProperties, name);
                        DrawLargeSmallImage(dat.ImageName, dat.Id);
                        dat.HashedId = Guid.NewGuid().ToString("N");
                        assetData.Add(dat);
                        Exported.Add(key);
                    }
                }
                catch (Exception e)
                {
                    Errors.Add(key);
                    Console.WriteLine($"Assets: ERROR Gathering info {key}");
                    continue;
                }
            }
        }

        private string GetItemImage(JArray propertiesArray)
        {
            JToken displayAssetPath = propertiesArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "DisplayAssetPath"));
            JToken largePreviewImage = propertiesArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "LargePreviewImage"));
            JToken smallPreviewImage = propertiesArray.FirstOrDefault(x => string.Equals(x["name"].Value<string>(), "SmallPreviewImage"));
            if (largePreviewImage != null || smallPreviewImage != null || displayAssetPath != null)
            {
                JToken assetPathName =
                    displayAssetPath != null ? displayAssetPath["tag_data"]["struct_type"]["asset_path_name"] :
                    largePreviewImage != null ? largePreviewImage["tag_data"]["asset_path_name"] :
                    smallPreviewImage != null ? smallPreviewImage["tag_data"]["asset_path_name"] : throw new Exception("error on writing picture");

                return FoldersUtility.FixFortnitePath(assetPathName.Value<string>()); ;
            }
            throw new Exception("error on writing picture");
        }

        private void LoadRecent_CDNIMGIDS()
        {
            var dir0 = Path.Combine(Settings.FOutput_Path, "FTNPower", "_CDNIMGIDS.json");
            FileInfo fi0 = new FileInfo(dir0);
            if (fi0.Exists)
            {
                using (StreamReader sr0 = new StreamReader(fi0.FullName))
                {
                    var jsn0 = sr0.ReadToEnd();
                    assetData = JsonConvert.DeserializeObject<List<AssetDat>>(jsn0).Distinct().ToList();
                }
            }
            else
            {
                assetData = new List<AssetDat>();
            }
        }

        private void LoadRecent_RECENTEXPORTS()
        {
            var dir0 = Path.Combine(Settings.FOutput_Path, "FTNPower", "_RECENTEXPORTS.json");
            FileInfo fi0 = new FileInfo(dir0);
            if (fi0.Exists)
            {
                using (StreamReader sr0 = new StreamReader(fi0.FullName))
                {
                    var jsn0 = sr0.ReadToEnd();
                    Exported = JsonConvert.DeserializeObject<List<string>>(jsn0).Distinct().ToList();
                }
            }
            else
            {
                Exported = new List<string>();
            }
        }
        private void LoadRecent_ERRORS()
        {
            var dir0 = Path.Combine(Settings.FOutput_Path, "FTNPower", "_ERRORS.json");
            FileInfo fi0 = new FileInfo(dir0);
            if (fi0.Exists)
            {
                using (StreamReader sr0 = new StreamReader(fi0.FullName))
                {
                    var jsn0 = sr0.ReadToEnd();
                    Errors = JsonConvert.DeserializeObject<List<string>>(jsn0).Distinct().ToList();
                }
            }
            else
            {
                Errors = new List<string>();
            }
        }

        private void onLoad()
        {
            PAKsLoader.LoadAllPAKs().Wait();
            PAKsLoader.LoadDifference(false).Wait();
        }

        private void onStart()
        {
            PAKEntries.PAKEntriesList = new List<PAKInfosEntry>();
            PAKEntries.PAKToDisplay = new Dictionary<string, PakReader.FPakEntry[]>();
            AssetEntries.ArraySearcher = new Dictionary<string, PakReader.FPakEntry[]>();
            AssetEntries.AssetEntriesDict = new Dictionary<string, PakReader.PakReader>();
            RegisterFromPath.CheckFortniteVersion();
            RegisterFromPath.FilterPAKs();
            DynamicKeysChecker.SetDynamicKeys();
            FoldersUtility.LoadFolders();
            var t = Task.Run(() =>
            {
                AssetTranslations.SetAssetTranslation("English");
            });

            t.Wait();
            var v1 = AssetTranslations.BRLocResDict;
            var v2 = AssetTranslations.HotfixLocResDict;
            var v3 = AssetTranslations.STWLocResDict;
        }

        private void SpecialExport(string commandName, string searchPath, string[] ClassesTypeDir, Func<string, bool> ignorePaths = null, Func<string, bool> selectByFilter = null)
        {
            GenerateClass(searchPath, ClassesTypeDir, ignorePaths, selectByFilter);
            WriteAllAssetDatsToFile();
            WriteAllExportsToFile();
            WriteAllErrorsToFile();
            Console.WriteLine($"{commandName} are Exported");
        }

        private void WriteAllAssetDatsToFile()
        {
            try
            {
                var jsonFile = new FileInfo(Path.Combine(Settings.FOutput_Path, "FTNPower", "_CDNIMGIDS.json"));
                using (StreamWriter sw = new StreamWriter(jsonFile.FullName, false))
                {
                    var jsonData = JsonConvert.SerializeObject(assetData.Distinct(), Formatting.Indented);
                    sw.Write(jsonData);
                    Console.WriteLine($"ALL DONE: _CDNIMGIDS.json is updated");
                };
            }
            catch (Exception)
            {
                throw new Exception("error on writing _CDNIMGIDS");
            }
        }

        private void WriteAllExportsToFile()
        {
            try
            {
                var jsonFile = new FileInfo(Path.Combine(Settings.FOutput_Path, "FTNPower", "_RECENTEXPORTS.json"));
                using (StreamWriter sw = new StreamWriter(jsonFile.FullName, false))
                {
                    var jsonData = JsonConvert.SerializeObject(Exported.Distinct());
                    sw.Write(jsonData);
                    Console.WriteLine($"ALL DONE: _RECENTEXPORTS.json is updated");
                };
            }
            catch (Exception)
            {
                throw new Exception("error on writing _RECENTEXPORTS");
            }
        }
        private void WriteAllErrorsToFile()
        {
            try
            {
                var jsonFile = new FileInfo(Path.Combine(Settings.FOutput_Path, "FTNPower", "_ERRORS.json"));
                using (StreamWriter sw = new StreamWriter(jsonFile.FullName, false))
                {
                    var jsonData = JsonConvert.SerializeObject(Errors.Distinct());
                    sw.Write(jsonData);
                    Console.WriteLine($"ALL DONE: _ERRORS.json is updated");
                };
            }
            catch (Exception)
            {
                throw new Exception("error on writing _ERRORS");
            }
        }
    }
}