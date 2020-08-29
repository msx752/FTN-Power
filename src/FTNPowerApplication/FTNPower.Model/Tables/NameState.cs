
using FTNPower.Model;
using FTNPower.Model.WebsiteModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables
{
    public class NameState
    {
        public NameState()
        {
            Priority = byte.MaxValue;
        }

        [IncludeQuery]
        public DiscordServer DiscordServer { get; set; }

        public string DiscordServerId { get; set; }

        [IncludeQuery]
        public FortniteUser FortniteUser { get; set; }

        public string FortniteUserId { get; set; }

        public bool InQueue { get; set; }
        public bool LockName { get; set; }

        /// <summary>
        /// 1-255
        /// </summary>
        public byte Priority { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ulong Uid { get { return FortniteUserId.ToUlong(); } }

        [NotMapped]
        [JsonIgnore]
        public ulong GUid { get { return DiscordServerId.ToUlong(); } }
    }

}