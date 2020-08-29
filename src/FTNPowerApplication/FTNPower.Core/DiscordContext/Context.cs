using Discord.Commands;
using Discord.WebSocket;

//using discordbot.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Global;
using System;
using System.Linq;
using System.Threading.Tasks;
using FTNPower.Model;
using FTNPower.Core.DomainService;
using FTNPower.Model.Interfaces;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using FTNPower.Core.Interfaces;

namespace FTNPower.Core.DiscordContext
{
    /// <summary>
    /// The context.
    /// </summary>
    public class Context : ShardedCommandContext, IContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> class.
        /// </summary>
        /// <param name="client">
        /// The client param.
        /// </param>
        /// <param name="message">
        /// The message param.
        /// </param>
        /// <param name="queueManager"></param>
        public Context(DiscordShardedClient client, SocketUserMessage message, IFTNPowerRepository fTNPowerRepository, IDiscordRestApi discordRestApi) : base(client, message)
        {
            DiscordRestApi = discordRestApi;
            Repo = fTNPowerRepository;
            GuildConfig = Repo.Guild.AddOrGetGuildConfig(Guild.Id.ToString());
            DiscordUser = Repo.User.AddOrGetUserAsync(User.Id.ToString(), Guild.Id.ToString()).Result;
            AccountPowerLevel = 1;
            if (DiscordUser.IsValidName)
            {
                if (DiscordUser.GameUserMode == GameUserMode.PVE)
                {
                    var pveuser = Repo.Db<FortnitePVEProfile>()
                                      .Where(f => f.EpicId == DiscordUser.EpicId)
                                      .Select(x => new { x.PlayerName, x.EpicId, x.AccountPowerLevel })
                                      .FirstOrDefault();
                    PlayerName = pveuser?.PlayerName;
                    if (pveuser != null)
                        AccountPowerLevel = Model.Utils.GetIntegerPower(pveuser.AccountPowerLevel);
                    else
                        AccountPowerLevel = 0;
                }
                else
                {
                    var pvpUser = Repo.Db<FortnitePVPProfile>()
                                      .Where(f => f.EpicId == DiscordUser.EpicId)
                                      .FirstOrDefault();
                    PlayerName = pvpUser?.PlayerName;
                    TotalPVPRankedWins = pvpUser.PvpCurrentModeWins(DiscordUser.GameUserMode);
                }
            }
            else
            {
                PlayerName = null;
            }

        }
        public IDiscordRestApi  DiscordRestApi
        {
            get;
        }
        private IRedisService _RedisService = null;
        public IRedisService Redis
        {
            get
            {
                if (_RedisService == null)
                    _RedisService = DIManager.Services.GetRequiredService<IRedisService>();
                return _RedisService;
            }
        }
        public IFTNPowerRepository Repo { get; private set; }
        public GuildConfig GuildConfig { get; private set; }
        public FortniteUser DiscordUser { get; private set; }
        public string PlayerName { get; private set; }
        public int AccountPowerLevel { get; private set; }
        public int TotalPVPRankedWins { get; private set; }




    }
}