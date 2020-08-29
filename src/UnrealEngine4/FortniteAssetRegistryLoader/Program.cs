using Fortnite.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FortniteAssetRegistryLoader
{
    class Program
    {
        static string[] selectIdFromAssetRegistry = new string[]
        {
           // "AthenaBattleStar",
           // "AthenaSeasonalXP",
            "Campaign_Event_Currency",
           // "CompendiumXP",
            "EventCurrency_Blockbuster",
            "EventCurrency_Candy",
            "EventCurrency_Founders",
           "EventCurrency_PumpkinWarhead",
            "EventCurrency_RoadTrip",
            "EventCurrency_Scaling",
            "EventCurrency_Scavenger",
            "EventCurrency_Snowballs",
            "EventCurrency_Spring",
            "EventCurrency_StormZone",
            "HeroXP",
            "PeopleResource",
            "PersonnelXP",
            "Reagent_Alteration_Ele_Fire",
            "Reagent_Alteration_Ele_Nature",
            "Reagent_Alteration_Ele_Water",
            "Reagent_Alteration_Generic",
            "Reagent_Alteration_Upgrade_R",
            "Reagent_Alteration_Upgrade_SR",
            "Reagent_Alteration_Upgrade_UC",
            "Reagent_Alteration_Upgrade_VR",
            "Reagent_C_T01",
            "Reagent_C_T02",
            "Reagent_C_T03",
            "Reagent_C_T04",
            "Reagent_EvolveRarity_R",
            "Reagent_EvolveRarity_SR",
            "Reagent_EvolveRarity_VR",
            "Reagent_People",
            "Reagent_Traps",
            "Reagent_Weapons",
            "SchematicXP",
            "SpecialCurrency_Daily",
            "Voucher_BasicPack",
            "ZCP_Improvised_R",
            "ZCP_Improvised_VR",
            "Improvised_VR"
        };

        static void Main(string[] args)
        {
            Global.Log.Initialize("FortniteAssetRegistryLoader", false);
            Global.Log.Information("test");
            var services = new ServiceCollection()
                 //.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>()
                 .AddSingleton<IJsonStringLocalizer, JsonStringLocalizer>();
            //.AddLocalization(options => options.ResourcesPath = "Resources");
            var serviceProvider = services.BuildServiceProvider();

            IJsonStringLocalizer localizerFromFilesBot = serviceProvider.GetRequiredService<IJsonStringLocalizer>();

            /* var rr = CultureInfo.GetCultureInfo("en");
             var rr2 = CultureInfo.GetCultureInfo("pt");*/
            //string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "locale-AssetRegistry.json");
            if (!localizerFromFilesBot.AssetRegistryPath.Exists)
            {
                Console.WriteLine("**INVALID PATH** for saving 'locale-AssetRegistry.json' file.");
                Console.ReadLine();
                return;
            }
            else
            {
                Console.WriteLine("path for saving 'locale-AssetRegistry.json' file is **VALID**.");
            }
            Console.WriteLine("\nplease select your work:\n\n" +
                //"1. LoadAssetRegistryBin file for new ids\n\n" +
                "2. MergeAssetRegistry (merge with 'locale-AssetRegistry.json')\n" +
                "3. ExportAssetRegistry Locales\n\n" +
                "4. MergeBot (merge with 'locale-Bot.json')\n" +
                "5. ExportBot Locales\n\n" +
                "6. Export Translation Files\n" +
                "\n0. EXIT");
            var result = int.TryParse(Console.ReadKey().KeyChar.ToString(), out int key);
            Console.WriteLine();
            if (result)
            {
                switch (key)
                {
                    case 1:
                        //LoadAssetRegistryBin(localizerFromFilesBot);
                        break;

                    case 2:
                        MergeAssetRegistry(localizerFromFilesBot);
                        break;


                    case 3:
                        ExportAssetRegistry(localizerFromFilesBot);
                        break;

                    case 4:
                        MergeBot(localizerFromFilesBot);
                        break;


                    case 5:
                        ExportBot(localizerFromFilesBot);
                        break;

                    case 6:
                        GenerateTranslation(localizerFromFilesBot);
                        break;

                    default:
                        Console.WriteLine($"undefined term:{key}");
                        break;
                }
            }
            else
            {

                Console.WriteLine($"WRONG IMPUT");
            }
            Console.ReadLine();
        }
        private static void GenerateTranslation(IJsonStringLocalizer localizerFromFilesBot)
        {
            Console.WriteLine("please specify a language code(i.e. 'en'or 'tr'):");
            string langCode = Console.ReadLine();
            Console.WriteLine();
            if (langCode.Length <= 3 && !int.TryParse(langCode, out int result))
            {
                CultureInfo cultureTo = new CultureInfo(langCode);
                List<TranslateJsonLocalization> AssetRegistryTranslations = new List<TranslateJsonLocalization>();
                foreach (var item in selectIdFromAssetRegistry)
                {
                    var select = localizerFromFilesBot.locale_AssetRegistry.FirstOrDefault(f => f.IsKeyEqual(item));
                    if (select != null)
                    {
                        if (!select.LocaleContains(cultureTo))
                        {
                            AssetRegistryTranslations.Add(select.ToTranslation(cultureTo));
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine($"value of '{item}' key is already translated.");
                            Console.WriteLine($"do you want to translate it again ? [y / n] (default: n)");
                            string answer = Console.ReadKey().Key.ToString().ToLowerInvariant();
                            if (answer == "y")
                            {
                                AssetRegistryTranslations.Add(select.ToTranslation(cultureTo));
                            }
                            else
                            {
                                Console.WriteLine();
                                Console.WriteLine($"value of '{item}' key is skipped.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"'{item}' key not found in 'locale_AssetRegistry'");
                    }
                }
                List<TranslateJsonLocalization> BotTranslations = new List<TranslateJsonLocalization>();
                foreach (var item in localizerFromFilesBot.locale_Bot)
                {
                    if (!item.LocaleContains(cultureTo))
                    {
                        BotTranslations.Add(item.ToTranslation(cultureTo));
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine($"value of '{item}' key is already translated.");
                        Console.WriteLine($"do you want to translate it again ? [y / n] (default: n)");
                        string answer = Console.ReadKey().Key.ToString().ToLowerInvariant();
                        if (answer == "y")
                        {
                            BotTranslations.Add(item.ToTranslation(cultureTo));
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine($"value of '{item}' key is skipped.");
                        }
                    }
                }
                string ExePath = AppDomain.CurrentDomain.BaseDirectory;
                string translationDir = Path.Combine(ExePath, "newTranslations");
                if (!Directory.Exists(translationDir))
                {
                    Directory.CreateDirectory(translationDir);
                }
                var f1 = $"{localizerFromFilesBot.Locale_AssetRegistryFileName}.{cultureTo.TwoLetterISOLanguageName}.txt";
                using (StreamWriter sw = new StreamWriter(Path.Combine(translationDir, f1), false))
                {
                    string assets = JsonConvert.SerializeObject(AssetRegistryTranslations, Formatting.Indented);
                    sw.Write(assets);
                }

                var f2 = $"{localizerFromFilesBot.Locale_BotFileName}.{cultureTo.TwoLetterISOLanguageName}.txt";
                using (StreamWriter sw = new StreamWriter(Path.Combine(translationDir, f2), false))
                {
                    string bots = JsonConvert.SerializeObject(BotTranslations, Formatting.Indented);
                    sw.Write(bots);
                }
                Console.WriteLine();
                Console.WriteLine($"{f1}\nand\n{f2}\n are successfully exported to '{translationDir}'");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("wrong format for 'GenerateTranslation'");
                return;
            }
        }
        private static void ExportBot(IJsonStringLocalizer localizerFromFilesBot)
        {
            Console.WriteLine($"insert the path of export file 'export-{localizerFromFilesBot.Locale_BotFileName}.json':");
            string path = Console.ReadLine();
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"undefined path for 'export-{localizerFromFilesBot.Locale_BotFileName}.json.");
                return;
            }
            string jsonExport = JsonConvert.SerializeObject(localizerFromFilesBot.locale_Bot, Formatting.Indented);
            var exportFile = Path.Combine(path, $"export-{localizerFromFilesBot.Locale_BotFileName}.json");
            using (StreamWriter sw = new StreamWriter(exportFile, false, Encoding.UTF8))
            {
                sw.Write(jsonExport);
            }
            Console.WriteLine($"datas are successfully expored({localizerFromFilesBot.locale_Bot.Count}) to:\n{exportFile}");
        }

        private static void MergeBot(IJsonStringLocalizer localizerFromFilesBot)
        {
            Console.WriteLine($"insert the folder of new-translated '{localizerFromFilesBot.Locale_BotFileName}.json' file:");
            string path = Console.ReadLine();
            string[] files = Directory.GetFiles(path, $"{localizerFromFilesBot.Locale_BotFileName}*");
            if (path == "" || files.Length == 0)
            {
                Console.WriteLine($"undefined new-translated '{localizerFromFilesBot.Locale_BotFileName}.json' file or location");
                return;
            }

            Console.WriteLine("please select a translated file index: ");
            Console.WriteLine();
            for (int i = 0; i < files.Length; i++)
            {
                var fl = new FileInfo(files[i]);
                Console.WriteLine($"[{i}]\t'{fl.Name}'");
            }
            Console.WriteLine();
            string indexNum = Console.ReadLine();
            Console.WriteLine();
            if (!int.TryParse(indexNum, out int findex))
            {
                Console.WriteLine($"undefined index was selected.");
                return;
            }
            else if (findex >= files.Length && findex < 0)
            {
                Console.WriteLine($"undefined index was selected.");
                return;
            }
            /*  else
              {
                  Console.WriteLine("file is selected.");
              }*/
            var fif = new FileInfo(files[findex]);
            var cultureKey = fif.Name.Split(".");
            List<JsonLocalization> newLocalizations = new List<JsonLocalization>();
            using (StreamReader sr = new StreamReader(fif.FullName))
            {
                var jsnl = JsonConvert.DeserializeObject<List<TranslateJsonLocalization>>(sr.ReadToEnd())
                    .Select(f => f.ToJsonLocalization(new CultureInfo(cultureKey[1]))).ToList();
                newLocalizations = jsnl;
                Console.WriteLine($"new file is loaded({newLocalizations.Count}).");
            }
            int countKey = 0;
            int countLocale = 0;
            int oldCount = localizerFromFilesBot.locale_Bot.Count;
            foreach (var item in newLocalizations)
            {
                foreach (var locale in item.Locales)
                {
                    if (locale.Value == "PUT_HERE")
                    {
                        continue;
                    }
                    localizerFromFilesBot.AddToBot(item.Key, new CultureInfo(locale.Key), locale.Value);
                    countLocale++;
                }
                countKey++;
            }
            Console.WriteLine($"{countKey} count keys(before:{oldCount}) and {countLocale} count locales are revised.");
            localizerFromFilesBot.SaveBot();
        }

        private static void ExportAssetRegistry(IJsonStringLocalizer localizerFromFilesBot)
        {
            Console.WriteLine($"insert the path of export file 'export-{localizerFromFilesBot.Locale_AssetRegistryFileName}.json':");
            string path = Console.ReadLine();
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"undefined path for 'export-{localizerFromFilesBot.Locale_AssetRegistryFileName}.json.");
                return;
            }
            string jsonExport = JsonConvert.SerializeObject(localizerFromFilesBot.locale_AssetRegistry, Formatting.Indented);
            var exportFile = Path.Combine(path, $"export-{localizerFromFilesBot.Locale_AssetRegistryFileName}.json");
            using (StreamWriter sw = new StreamWriter(exportFile, false, Encoding.UTF8))
            {
                sw.Write(jsonExport);
            }
            Console.WriteLine($"datas are successfully expored({localizerFromFilesBot.locale_AssetRegistry.Count}) to:\n{exportFile}");

        }

        private static void MergeAssetRegistry(IJsonStringLocalizer localizerFromFilesBot)
        {
            Console.WriteLine($"insert the folder of new 'new-{localizerFromFilesBot.Locale_AssetRegistryFileName}.json' file:");
            string path = Console.ReadLine();
            string[] files = Directory.GetFiles(path, $"{localizerFromFilesBot.Locale_AssetRegistryFileName}*");
            if (path == "" || files.Length == 0)
            {
                Console.WriteLine($"undefined new-translated '{localizerFromFilesBot.Locale_AssetRegistryFileName}.json' file or location");
                return;
            }

            Console.WriteLine("please select a translated file index: ");
            Console.WriteLine();
            for (int i = 0; i < files.Length; i++)
            {
                var fl = new FileInfo(files[i]);
                Console.WriteLine($"[{i}]\t'{fl.Name}'");
            }
            Console.WriteLine();
            string indexNum = Console.ReadLine();
            Console.WriteLine();
            if (!int.TryParse(indexNum, out int findex))
            {
                Console.WriteLine($"undefined index was selected.");
                return;
            }
            else if (findex >= files.Length && findex < 0)
            {
                Console.WriteLine($"undefined index was selected.");
                return;
            }
            /*  else
              {
                  Console.WriteLine("file is selected.");
              }*/
            var fif = new FileInfo(files[findex]);
            var cultureKey = fif.Name.Split(".");
            List<JsonLocalization> newLocalizations = new List<JsonLocalization>();
            using (StreamReader sr = new StreamReader(fif.FullName))
            {
                var jsnl = JsonConvert.DeserializeObject<List<TranslateJsonLocalization>>(sr.ReadToEnd())
                    .Select(f => f.ToJsonLocalization(new CultureInfo(cultureKey[1]))).ToList();
                newLocalizations = jsnl;
                Console.WriteLine($"new file is loaded({newLocalizations.Count}).");
            }
            int countKey = 0;
            int countLocale = 0;
            int oldCount = localizerFromFilesBot.locale_AssetRegistry.Count;
            foreach (var item in newLocalizations)
            {
                foreach (var locale in item.Locales)
                {
                    if (locale.Value == "PUT_HERE")
                    {
                        continue;
                    }
                    localizerFromFilesBot.AddToAssetRegistry(item.Key, new CultureInfo(locale.Key), locale.Value);
                    string findDerivative = item.Key;
                    if (findDerivative == "EventCurrency_Scaling")
                    {
                        findDerivative = "EventScaling";
                    }
                    var derivatives = localizerFromFilesBot.locale_AssetRegistry
                        .Where(f => f.Key.Substring(3).StartsWith($"_{findDerivative}", StringComparison.InvariantCultureIgnoreCase)).ToList();
                    foreach (var derivativeKey in derivatives)
                    {
                        localizerFromFilesBot.AddToAssetRegistry(derivativeKey.Key, new CultureInfo(locale.Key), locale.Value);
                    }
                    countLocale++;
                    countLocale += derivatives.Count;
                }
                countKey++;
            }
            Console.WriteLine($"{countKey} count keys(before:{oldCount}) and {countLocale} count locales are revised.");
            localizerFromFilesBot.SaveAssetRegistry();
        }

    }


}
