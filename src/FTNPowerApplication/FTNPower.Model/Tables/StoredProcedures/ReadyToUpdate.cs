using FTNPower.Model;
using FTNPower.Model.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables.StoredProcedures
{
    [Serializable]
    public class ReadyToUpdate
    {
        public ReadyToUpdate()
        {

        }
        public string DiscordServerId { get; set; } = "";
        public string FortniteUserId { get; set; } = "";
        public string EpicId { get; set; } = "";

        public GameUserMode GameUserMode { get; set; }


        public bool NameTag { get; set; }
        public bool PVEDecimals { get; set; }
        public override string ToString()
        {
            return $"{DiscordServerId},{FortniteUserId}";
        }

        [NotMapped]
        [JsonIgnore]
        public ulong Uid { get { return FortniteUserId.ToUlong(); } }

        [NotMapped]
        [JsonIgnore]
        public ulong GUid { get { return DiscordServerId.ToUlong(); } }
    }
}