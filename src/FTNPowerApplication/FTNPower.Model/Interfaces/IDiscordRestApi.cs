using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Model.Interfaces
{
    public interface IDiscordRestApi
    {

        DiscordRestClient GetApi { get; }
        Task<RestGuildUser> GetGuildUserAsync(ulong guildId, ulong userId);
        Task<RestGuildUser> GetGuildUserByAspNetIdAsync(string aspNetId, ulong guildId);
        Task<RestUser> GetUserByAspNetIdAsync(string aspNetId);
        Task<ulong> GetUserIdByExternalLoginAsync(string aspNetId, string loginProvider = "Discord");
        Task<bool> DeleteTextMessage(ulong channelId, ulong messageId);
    }
}
