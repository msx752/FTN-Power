using Discord;
using Discord.WebSocket;
using Fortnite.Localization;
using FTNPower.Core.ApplicationService;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Tables;
using System;
using System.Collections.Generic;

namespace FTNPower.Core.DomainService.SubRepositories
{
    public class GuildRepo
    {
        private readonly IRedisService _redis;
        private readonly IUnitOfWork _uow;
        public GuildRepo(IUnitOfWork uow, IRedisService redisService)
        {
            _uow = uow;
            _redis = redisService;
        }

        public KeyValuePair<DiscordServer, GuildConfig> AddOrGetGuild(string guildId, GuildLanguage lang = GuildLanguage.EN)
        {
            var svr = _uow.Db<DiscordServer>().GetById(guildId);
            if (svr == null)
            {
                svr = new DiscordServer { Id = guildId, Language = lang };
                _uow.Db<DiscordServer>().Add(svr);
                _uow.Commit();
            }
            var gConfig = AddOrGetGuildConfig(guildId);
            return new KeyValuePair<DiscordServer, GuildConfig>(svr, gConfig);
        }

        public GuildConfig AddOrGetGuildConfig(string guildId)
        {
            GuildConfig gConfig = GetConfig(guildId);
            if (gConfig == null)
            {
                gConfig = new GuildConfig { Id = guildId };
                _uow.Db<GuildConfig>().Add(gConfig);
                _uow.Commit();
                _redis.JsonDelete(_redis.Key<GuildConfig>(gConfig.Id));
            }
            return gConfig;
        }
        public GuildLanguage Language(string guildId)
        {
            var gconfig = GetConfig(guildId);
            return gconfig.Owner.DefaultLanguage;
        }

        public bool Enable(SocketGuild guild, GuildLanguage lang)
        {
            if (guild == null || !guild.CurrentUser.GuildPermissions.ManageRoles || !guild.CurrentUser.GuildPermissions.ManageNicknames)
                return false;

            var reorder2 = new List<ReorderRoleProperties>();
            {
                var discordServer = _uow.Db<DiscordServer>().GetById(guild.Id.ToString());
                GuildConfig gConfig = null;

                if (discordServer == null)
                    gConfig = AddOrGetGuild(guild.Id.ToString(), lang).Value;
                else
                    gConfig = AddOrGetGuildConfig(guild.Id.ToString());

                var list = guild.GetMissingMapRolesAsync(gConfig.Owner.DefaultLanguage, out reorder2).Result;

                if (list == null || list.Count < 4)
                    return false;

                try
                {
                    foreach (var crole in list)
                    {
                        crole.Value.SetMapRolePermissionsAsync();
                    }
                }
                catch (AggregateException e)
                {
                    try
                    {
                        guild.Owner.GetOrCreateDMChannelAsync(Core.Utils.RequestOption)
                            .ContinueWith((p1) =>
                            {
                                p1.Result.SendMessageAsync(e.Message)
                                    .ContinueWith((p2) =>
                                    {
                                        p1.Result.CloseAsync(Core.Utils.RequestOption).Wait();
                                    });
                            });
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception)
                {
                }
                try
                {
                    if (reorder2.Count >= 4)
                        guild.ReorderRolesAsync(reorder2, Core.Utils.RequestOption).Wait();
                }
                catch (Exception)
                {
                };
            }
            return true;
        }


        public GuildConfig GetConfig(string guildId)
        {
            GuildConfig cachedConfig = _redis.JsonGet<GuildConfig>(_redis.Key<GuildConfig>(guildId));
            if (cachedConfig == null)
            {
                cachedConfig = _uow.Db<GuildConfig>().GetById(guildId);
                _redis.JsonSet(_redis.Key<GuildConfig>(guildId), cachedConfig, expiry: new TimeSpan(1, 0, 0));
            }
            return cachedConfig;
        }
    }
}