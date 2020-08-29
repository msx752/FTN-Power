using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Model.WebsiteModels
{
    public class ReadyToVerify
    {
        public ReadyToVerify()
        {
            Expire = DateTimeOffset.UtcNow.AddHours(-1);
        }
        public ReadyToVerify(string epicUsername, string EpicId, string discordUserId) : this()
        {
            this.EpicId = EpicId;
            DiscordUserId = discordUserId;
            EpicUsername = epicUsername;
            Expire = DateTimeOffset.UtcNow.AddMinutes(2);
            JustDeQueue = false;
        }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public string EpicId { get; set; }
        public string DiscordUserId { get; set; }
        public DateTimeOffset Expire { get; set; }
        public string EpicUsername { get; set; }
        public bool JustDeQueue { get; set; }
        public override string ToString()
        {
            return $"{DiscordUserId}|{EpicId}";
        }
    }
}
