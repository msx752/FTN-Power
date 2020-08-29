using Newtonsoft.Json;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class Description
    {
        public string de { get; set; }
        public string ru { get; set; }
        public string Ko { get; set; }

        [JsonProperty("zh-hant")]
        public string zh_hant { get; set; }

        [JsonProperty("pt-br")]
        public string pt_br { get; set; }

        public string en { get; set; }
        public string it { get; set; }
        public string fr { get; set; }

        [JsonProperty("zh-cn")]
        public string zh_cn { get; set; }

        public string es { get; set; }
        public string ar { get; set; }
        public string ja { get; set; }
        public string pl { get; set; }

        [JsonProperty("es-419")]
        public string es_419 { get; set; }

        public string tr { get; set; }
        public string zh { get; set; }
    }
}