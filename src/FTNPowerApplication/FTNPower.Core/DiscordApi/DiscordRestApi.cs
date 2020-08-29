using Discord;
using Discord.Rest;
using FTNPower.Core;
using FTNPower.Data.Migrations;
using FTNPower.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Core.DiscordApi
{
    public class DiscordRestApi : IDiscordRestApi
    {
        private DiscordRestClient api;

        public DiscordRestClient GetApi
        {
            get
            {
                try
                {
                    return api;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private void SetApi(DiscordRestClient value)
        {
            api = value;
        }
        public DiscordRestApi(bool isDeveloperMode, string token)
        {
            SetApi(new DiscordRestClient(new DiscordRestConfig()
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                LogLevel = isDeveloperMode ? LogSeverity.Debug : LogSeverity.Warning,
             
            }));
            GetApi.LoggedIn += RestApi_LoggedIn;
            GetApi.LoggedOut += RestApi_LoggedOut;
            GetApi.LoginAsync(TokenType.Bot, token).Wait();
        }

        private Task RestApi_LoggedOut()
        {
            return Task.Run(() =>
            {
                Global.Log.Information("Discord Rest Api: Logged Out!");
            });
        }

        private Task RestApi_LoggedIn()
        {
            return Task.Run(() =>
            {
                Global.Log.Information("Discord Rest Api: Successfully Logged In");
            });
        }

        public Task<RestGuildUser> GetGuildUserAsync(ulong guildId, ulong userId)
        {
            return Task.Run(async () =>
            {
                var guser = await GetApi.GetGuildUserAsync(guildId, userId, Utils.RequestOption);
                return guser;
            });
        }

        public Task<RestUser> GetUserByAspNetIdAsync(string aspNetId)
        {
            return Task.Run(async () =>
            {
                var userId = await GetUserIdByExternalLoginAsync(aspNetId);
                var user = await GetApi.GetUserAsync(userId, Utils.RequestOption);
                if (user == null)
                {
                    return null;
                }
                return user;
            });
        }
        public Task<RestGuildUser> GetGuildUserByAspNetIdAsync(string aspNetId, ulong guildId)
        {
            return Task.Run(async () =>
            {
                var userId = await GetUserIdByExternalLoginAsync(aspNetId);
                var user = await GetApi.GetGuildUserAsync(guildId, userId, Utils.RequestOption);
                if (user == null)
                {
                    return null;
                }
                return user;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aspNetId"></param>
        /// <param name="loginProvider"></param>
        /// <returns></returns>
        public Task<ulong> GetUserIdByExternalLoginAsync(string aspNetId, string loginProvider = "Discord")
        {
            return Task.Run(() =>
           {
               using (BotContext db = new BotContext())
               {
                   var selectedLogin = db.UserLogins.FirstOrDefault(f => f.UserId == aspNetId && f.LoginProvider == loginProvider);// use cache for increasing the db performance
                    if (selectedLogin == null)
                   {
                       return (ulong)0;
                   }
                   return ulong.Parse(selectedLogin.ProviderKey);
               }
           });
        }

        public Task<bool> DeleteTextMessage(ulong channelId, ulong messageId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var channel = (IRestMessageChannel)await GetApi.GetChannelAsync(channelId);
                    await channel?.DeleteMessageAsync(messageId);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            });

        }

    }
}
