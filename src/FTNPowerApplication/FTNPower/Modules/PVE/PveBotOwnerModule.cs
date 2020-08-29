using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Core.Services;
using Fortnite.Model.Enums;
using fortniteLib.Responses.Catalog;
using Microsoft.Extensions.DependencyInjection;
using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fortnite.External.Api.Interfaces;
using Fortnite.External.Responses.BDailyStore;
using FTNPower.Model.Tables;
using FTNPower.Data;
using FTNPower.Model.Enums;
using Fortnite.Localization;
using FTNPower.Model;
using FTNPower.Data.Tables;
using FTNPower.Core;
using FTNPower.Core.DiscordContext.Preconditions;
using Fortnite.Api;

namespace FTNPower.Modules.PVE
{
    public class PveBotOwnerModule : PveFunctionLayer
    {
        private static object _lockRefreshName = new object();
        public PveBotOwnerModule()
            : base()
        {

        }
        [FTNPowerOwner]
        [Command("sunucu.pve.haritalar.ekle")]
        [Alias("server.pve.maproles.add")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task AddMapRoles()
        {
            return _AddMapRoles();

        }
        [FTNPowerOwner]
        [Command("ignore")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task Answer(int isIgnored)
        {
            return Task.Run(() =>
            {
#if RELEASE
                BotConfig botConfig = Context.Redis.JsonGet<BotConfig>();
                botConfig.Vars.IgnoreRequest = isIgnored==1;
                Context.Redis.JsonSet<BotConfig>(botConfig);
                ReplyEmbedAsync($"ignored:{isIgnored == 1}").Wait();
#endif
            });
        }

        [FTNPowerOwner]
        [Command("answer")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task Answer(string mention, [Remainder] string message)
        {
            return Task.Run(async () =>
            {
                Regex r = new Regex("<@!?(\\d+)>", RegexOptions.Singleline);
                var m = r.Match(mention);
                if (m.Success)
                {
                    var user = Context.DiscordRestApi.GetGuildUserAsync(this.Context.Guild.Id, Context.Message.MentionedUsers.First().Id).Result;
                    var guild = Context.DiscordRestApi.GetApi.GetGuildAsync(465028350067605504).Result;
                    var dm = await user.GetOrCreateDMChannelAsync(Core.Utils.RequestOption);
                    string desc = $"ds[{guild.Id}]**{guild.Name}**\n" +
                                  $"Help Discord:https://discord.gg/C9CgzVg" +
                                  $"\n---\n";
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Author = new EmbedAuthorBuilder()
                        {
                            IconUrl = Context.User.GetAvatarUrl(),
                            Name = $"{Context.User.Username}#{Context.User.Discriminator}"
                        },
                        Color = Color.Green,
                        Title = $"Reply from DEVELOPER",
                        Description = $"{desc}**FROM:** {Context.User.Mention}, **MESSAGE:** {message}\n\n",
                        Footer = new EmbedFooterBuilder()
                        {
                            IconUrl = Context.Guild.CurrentUser.GetAvatarUrl(),
                            Text = $"only you can reply with f.helpme <message> in any discord text-channel"
                        }
                    };
                    try
                    {
                        var msg = await dm.SendMessageAsync(string.Empty, false, embed.Build(), Core.Utils.RequestOption);
                        await dm.CloseAsync(Core.Utils.RequestOption);
                        await ReplyEmbedAsync($"response has sent to User({user.Mention})");
                    }
                    catch (Exception e)
                    {
                        await ReplyEmbedErrorAsync(e.Message);
                    }
                }
            });
        }

        [FTNPowerOwner]
        [Command("server.autoremove")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public Task AutoRemoveRequest(string state)
        {
            return _AutoRemoveRequest(state);
        }

        [FTNPowerOwner]
        [Command("server.restrict.role.clear")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task ClearRestrictionRole()
        {
            return _ClearRestrictionRole();
        }

        [FTNPowerOwner]
        [Alias("server.user.ban")]
        [Command("sunucu.user.ban")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task UserBan(ulong Id)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Banned User";
            var cuser = Context.DiscordRestApi.GetGuildUserAsync(this.Context.Guild.Id, Id).Result;
            Context.Repo.Blacklist.AddUserToBlacklist(Id.ToString());
            embed.Description = $"({Id}){cuser?.Username}#{cuser?.Discriminator} *has been banned...*";
            await ReplyEmbedAsync(embed);
        }

        [FTNPowerOwner]
        [Alias("server.guild.ban")]
        [Command("sunucu.guild.ban")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task GuildBan(ulong Id)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Banned Guild";
            var cuser = Context.DiscordRestApi.GetGuildUserAsync(this.Context.Guild.Id, Id).Result;
            Context.Repo.Blacklist.AddGuildToBlacklist(Id.ToString());
            embed.Description = $"({Id}){cuser?.Username} *has been banned...*";
            await ReplyEmbedAsync(embed);
        }

        [FTNPowerOwner]
        [Alias("server.guild.banlist")]
        [Command("sunucu.guild.banlist")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task GuildBanList()
        {
            var lst = Context.Repo.Db<BlackListGuild>().All().ToList();
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Banned Guild List";
            string banlist = "";
            foreach (var guild in lst)
            {
                ulong guid = ulong.Parse(guild.Id);
                var cguild = Context.DiscordRestApi.GetApi.GetGuildAsync(guid).Result;
                if (cguild != null)
                    banlist += $"id:{guild.Id}, name:{cguild.Name}\n";
                else
                    banlist += $"id:{guild.Id}, name:??\n";
            }
            embed.Description = banlist;
            await ReplyEmbedAsync(embed);
        }

        [FTNPowerOwner]
        [Alias("server.guild.ban.remove")]
        [Command("sunucu.guild.ban.remove")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task GuildBanRemove(string mention)
        {
            if (!mention.HasMentionedId().Key)
            {
                return;
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Banned Guild Removed";
            Context.Repo.Blacklist.RemoveGuildFromBlacklist(mention.HasMentionedId().Value);
            var cguild = Context.DiscordRestApi.GetApi.GetGuildAsync(mention.HasMentionedId().Value.ToUlong()).Result;
            embed.Description = $"({mention.HasMentionedId().Value}){cguild?.Name} *ban has been removed...*";
            await ReplyEmbedAsync(embed);
        }
        [FTNPowerOwner]
        [Command("resetbnick")]
        public Task resetbnick()
        {
            try
            {
                Context.Guild.CurrentUser.ModifyAsync((o) =>
                {
                    o.Nickname = "";
                });
                Context.Message.DeleteAsync().Wait();
            }
            catch (Exception)
            {

            }
            return Task.CompletedTask;
        }
        [FTNPowerOwner]
        [Command("hi")]
        public Task Hi()
        {
            return ReplyEmbedAsync($"Hello master {Context.User.Mention}", null, null);
        }

        [FTNPowerOwner]
        [Command("server.priority.list")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task PriorityList()
        {
            return AllPriorities();
        }

        [FTNPowerOwner]
        [Command("server.priority.update")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task PriorityUpdate(string id = "", string extendFormat = "", [Remainder] string customText = null)
        {
            return Task.Run(async () =>
            {
                PriorityTable priority = Context.Repo.Priority.UpdatePriority(id, extendFormat, customText);
                if (priority != null)
                {
                    IGuildUser usr = null;
                    IGuild gld = null;
                    string namefor = "";
                    if (priority.State == PriorityState.User)
                    {
                        usr = Context.DiscordRestApi.GetGuildUserAsync(this.Context.Guild.Id, priority.GetUlongId()).Result;
                        namefor = $"{usr?.Username}#{usr?.Discriminator}";
                    }
                    else if (priority.State == PriorityState.Guild)
                    {
                        gld = Context.DiscordRestApi.GetApi.GetGuildAsync(priority.GetUlongId()).Result;
                        usr = gld?.GetOwnerAsync().Result;
                        namefor = $"{gld?.Name}";
                    }
                    else
                    {
                        throw new Exception("undefined PriorityState for PriorityManager");
                    }
                    if (priority.CheckValidity())
                    {
                        DateTime dtx = new DateTime();
                        dtx = dtx.Add(priority.Remining);
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Author = new EmbedAuthorBuilder()
                            {
                                IconUrl = Context.Guild.CurrentUser.GetAvatarUrl(),
                                Name = Context.Guild.CurrentUser.Username
                            },
                            Color = Color.Green,
                            Title = "DISCORD PLAN IS UPDATED",
                        };
                        embed.Description += $"Type: **{priority.State}**\n" +
                                             $"Name: {namefor}\n" +
                                             $"Expires In: **{dtx.Year - 1}**Years **{dtx.Month - 1}**Months **{dtx.Day - 1}**days **{dtx.Hour}**hours\n";
                        embed.Description += "\n\n";
                        string text = $"[id:**{priority.Id}**][**{namefor}**].";
                        Log.Information("Priority: subs state is updated via cmd for {CustomerPriorityId} name with {CustomerName}, value {AddedValue} and Text {CustomText}", priority.Id, namefor, extendFormat, customText);
                        try
                        {
                            var dm = await usr.GetOrCreateDMChannelAsync();
                            var msg = await dm.SendMessageAsync(string.Empty, false, embed.Build(), Core.Utils.RequestOption);
                            await dm.CloseAsync(Core.Utils.RequestOption);
                        }

                        catch (Exception e)

                        {
                            await ReplyEmbedAsync($"{text} but DM was close, **COULDN'T** informed.", "DISCORD PLAN IS UPDATED");
                            return;
                        }
                        await ReplyEmbedAsync(text, "DISCORD PLAN IS UPDATED");
                    }
                    else
                    {
                        await ReplyEmbedAsync($"[id:{priority.Id}][{namefor}]. but was **expired**", "DISCORD PLAN IS UPDATED");
                    }
                }
                else
                {
                    await ReplyEmbedErrorAsync("format is not correct i.e.:\n **u193749607107395585** **1M** **1d** **1h** **Text**/**null(off)**(not required)");
                }
            });
        }

        [FTNPowerOwner]
        [Command("test")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task ProfileInfo(string mention)
        {
            return Task.CompletedTask;
        }

        [FTNPowerOwner]
        [Command("server.ingames")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task PvePlayerList()
        {
            return _PveplayerList();
        }

        [FTNPowerOwner]
        [Alias("server.user.update", "sunucu.üye.güncel")]
        [Command("server.user.up")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task RefreshName(string mention)
        {
            return _UpdateName(mention);
        }

        [FTNPowerOwner]
        [Command("server.restrict.role.remove")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task RemoveRestrictionRole([Remainder] string roleName = null)
        {
            return _RemoveRestrictionRole(roleName);
        }

        [FTNPowerOwner]
        [Command("server.resent.mission")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentAlerts()
        {
            return Task.Run(() =>
           {
               var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
               var Missions = queueApi.WebhookMissions();
               FortniteEventHandler.MissionWebhook(Context.Guild.Id, Context.GuildConfig, Missions).Wait();
           });
        }
        [FTNPowerOwner]
        [Command("server.resent.all.daily")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentAllDaily()
        {
            return Task.Run(async () =>
            {
                var partners = Context.Repo.Priority.GetValidGuildPartners();
                int count = 0;
                var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                var dEmbedList = queueApi.DailyLlama().ToList();
                if (dEmbedList.Count > 0)
                {
                    foreach (var partner in partners)
                    {
                        try
                        {
                            var gConfig = Context.Repo.Guild.GetConfig(partner.Id);
                            if (gConfig == null)
                                continue;
                            FortniteEventHandler.DailyLlamaWebhook(partner.GetUlongId(), gConfig, new List<Embed>() { dEmbedList.ToWebhookEmbed() }).Wait();
                            count++;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    await ReplyEmbedAsync($"({dEmbedList.Count()})DailyLlamas are sent to premium discords({count}).");
                }
                else
                {
                    await ReplyEmbedErrorAsync($"there is no DaillyLama right now.");
                }
            });
        }

        [FTNPowerOwner]
        [Command("server.resent.all.mission")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentAllMissions()
        {
            return Task.Run(async () =>
            {
                var partners = Context.Repo.Priority.GetValidGuildPartners();
                int count = 0;
                var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                var Missions = queueApi.WebhookMissions();
                if (Missions.Count() > 0)
                {
                    foreach (var partner in partners)
                    {
                        try
                        {
                            var gConfig = Context.Repo.Guild.GetConfig(partner.Id);
                            if (gConfig == null)
                                continue;
                            FortniteEventHandler.MissionWebhook(partner.GetUlongId(), gConfig, Missions).Wait();
                            count++;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    await ReplyEmbedAsync($"({Missions.Count()})Missions are sent to premium discords({count}).");
                }
                else
                {
                    await ReplyEmbedErrorAsync($"there is no Missions right now.");
                }
            });
        }

        private Task ResentSelectedMissions(string nameOfFilter, Func<IMissionX, bool> predicts)
        {
            return Task.Run(async () =>
            {
                var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                var Missions = queueApi.WebhookMissions();
                var partners = Context.Repo.Priority.GetValidGuildPartners();
                int count = 0;
                var mEmbedList = Missions.Where(predicts);
                if (mEmbedList.Count() > 0)
                {
                    foreach (var partner in partners)
                    {
                        try
                        {

                            string guid = partner.GetUlongId().ToString();
                            var gConfig = Context.Repo.Guild.GetConfig(partner.Id);
                            FortniteEventHandler.MissionWebhook(partner.GetUlongId(), gConfig, Missions).Wait();
                            count++;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    await ReplyEmbedAsync($"({mEmbedList.Count()}){nameOfFilter} are sent to premium discords({count}).");
                }
                else
                {
                    await ReplyEmbedErrorAsync($"there is no {nameOfFilter} right now.");
                }
            });
        }

        [FTNPowerOwner]
        [Command("server.resent.all.brstore")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentAllBrStore()
        {
            return Task.Run(async () =>
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 0, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;
                var exApi = DIManager.Services.GetRequiredService<IExternalApi>();
                var partners = Context.Repo.Priority.GetValidGuildPartners();
                var result = exApi.GetBattleRoyaleDailyStore();
                var img = await result.Value?.GetBrDailyImageAsync();
                foreach (var priority in partners)
                {
                    var gConfig = Context.Repo.Guild.GetConfig(priority.Id);
                    if (gConfig == null)
                        continue;
                    FortniteEventHandler.BrStoreWebhook(priority.GetUlongId(), result.Value?.GetBrDailyImageName(), result.Value?.GetBrDailyTitle(), gConfig).Wait();
                }
            });
        }
        [FTNPowerOwner]
        [Command("server.resent.all.vbucks")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentVbucksAlerts()
        {
            return ResentSelectedMissions("VBucks", f => f.HasVBuck());
        }
        [FTNPowerOwner]
        [Command("server.resent.all.mythics")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentMythicAlerts()
        {
            return ResentSelectedMissions("Mythics", f => f.AnyMythic());
        }
        [FTNPowerOwner]
        [Command("server.resent.daily")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentDaily()
        {
            return Task.Run(() =>
            {
                var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                var dailyllama = queueApi.DailyLlama().ToList();
                FortniteEventHandler.DailyLlamaWebhook(Context.Guild.Id, Context.GuildConfig, new List<Embed>() { dailyllama.ToWebhookEmbed() }).Wait();
            });
        }

        [FTNPowerOwner]
        [Command("server.resent.brstore")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public Task ResentBrDaily()
        {
            return _ResentBrDaily();
        }

        [FTNPowerOwner]
        [Command("server.resent.stwstore")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public Task ResentStwStore()
        {
            return _ResentStwStore();
        }
        [FTNPowerOwner]
        [Command("server.resent.vbuck")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task ResentVBuck()
        {
            return Task.Run(() =>
            {
                var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                var mssin = queueApi.MissionWhere(f => f.HasVBuck());
                FortniteEventHandler.MissionWebhook(Context.Guild.Id, Context.GuildConfig, mssin);
            });
        }

        [FTNPowerOwner]
        [Command("selam")]
        public Task Selam()
        {
            return ReplyEmbedAsync($"Selam Usta {Context.User.Mention}", null, null, new EmbedFooterBuilder()
            {
                Text = $"latency: {Context.Client.Latency}"
            });
        }

        [FTNPowerOwner]
        [Alias("server.message.send")]
        [Command("sunucu.mesaj.gönder")]
        [RequireContext(ContextType.Guild)]
        public async Task SendDmMessage(ulong guildId, [Remainder] string msg)
        {
            try
            {
                var socketuser = Context.Message.Author;
                try
                {
                    await Context.Message.DeleteAsync();
                }

                catch (Exception e)

                {
                }
                var socketGuild = Context.DiscordRestApi.GetApi.GetGuildAsync(guildId).Result;
                if (socketGuild != null)
                {
                    var serverAdmin = $"<@{socketGuild.OwnerId}>";
                    string message = $"**FortnitePower Help Center: **https://discord.gg/C9CgzVg" +
                                     $"\n```diff\n! [only you got this message]\n" +
                                     $"- you directly got a message from FortnitePower's OWNER\n" +
                                     $"```\n```diff\n! ======= [DO NOT REPLY THIS MESSAGE]\n" +
                                     $"```\n" +
                                     $"{socketuser.Username}#{socketuser.Discriminator}:\n*{msg}*" +
                                     $"\n" +
                                     $"```diff" +
                                     $"\n! ======= [DO NOT REPLY THIS MESSAGE]\n```";
                    var dm = await socketGuild.GetOwnerAsync().Result.GetOrCreateDMChannelAsync();
                    await dm.SendMessageAsync(message);
                    await dm.CloseAsync();

                    var dm2 = await socketuser.GetOrCreateDMChannelAsync();
                    await dm2.SendMessageAsync("MESSAGE COPY OF\n" + message);
                    await dm2.CloseAsync();
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}\n");
            }
        }

        [FTNPowerOwner]
        [Alias("servers.count")]
        [Command("sunucular.adet")]
        [RequireContext(ContextType.Guild)]
        public Task ServerCount()
        {
            var count = Context.Client.Guilds.Count();
            var usersum = Context.Client.Guilds.Sum(x => x.MemberCount);
            return ReplyEmbedAsync($"Guilds:{count}\nUsers:{usersum}", $"Servers\n");
        }

        [FTNPowerOwner]
        [Alias("server.info")]
        [Command("sunucu.bilgi")]
        [RequireContext(ContextType.Guild)]
        public async Task ServerInfo()
        {
            await _ServerInfo(Context.GuildConfig.GUid);
        }

        [FTNPowerOwner]
        [Alias("server.info")]
        [Command("sunucu.bilgi")]
        [RequireContext(ContextType.Guild)]
        public async Task ServerInfo(ulong serverId)
        {
            await _ServerInfo(serverId);
        }

        [FTNPowerOwner]
        [Alias("servers")]
        [Command("sunucular")]
        [RequireContext(ContextType.Guild)]
        public Task ServerList()
        {
            var glds = Context.Client.Guilds.OrderBy(s => s.Id);
            string list
                = $"**bot active in that servers(count:{glds.Count()})(ManageRoles/ManageNicknames/EmbedLinks)**\n\n";
            foreach (var guild in glds)
            {
                string perms = $"**({guild.CurrentUser.GuildPermissions.ManageRoles}/" +
                               $"{guild.CurrentUser.GuildPermissions.ManageNicknames}/" +
                               $"{guild.CurrentUser.GuildPermissions.EmbedLinks})**";
                var currentGuild = $"**Id: **__{guild.Id}__\t**Users: **__{guild.Users.Count(s => !s.IsBot)}__\t**Name: **__{guild.Name}__{perms}\n";
                if (list.Length + currentGuild.Length <= 1900)
                {
                    list += currentGuild;
                }
                else
                {
                    ReplyAsync($"{list}\n").Wait();
                    list = currentGuild;
                }
            }
            return ReplyAsync($"{list}\n");
        }

        [FTNPowerOwner]
        [Alias("server.roles.get")]
        [Command("sunucu.roller.getir")]
        [RequireContext(ContextType.Guild)]
        public async Task ServerRoles(ulong serverId)
        {
            try
            {
                string responseData = "__**Discord Server Roles**__\n";
                var language = GetLanguage(serverId);
                var selectedGuild = Context.DiscordRestApi.GetApi.GetGuildAsync(serverId).Result;
                if (selectedGuild == null)
                {
                    await ReplyEmbedAsync($"`{serverId}` **is not found.**\n");
                    return;
                }
                var serverMapRoles = selectedGuild.GetMapRolesAsync(language).Result;
                var defaultRoles = ((MapRoles[])Enum.GetValues(typeof(MapRoles))).ToList();
                Dictionary<MapRoles, string> missingRoles = new Dictionary<MapRoles, string>();
                foreach (var crole in defaultRoles)
                {
                    var currentRole = "";
                    if (crole == MapRoles.noname)
                    {
                        currentRole = Translate.GetBotTranslation(BotTranslationString.noname, language);
                    }
                    else if (crole == MapRoles.Stonewood)
                    {
                        currentRole = Translate.GetBotTranslation(BotTranslationString.Stonewood, language);
                    }
                    else if (crole == MapRoles.Plankerton)
                    {
                        currentRole = Translate.GetBotTranslation(BotTranslationString.Plankerton, language);
                    }
                    else if (crole == MapRoles.CannyValley)
                    {
                        currentRole = Translate.GetBotTranslation(BotTranslationString.CannyValley, language);
                    }
                    else if (crole == MapRoles.TwinePeaks)
                    {
                        currentRole = Translate.GetBotTranslation(BotTranslationString.TwinePeaks, language);
                    }
                    missingRoles.Add(crole, currentRole);
                }
                foreach (var crole in missingRoles)
                {
                    var selected = serverMapRoles.FirstOrDefault(p => p.Value.Name == crole.Value);
                    if (selected.Value != null)
                    {
                        responseData += $":white_check_mark: {selected.Value}\n";
                    }
                    else
                    {
                        responseData += $":x: \"`{crole.Value}`\"\n";
                    }
                }
                await ReplyAsync($"{responseData}\n");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}\n");
            }
        }

        [FTNPowerOwner]
        [Alias("server.roles.add")]
        [Command("sunucu.roller.ekle")]
        [RequireContext(ContextType.Guild)]
        public async Task ServerRolesAdd(ulong serverId)
        {
            try
            {
                var language = GetLanguage(serverId);
                var selectedGuild = Context.DiscordRestApi.GetApi.GetGuildAsync(serverId).Result;

                if (selectedGuild == null)
                {
                    await ReplyEmbedAsync($"`{serverId}` **is not found.**\n");
                    return;
                }
                if (!selectedGuild.GetCurrentUserAsync().Result.GuildPermissions.ManageRoles)
                {
                    await ReplyEmbedAsync($"**needs ManageRoles permission to update roles at** `{selectedGuild.Name}` **Discord Server.**\n");
                    return;
                }
                var reorder = new List<ReorderRoleProperties>();
                var list = selectedGuild.GetMissingMapRolesAsync(language, out reorder).Result;
                foreach (var selectR in list)
                {
                    await selectR.Value.SetMapRolePermissionsAsync();
                }
                await selectedGuild.ReorderRolesAsync(reorder);
                await ReplyEmbedAsync($"`missing map-roles` **are successfully added to** `{selectedGuild.Name}`.\n");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}\n");
            }
        }

        [FTNPowerOwner]
        [Command("server.restrict.role")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task SetRestrictionRole([Remainder] string roleName = null)
        {
            return _SetRestrictionRole(roleName);
        }

        [FTNPowerOwner]
        [Alias("server.user.name")]
        [Command("sunucu.üye.kayıt")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task SetUserName(string mention, [Remainder] string name)
        {
            return _NameChange(mention, name);
        }

        [FTNPowerOwner]
        [Alias("server.stat")]
        [Command("sunucu.durum")]
        [RequireContext(ContextType.Guild)]
        public async Task Status(ulong serverId)
        {
            var selectedGuild = Context.DiscordRestApi.GetApi.GetGuildAsync(serverId).Result;
            if (selectedGuild == null)
            {
                await ReplyEmbedAsync($"`{serverId}` **is not found.**\n");
                return;
            }
            await _StatusAsync(selectedGuild);
        }

        [FTNPowerOwner]
        [Command("users.autoupdate")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task UpdateCurrentGuildUserAsync()
        {
            return AutoUpdateSelectedGuildUsers(Context.Guild.Id.ToString());
        }



        [FTNPowerOwner]
        [Alias("server.user.banlist")]
        [Command("sunucu.user.banlist")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task UserBanlist()
        {
            var lst = Context.Repo.Db<BlackListGuild>().All().ToList();
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Banned User List";
            string banlist = "";
            foreach (var user in lst)
            {
                ulong guid = ulong.Parse(user.Id);
                var cuser = Context.Client.GetUser(guid);
                banlist += $"id:{guid}, name:{cuser?.Username}#{cuser?.Discriminator}\n";
            }
            embed.Description = banlist;
            await ReplyEmbedAsync(embed);
        }

        [FTNPowerOwner]
        [Alias("server.user.ban.remove")]
        [Command("sunucu.user.ban.remove")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task UserBanRemove(string mention)
        {
            if (!mention.HasMentionedId().Key)
            {
                return;
            }
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "Banned User Removed";
            Context.Repo.Blacklist.RemoveUserFromBlacklist(mention.HasMentionedId().Value);
            var cuser = Context.Client.GetUser(mention.HasMentionedId().Value.ToUlong());
            embed.Description = $"id:{mention.HasMentionedId().Value}, name:{cuser?.Username}#{cuser?.Discriminator} *ban has been removed...*";
            await ReplyEmbedAsync(embed);
        }

        [FTNPowerOwner]
        [Command("server.ingame")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task UserGameInfo(string userName)
        {
            return _UserGameInfo(userName);
        }

        [FTNPowerOwner]
        [Command("sunucu.üye.bilgi")]
        [Alias("server.user.info")]
        [RequireContext(ContextType.Guild)]
        public Task UserInfo(string mention)
        {
            return _UserInfo(mention);
        }
    }
}