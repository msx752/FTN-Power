using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class WWorldDisplayName : WWorldName
    { }

    public class WWorldDescription : WWorldName
    { }

    public class WWorldName
    {
        public string de { get; set; }
        public string ru { get; set; }
        public string ko { get; set; }

        [JsonProperty(PropertyName = "zh-hant")]
        public string zh_hant { get; set; }

        public string pt_br { get; set; }
        public string en { get; set; }
        public string it { get; set; }
        public string fr { get; set; }

        [JsonProperty(PropertyName = "zh-cn")]
        public string zh_cn { get; set; }

        public string es { get; set; }
        public string ar { get; set; }
        public string ja { get; set; }
        public string pl { get; set; }

        [JsonProperty(PropertyName = "es-419")]
        public string es_419 { get; set; }

        public string tr { get; set; }
    }

    public class World
    {
        public WWorldName displayName { get; set; }
        public List<Tile> tiles { get; set; }
        public List<Region> regions { get; set; }
        public string uniqueId { get; set; }
    }
}