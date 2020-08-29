using Fortnite.Model.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace fortniteLib.Responses.FriendList
{
    public class Friend
    {
        public string accountId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FriendStatus status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FriendDirection direction { get; set; }

        public DateTime created { get; set; }
        public bool favorite { get; set; }
    }
}