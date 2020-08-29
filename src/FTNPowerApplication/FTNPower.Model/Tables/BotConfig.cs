using FTNPower.Model.WebsiteModels;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables
{
    [Table("BotConfigs")]
    public class BotConfig
    {
        private bool developing;

        public BotConfig()
        {
            Variables = new FTNPower.Model.WebsiteModels.CustomVariables();
        }

        public BotConfig(string id, string token) : this()
        {
            Id = id;
            Token = token;
            Developing = false;
            Id = "FortnitePower";
            LogCommandUsages = true;
            LogUserMessages = false;
            Prefix = "f.";
            Shards = 1;
            Token = "TokenValue";

        }

        /// <summary>
        /// Gets or sets a value indicating whether the bot is being developed
        /// </summary>-
        public bool Developing
        {
            get
            {
                return developing;
            }
            set
            {
                developing = value;
            }
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log command usages.
        /// </summary>
        public bool LogCommandUsages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to log user messages.
        /// </summary>
        public bool LogUserMessages { get; set; }

        /// <summary>
        /// Gets or sets the bot prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Gets or sets the amount of shards for the bot
        /// </summary>
        public int Shards { get; set; }

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// custom variables for different discordbots
        /// </summary>
        public FTNPower.Model.WebsiteModels.CustomVariables Variables { get; set; }

        [NotMapped]
        [JsonIgnore]
        public CustomVariables Vars
        {
            get
            {
                return Variables;
            }
        }
    }
}