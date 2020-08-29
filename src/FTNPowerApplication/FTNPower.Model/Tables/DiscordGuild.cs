using Fortnite.Localization;
using FTNPower.Model.Enums;
using FTNPower.Model.WebsiteModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables
{
    [Table("DiscordServers")]
    public class DiscordServer
    {
        private string _restrictedRoleIds = "";

        public DiscordServer()
        {
            NameStates = new HashSet<NameState>();
            LastMassUpdate = DateTimeOffset.UtcNow.AddYears(-1);
        }

        [StringLength(1)]
        public string CustomPrefix { get; set; } = null;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        public string Id { get; set; }

        public bool IsInitialized { get; set; }

        [Required]
        public GuildLanguage Language { get; set; } = GuildLanguage.EN;

        [IncludeQuery]
        public ICollection<NameState> NameStates { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ulong GUid { get { return this.Id.ToUlong(); } }

        public DateTimeOffset LastMassUpdate { get; set; }

        /// <summary>
        /// removes user request after bot responds
        /// </summary>
        public bool AutoRemoveRequest { get; set; }
        public string RestrictedRoleIds
        {
            get
            {
                if (_restrictedRoleIds == null || string.IsNullOrWhiteSpace(_restrictedRoleIds))
                    _restrictedRoleIds = "[]";
                return _restrictedRoleIds;
            }

            set
            {
                if (value == null || string.IsNullOrWhiteSpace(value))
                    _restrictedRoleIds = "[]";
                else
                    _restrictedRoleIds = value;
            }
        }


        public GameUserMode DefaultGameMode { get; set; }
        public bool PVEDecimals { get; set; }
    }
}