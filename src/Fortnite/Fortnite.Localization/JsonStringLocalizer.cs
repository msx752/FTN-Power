using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Fortnite.Localization
{
    public interface IJsonStringLocalizer
    {
        string GetBotTranslation(string key, CultureInfo culture, params object[] arguments);

        string GetBotTranslation(BotTranslationString key, GuildLanguage guildCulture, params object[] arguments);

        string GetBotTranslation(string key, string guildCulture, params object[] arguments);
        string GetBotTranslation(string key, GuildLanguage guildCulture, params object[] arguments);

        string GetAssetRegistryTranslation(string key, CultureInfo culture, params object[] arguments);

        string GetAssetRegistryTranslation(string key, string guildCulture, params object[] arguments);

        string GetAssetRegistryTranslation(string key, GuildLanguage guildCulture, params object[] arguments);

        bool AddToBot(string key, CultureInfo culture, string value);

        bool AddToAssetRegistry(string key, CultureInfo culture, string value);

        bool IsContainInAssetRegistry(string key);

        bool IsContainInBot(string key);

        bool SaveAssetRegistry();

        bool SaveBot();

        FileInfo AssetRegistryPath { get; }
        List<JsonLocalization> locale_Bot { get; }
        List<JsonLocalization> locale_AssetRegistry { get; }
        string Locale_BotFileName { get; }
        string Locale_AssetRegistryFileName { get; }
    }

    public class JsonStringLocalizer : IJsonStringLocalizer
    {
        public List<JsonLocalization> locale_AssetRegistry { get; } = new List<JsonLocalization>();
        public List<JsonLocalization> locale_Bot { get; } = new List<JsonLocalization>();

        public string Locale_BotFileName
        {
            get
            {
                return "locale-Bot";
            }
        }

        public string Locale_AssetRegistryFileName
        {
            get
            {
                return "locale-AssetRegistry";
            }
        }

        [JsonIgnore]
        [NotMapped]
        public FileInfo AssetRegistryPath
        {
            get
            {

                return new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Locale_AssetRegistryFileName}.json"));

            }
        }

        [JsonIgnore]
        [NotMapped]
        public FileInfo BotPath
        {
            get
            {


                return new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{Locale_BotFileName}.json"));

            }
        }

        public JsonStringLocalizer()
        {
            var serializer = new JsonSerializer();
            var strAssetRegistry = File.ReadAllText(AssetRegistryPath.FullName);
            var strBot = File.ReadAllText(BotPath.FullName);
            locale_AssetRegistry = JsonConvert.DeserializeObject<List<JsonLocalization>>(strAssetRegistry);
            locale_Bot = JsonConvert.DeserializeObject<List<JsonLocalization>>(strBot);
            Global.Log.Information("Localization: Files are loaded");
        }

        public bool IsContainInAssetRegistry(string key)
        {
            return locale_AssetRegistry.FirstOrDefault(f => f.IsKeyEqual(key)) != null;
        }

        public bool IsContainInBot(string key)
        {
            return locale_Bot.FirstOrDefault(f => f.IsKeyEqual(key)) != null;
        }

        public bool AddToBot(string key, CultureInfo culture, string value)
        {
            var selected = locale_Bot.FirstOrDefault(f => f.IsKeyEqual(key));
            if (selected != null)
            {
                if (!selected.LocaleContains(culture))
                {
                    selected.Locales.Add(culture.TwoLetterISOLanguageName.ToLower(), value);
                    //locale_Bot.Add(selected);
                }
                else
                {
                    selected.Locales[culture.TwoLetterISOLanguageName.ToLower()] = value;
                }
            }
            else
            {
                JsonLocalization njl = new JsonLocalization();
                njl.Key = key;
                njl.Locales.Add(culture.TwoLetterISOLanguageName.ToLower(), value);
                locale_Bot.Add(njl);
            }
            return true;
        }

        public bool AddToAssetRegistry(string key, CultureInfo culture, string value)
        {
            var selected = locale_AssetRegistry.FirstOrDefault(f => f.IsKeyEqual(key));
            if (selected != null)
            {
                if (!selected.LocaleContains(culture))
                {
                    selected.Locales.Add(culture.TwoLetterISOLanguageName.ToLower(), value);
                    //locale_AssetRegistry.Add(selected);
                }
                else
                {
                    selected.Locales[culture.TwoLetterISOLanguageName.ToLower()] = value;
                }
            }
            else
            {
                JsonLocalization njl = new JsonLocalization();
                njl.Key = key;
                njl.Locales.Add(culture.TwoLetterISOLanguageName.ToLower(), value);
                locale_AssetRegistry.Add(njl);
            }
            return true;
        }

        public string GetAssetRegistryTranslation(string key, string guildCulture, params object[] arguments)
        {
            return GetAssetRegistryString(key, new CultureInfo(guildCulture.ToLower()), arguments);
        }

        public string GetAssetRegistryTranslation(string key, CultureInfo culture, params object[] arguments)
        {
            return GetAssetRegistryString(key, culture, arguments);
        }

        public string GetBotTranslation(BotTranslationString key, GuildLanguage guildCulture, params object[] arguments)
        {
            return GetBotString(key.ToString(), new CultureInfo(guildCulture.ToString().ToLower()), arguments);
        }

        public string GetBotTranslation(string key, GuildLanguage guildCulture, params object[] arguments)
        {
            return GetBotString(key, new CultureInfo(guildCulture.ToString().ToLower()), arguments);
        }
        public string GetBotTranslation(string key, string guildCulture, params object[] arguments)
        {
            return GetBotString(key, new CultureInfo(guildCulture.ToLower()), arguments);
        }
        public string GetBotTranslation(string key, CultureInfo culture, params object[] arguments)
        {
            return GetBotString(key, culture, arguments);
        }

        private string GetString(int LocalizeType, string name, CultureInfo cultureInfo, params object[] arguments)
        {
            JsonLocalization selectedKey = null;
            switch (LocalizeType)
            {
                case 0:
                    {
                        selectedKey = locale_Bot.FirstOrDefault(f => f.IsKeyEqual(name));
                        break;
                    }

                case 1:
                    {
                        selectedKey = locale_AssetRegistry.FirstOrDefault(f => f.IsKeyEqual(name));
                        break;
                    }
            }

            if (selectedKey == null)
            {
                return "";
                throw new Exception($"undefined translation key:'{name}', Culture:'{cultureInfo}'");
            }
            string value = "";
            if (!selectedKey.LocaleContains(cultureInfo))
            {
                value = selectedKey.DefaultLocale;//default en
            }
            else
            {
                value = selectedKey.GetLocaleValue(cultureInfo);
            }
            if (arguments.Any())
            {
                value = string.Format(value, arguments);
            }
            return value;
        }

        private string GetBotString(string name, CultureInfo cultureInfo, params object[] arguments)
        {
            return GetString(0, name, cultureInfo, arguments);
        }

        private string GetAssetRegistryString(string name, CultureInfo cultureInfo, params object[] arguments)
        {
            var str = GetString(1, name, cultureInfo, arguments);
            str = str.Replace("\\", "").Replace("*", "").TrimStart(' ').TrimEnd(' ');
            return str;
        }

        public bool SaveAssetRegistry()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(AssetRegistryPath.FullName, false, Encoding.UTF8))
                {
                    string json = JsonConvert.SerializeObject(locale_AssetRegistry, Formatting.Indented);
                    sw.Write(json);
                    Console.WriteLine($"AssetRegistry is saved to '{AssetRegistryPath.FullName}'");
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
#pragma warning disable CS0162 // Unreachable code detected
            return false;
#pragma warning restore CS0162 // Unreachable code detected
        }

        public bool SaveBot()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(BotPath.FullName, false, Encoding.UTF8))
                {
                    string json = JsonConvert.SerializeObject(locale_Bot, Formatting.Indented);
                    sw.Write(json);
                    Console.WriteLine($"AssetRegistry is saved to '{BotPath.FullName}'");
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
#pragma warning disable CS0162 // Unreachable code detected
            return false;
#pragma warning restore CS0162 // Unreachable code detected
        }

        public string GetAssetRegistryTranslation(string key, GuildLanguage guildCulture, params object[] arguments)
        {
            return GetAssetRegistryTranslation(key, guildCulture.ToString(), arguments);
        }
    }
}