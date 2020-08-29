using Fortnite.Model.Enums;
using Fortnite.Model.Responses.WorldInfo;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fortnite.Static
{
    public static class MissionsStaticData
    {
        public static void LoadStatics()
        {
            ItemRegexTable = new Dictionary<string, Regex>()
            {
                {"Schematic",new Regex("(Schematic:sid_)(\\w+)_(c|uc|r|vr|sr|uv)_ore_(t\\d+)", RegexOptions.Singleline)},
                {"Schematic2",new Regex("(Schematic:sid_)(\\w+)_(c|uc|r|vr|sr|uv)_(t\\d+)", RegexOptions.Singleline)},
                {"CardPack",new Regex("(CardPack:zcp_)(\\w+)_(t\\d+)?((verylow|low|medium|veryhigh|high)|(c|uc|r|vr|sr|uv))?", RegexOptions.Singleline)},
                {"AccountResource",new Regex("(AccountResource:)(\\w+)(_t\\d+)?", RegexOptions.Singleline)},
                {"ConversionControl",new Regex("(ConversionControl:cck_)(\\w+)_consumable_(c|uc|r|vr|sr|uv)", RegexOptions.Singleline)},
                {"Worker",new Regex("(Worker:)(\\w+)_(c|uc|r|vr|sr|uv)_(t\\d+)", RegexOptions.Singleline)},
                {"Worker2",new Regex("(Worker:)(manager\\w+)_(c|uc|r|vr|sr|uv)_(\\w+)_(t\\d+)", RegexOptions.Singleline)},//mythic leader
                {"Defender",new Regex("(Defender:did_)(\\w+)_(c|uc|r|vr|sr|uv)_(t\\d+)", RegexOptions.Singleline)},
                {"Hero",new Regex("(Hero:hid_)(\\w+)_(c|uc|r|vr|sr|uv)_(t\\d+)", RegexOptions.Singleline)},
                {"Currency",new Regex("(Currency:)(\\w+)", RegexOptions.Singleline)},
                {"GameplayModifier",new Regex("(GameplayModifier:)(gm_enemy|gm_basehusk|minibossenableprimarymissionitem|elementalzonefireenableitem|elementalzonenatureenableitem|elementalzonewaterenableitem)(_\\w+)?", RegexOptions.Singleline)},
                {"GameplayModifier2",new Regex("(GameplayModifier:)(gm_hero|gm_player|gm_soldier|gm_ninja_|gm_constructor|gm_outlander|gm_trap)(\\w+)", RegexOptions.Singleline)},
            };
        }

        public static Dictionary<string, Regex> ItemRegexTable;

        public static string GetName(this WorldName mname)
        {
            return mname.ToString().Replace("_", " ");
        }

        public static WorldName ParseMapName(this WWorldName mapname)
        {
            var mname = (WorldName)Enum.Parse(typeof(WorldName), mapname.en.Replace(" ", "_"));
            return mname;
        }

        public static string ToStringHMS(this DateTimeOffset datetimeoffset)
        {
            string time = "";
            var fraction = datetimeoffset - DateTimeOffset.UtcNow;
            if ((int)fraction.TotalHours > 0)
            {
                time = $"{(int)fraction.TotalHours}h {fraction.Minutes }m";
            }
            else if (fraction.Minutes > 0)
            {
                time = $"{fraction.Minutes}m";
            }
            else if (fraction.Seconds > 0)
            {
                time = $"{fraction.Seconds}s";
            }
            return time;
        }
    }
}