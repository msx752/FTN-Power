using FTNPower.Model;
using FTNPower.Model.WebsiteModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace FTNPower.Model.Tables
{
    public class GuildConfig
    {
        private EventConf @event;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        public string Id { get; set; }
        public GuildConfig()
        {
            Owner = new OwnerConf();
            Admin = new AdminConf();
            Event = new EventConf();
            Other = new OtherConf();
        }
        public OwnerConf Owner { get; set; }
        public AdminConf Admin { get; set; }
        public EventConf Event
        {
            get
            {
                if (@event == null)
                    @event = new EventConf();
                return @event;
            }
            set
            {
                @event = value;
            }
        }
        public OtherConf Other { get; set; }
        [NotMapped]
        [JsonIgnore]
        public ulong GUid { get { return Id.ToUlong(); } }
    }
}
