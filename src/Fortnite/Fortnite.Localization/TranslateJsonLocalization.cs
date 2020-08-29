using System.Collections.Generic;
using System.Globalization;

namespace Fortnite.Localization
{
    public class TranslateJsonLocalization
    {
        public TranslateJsonLocalization(string key, string translateFrom, string translateTo)
        {
            Id = key;
            Translate_From = translateFrom;
            Translate_To = translateTo;
        }

        public string Id { get; set; }
        public string Translate_From { get; set; }
        public string Translate_To { get; set; }

        public JsonLocalization ToJsonLocalization(CultureInfo languageCode)
        {
            var translate = new JsonLocalization(Id)
            {
                Locales = new Dictionary<string, string>()
                {
                    {languageCode.TwoLetterISOLanguageName,Translate_To }
                }
            };
            return translate;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}