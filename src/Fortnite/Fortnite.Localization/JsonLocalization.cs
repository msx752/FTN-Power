using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Fortnite.Localization
{
    public class JsonLocalization
    {
        public JsonLocalization(string key)
        {
            Key = key;
        }

        public JsonLocalization()
        {
        }

        public string Key { get; set; }
        public Dictionary<string, string> Locales { get; set; } = new Dictionary<string, string>();

        [JsonIgnore]
        [NotMapped]
        public string DefaultLocale
        {
            get
            {
                try
                {
                    return Locales[new CultureInfo("en").TwoLetterISOLanguageName.ToLower()];
                }

                catch (Exception e)

                {
                    throw new Exception($"undefined default language for '{Key}' key");
                }
            }
        }

        public string GetLocaleValue(CultureInfo cultureInfo)
        {
            bool bl = Locales.TryGetValue(cultureInfo.TwoLetterISOLanguageName.ToLower(), out string value);
            return bl == true ? value : null;
        }

        public bool LocaleContains(CultureInfo cultureInfo)
        {
            return GetLocaleValue(cultureInfo) != null;
        }

        public bool IsKeyEqual(string key)
        {
            return string.Equals(Key, key, StringComparison.CurrentCultureIgnoreCase);
        }

        public override string ToString()
        {
            return Key;
        }

        public TranslateJsonLocalization ToTranslation(CultureInfo toCulture)
        {
            var defaultTranslation = "PUT_HERE";
            if (LocaleContains(toCulture))
            {
                defaultTranslation = GetLocaleValue(toCulture);
            }
            var toTranslate = new TranslateJsonLocalization(Key, DefaultLocale, defaultTranslation);
            return toTranslate;
        }
    }
}