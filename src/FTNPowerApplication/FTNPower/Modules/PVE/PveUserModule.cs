using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Global;
using Fortnite.Core.Interfaces;
using Fortnite.Core.Services;
using Fortnite.Model.Enums;
using Fortnite.Localization;
using FTNPower.Model.WebsiteModels;
using FTNPower.Model.Tables;
using FTNPower.Core.Interfaces;
using StackExchange.Redis;
using FTNPower.Model.Enums;
using FTNPower.Data;
using Fortnite.Api;
using Fortnite.Core;
using FTNPower.Core;

namespace FTNPower.Modules.PVE
{
    public class PveUserModule : PveFunctionLayer
    {
        public PveUserModule() : base()
        {
        }

        [Alias("mod")]
        [Command("mode")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task GameMode(string gameMode)
        {
            return GameMode(Context.DiscordUser.Uid, gameMode);
        }
        [Command("vote")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task Vote()
        {
            return Task.Run(async () =>
            {
                string cmdText = "show your love to [FTN Power](https://bit.ly/FTNPowerVoteLink1) :heart:";
                await ReplyEmbedAsync(cmdText, "FTN Power Vote");
            });
        }
        [Alias("yardım")]
        [Command("help")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task Help()
        {
            return _HelpCmd2();
        }
        [Command("unlock")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task UnLockNameAsync()
        {
            return _unlock(Context.Guild.Id.ToString(), Context.User.Id.ToString());
        }
        [Command("lock")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task LockNameAsync()
        {
            return _lock(Context.Guild.Id.ToString(), Context.User.Id.ToString());
        }

        [Alias("info", "donate")]
        [Command("bilgi")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task MadeBy()
        {
            EmbedBuilder emb = new EmbedBuilder();
            emb.WithTitle("`by Kesintisiz` (*Mustafa Salih ASLIM*)");
            emb.WithAuthor((a) =>
            {
                a.WithName("FTN Power");
                a.WithIconUrl(Context.Guild.CurrentUser.GetAvatarUrl());
            });
            emb.WithDescription(ToTranslate(BotTranslationString.BotDescription));
            emb.AddField(ToTranslate(BotTranslationString.Help), "-", true);
            emb.AddField("https://discord.gg/C9CgzVg", "-", false);
            emb.AddField($"Get Pro:", "[via Paypal](https://www.paypal.com/paypalme/humeyraa)", true);
            emb.AddField($"Twitter:", "[@ftnpower](https://twitter.com/ftnpower)", true);
            emb.AddField($"Reddit:", "[u/ftnpower](https://www.reddit.com/user/FortnitePower)", true);
            emb.AddField($"Vote:", "[on top.gg](https://top.gg/bot/454547389731045380/vote)", true);
            emb.WithColor(Color.LightOrange);
            Context.Channel.SendMessageAsync("", false, emb.Build());
            return Task.CompletedTask;
        }

        [Command("kayıt")]
        [Alias("name", "kayit", "link")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task NameChange([Remainder] string newName)
        {
            return _NameChange(null, newName);
        }
        [Command("unlink")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task UnLinkEpicName()
        {
            return Task.Run(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    NameState ns = Context.Repo.Db<NameState>().Where(p => p.DiscordServerId == Context.Guild.Id.ToString() && p.FortniteUserId == Context.DiscordUser.Id).FirstOrDefault();
                    if (ns != null)
                    {
                        ns.LockName = false;
                        Context.Repo.Db<NameState>().Update(ns);
                        Context.Repo.Commit();
                    }
                    Context.DiscordUser.EpicId = null;
                    Context.DiscordUser.IsValidName = false;
                    Context.DiscordUser.VerifiedProfile = false;
                    Context.Repo.Db<FortniteUser>().Update(Context.DiscordUser);
                    Context.Repo.Commit();
                    try
                    {
                        var user = (SocketGuildUser)Context.Message.Author;
                        List<Task> tlist = new List<Task>();
                        tlist.Add(user.ModifyAsync((o) =>
                        {
                            o.Nickname = null;
                        }));
                        var roles = await user.GetUserRolesAsync(GetLanguage());
                        foreach (var role in roles)
                        {
                            tlist.Add(user.RemoveRoleAsync(role.Value));
                        }
                        Task.WaitAll(tlist.ToArray());
                    }
                    catch (Exception)
                    {

                    }
                    await ReplyEmbedAsync(Translate.GetBotTranslation(BotTranslationString.NameUnlinked, GetLanguage(), Context.PlayerName));
                }
                else
                {
                    await ReplyEmbedAsync(Translate.GetBotTranslation(BotTranslationString.NameisNotLinked, GetLanguage()));
                }
            });
        }

        [Alias("isim.tag")]
        [Command("name.tag")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public Task NameTag([Remainder]string tag)
        {
            return _NameTag(Context.DiscordUser, Context.Guild.GetUser(Context.User.Id), tag);
        }

        [Alias("yama")]
        [Command("patch")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task PatchNotes()
        {
            return Task.Run(async () =>
            {
                var result = Api.GetVersion();
                var split = result.Split(' ');
                var version = "";
                var url = "https://www.epicgames.com/fortnite/patch-notes";
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Color.DarkPurple,
                    ThumbnailUrl = "https://cdn.discordapp.com/emojis/522531736828117011.png",
                    Title = Translate.GetBotTranslation(BotTranslationString.PatchNotes, GetLanguage(), version),
                    Description = Translate.GetBotTranslation(BotTranslationString.CurrentPatchNotes, GetLanguage()),
                };
                await ReplyEmbedAsync(embed);
            });
        }
        private Task PveMissionShortCmd(IEnumerable<IMissionX> missions, string cmdType)
        {
            if (!HasPVEMode())
                return Task.CompletedTask;
            return CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    KeyValuePair<bool, PriorityTable> pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                    if (pstate.Value == null)
                        return;
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Author = GetVerificationAuthor(),
                        Color = Color.Green
                    };

                    this.DrawMissions(pstate, embed, missions, $"for: '{cmdType}'");
                    if (pstate.Value == null)
                    {
                        Context.Repo.StoredProcedure.SP_User_LastUpdateTime(Context.DiscordUser.Id);
                    }
                }
            });
        }
        [Command("vbuck")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissionsVbuck()
        {
            var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
            return PveMissionShortCmd(queueApi.MissionWhere(p => p.HasVBuck()), "vbuck");
        }
        [Command("lperk")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissionsLegendPerk()
        {
            var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
            return PveMissionShortCmd(queueApi.MissionWhere(p => p.HasLegendaryPerkUp(WorldName.All)).Take(9), "legendary perk");
        }
        [Command("eperk")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissionsEpicPerk()
        {
            var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
            return PveMissionShortCmd(queueApi.MissionWhere(p => p.HasEpicPerkUp(WorldName.All)).Take(9), "epic perk");
        }
        [Command("lgsur")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissionsLegendarySurvivor()
        {
            var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
            return PveMissionShortCmd(queueApi.MissionWhere(p => p.HasLegendarySurvivor()), "legendary survivor");
        }
        [Command("lg")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissionsLegendaries()
        {
            var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
            return PveMissionShortCmd(queueApi.MissionWhere(p => p.AnyLegendary()).Take(9), "legendary");
        }
        [Alias("pve.mission", "pm")]
        [Command("pve.missions")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissions([Remainder]string search)
        {
            if (!HasPVEMode())
                return Task.CompletedTask;
            return CheckRestriction(async () =>
            {
                if (search.Length > 50)
                {
                    Log.Write(Serilog.Events.LogEventLevel.Error, "user {UserName}({UserId}) in guild {GuildId} has exeeded valid length '{InvalidLength}', searchKey is'{InvalidCmd}'", Context.User.Username, Context.User.Id, Context.Guild.Id, search.Length, search);
                    return;
                }
                if (Context.DiscordUser.IsValidName)
                {
                    KeyValuePair<bool, PriorityTable> pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                    if (pstate.Value == null)
                        return;
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Author = GetVerificationAuthor(),
                        Color = Color.Green
                    };

                    IEnumerable<IMissionX> missions = null;
                    var src = search.ToLowerInvariant();
                    var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                    switch (src)
                    {
                        case "legend survivors":
                        case "legend survivor":
                            missions = queueApi.MissionWhere(p => p.HasLegendarySurvivor()).Distinct();
                            break;

                        case "vbuck":
                        case "vbucks":
                            missions = queueApi.MissionWhere(p => p.HasVBuck()).Distinct();
                            break;

                        case "mythic":
                        case "mythics":
                            missions = queueApi.MissionWhere(p => p.AnyMythic()).Distinct();
                            break;

                        case "legendary":
                            missions = queueApi.MissionWhere(p => p.AnyLegendary()).Distinct().Take(11);
                            break;

                        case "epic":
                            missions = queueApi.MissionWhere(p => p.AnyEpic()).Distinct().Take(15);
                            break;

                        case "legend perk":
                            missions = queueApi.MissionWhere(p => p.HasLegendaryPerkUp(WorldName.All)).Distinct().Take(11);
                            break;

                        case "epic perk":
                            missions = queueApi.MissionWhere(p => p.HasEpicPerkUp(WorldName.All)).Distinct().Take(11);
                            break;

                        case "pure drop":
                            missions = queueApi.MissionWhere(p => p.Has4xPureDropOfRain(WorldName.Twine_Peaks)).Distinct().Take(11);
                            break;

                        case "storm shard":
                            missions = queueApi.MissionWhere(p => p.Has4xStormShard(WorldName.Twine_Peaks)).Distinct().Take(11);
                            break;

                        case "lightning":
                            missions = queueApi.MissionWhere(p => p.Has4xLightningInABottle(WorldName.Twine_Peaks)).Distinct().Take(11);
                            break;

                        case "eyeofstorm":
                            missions = queueApi.MissionWhere(p => p.Has4xEyeOfStorm(WorldName.Twine_Peaks)).Distinct().Take(11);
                            break;

                        default:
                            await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.SelectItemType1, GetLanguage()));
                            return;
                    }
                    this.DrawMissions(pstate, embed, missions, $"for: '{src}'");
                    if (pstate.Value == null)
                    {
                        Context.Repo.StoredProcedure.SP_User_LastUpdateTime(Context.DiscordUser.Id);
                    }
                }
            });
        }

        [Alias("pve.mission", "pm")]
        [Command("pve.missions")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveMissions()
        {
            if (!HasPVEMode())
                return Task.CompletedTask;
            return CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    KeyValuePair<bool, PriorityTable> pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                    if (pstate.Value == null)
                        return;
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Author = GetVerificationAuthor(),
                        Color = Color.Green
                    };
                    var queueApi = DIManager.Services.GetRequiredService<IFortniteQueueApi>();
                    var missions = queueApi.MissionTop10();
                    if (missions != null)
                    {
                        this.DrawMissions(pstate, embed, missions, "");
                    }
                    if (pstate.Value == null)
                    {
                        Context.Repo.StoredProcedure.SP_User_LastUpdateTime(Context.DiscordUser.Id);
                    }
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd1, GetLanguage(), Context.User.Mention));
                }
            });
        }

        [Command("ingame")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task PvePlayer([Remainder] string userName)
        {
            SocketGuildUser cuser = Context.Message.Author as SocketGuildUser;
            if (Context.Message.Author.Id == 382413606459015170 ||
                Context.Message.Author.Id == 265567747079929857 ||
                Context.Message.Author.Id == 205848197589893121 ||
                Context.Message.Author.Id == 193749607107395585 ||
                cuser != null && cuser.GuildPermissions.Administrator)
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;

                if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                {
                    await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                    return;
                }

                await _UserGameInfo(userName);
                return;
            }
            await Context.Message.DeleteAsync();
            return;
        }

        [Command("ingames")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task PvePlayers()
        {
            SocketGuildUser cuser = Context.Message.Author as SocketGuildUser;
            if (Context.Message.Author.Id == 382413606459015170 ||
                Context.Message.Author.Id == 265567747079929857 ||
                Context.Message.Author.Id == 205848197589893121 ||
                Context.Message.Author.Id == 193749607107395585 ||
                cuser != null && cuser.GuildPermissions.Administrator)
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;

                if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                {
                    await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                    return;
                }

                await _PveplayerList();
                return;
            }
            await Context.Message.DeleteAsync();
            return;
        }
        [Command("pve")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task PveProfile()
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await PveProfile(true);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }

        [Command("pve")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task PveProfile([Remainder]string userName)
        {
            await CheckRestriction(async () =>
              {
                  if (Context.DiscordUser.IsValidName)
                  {
                      await PveProfile(false, userName);
                  }
                  else
                  {
                      await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.YouShouldHaveValidName, GetLanguage()) + Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                  }
              });
        }

        [Alias("pve.survivor", "ps")]
        [Command("pve.survivors")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task PveSurvivorReq()
        {
            return _PveSurvivorReqirements();
        }

        [Alias("update", "güncel", "pvp.up", "pvp.update")]
        [Command("up")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task RefreshName()
        {
            return _UpdateName(Context.User.Mention);
        }

        [Alias("pve.kaynaklar", "pr")]
        [Command("pve.resources")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task ResourcesListAsync([Remainder]string userName)
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await ResourcesList(false, userName);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.YouShouldHaveValidName, GetLanguage()) + Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }

        [Alias("pve.kaynaklar", "pr")]
        [Command("pve.resources")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task ResourcesListAsync()
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await ResourcesList(true);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.YouShouldHaveValidName, GetLanguage()) + Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }

        [Command("top")]
        [Alias("top.local")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public Task Top()
        {
            return _Top2(Context.Guild.Id);
        }

        [Command("top.global")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public Task TopGlobal()
        {
            return _Top2();
        }


        [Command("verify")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.AddReactions)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task AccountVerify()
        {
            return Task.Run(async () =>
            {
                var usr = (SocketGuildUser)Context.User;
                if (!Context.DiscordUser.IsValidName)
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), usr.Mention));
                    return;
                }
                if (Context.DiscordUser.VerifiedProfile)
                {
                    await ReplyEmbedErrorAsync("you are already verified.");
                    return;
                }

                if (Context.Repo.Db<FortniteUser>().Where(f => f.VerifiedProfile && f.IsValidName && f.EpicId == Context.DiscordUser.EpicId && f.Id != Context.DiscordUser.Id).Any())
                {
                    await ReplyEmbedErrorAsync($"you can not take {Context.PlayerName} Username, it is __already verified__ by account owner.");
                    return;
                }

                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 10, 0), Context.DiscordUser, usr);
                if (pstate.Value == null)
                    return;
                var verifyPro = Context.Repo.Db<EpicIdVerifyOrder>().GetById(Context.DiscordUser.EpicId);
                if (verifyPro == null)
                {
                    var rabbirMqService = DIManager.Services.GetRequiredService<IRedisService>();
                    var msg = await ReplyEmbedAsync(
                        "**To be verified**\n please send Friend Request to **VerifyProfile** (epic username) __in 2 minutes__ on *Epic Games* then **wait EMOTE REACTION from bot**.");
                    rabbirMqService.ListLeftPush("ProfileVerificationQueue", new ReadyToVerify()
                    {
                        JustDeQueue = true,
                        EpicId = Context.DiscordUser.EpicId
                    }, CommandFlags.FireAndForget);

                    try
                    {
                        Context.Repo.Db<EpicIdVerifyOrder>().Add(new EpicIdVerifyOrder()
                        {
                            Id = Context.DiscordUser.EpicId
                        });
                        Context.Repo.Commit();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    ReadyToVerify verify = new ReadyToVerify(Context.PlayerName, Context.DiscordUser.EpicId, Context.DiscordUser.Id)
                    {
                        MessageId = msg.Id,
                        ChannelId = Context.Message.Channel.Id
                    };
                    rabbirMqService
              .ListLeftPush("ProfileVerificationQueue", verify);

                }
                else
                {
                    await ReplyEmbedErrorAsync(
                        $"**someone(maybe you), trying to verify this epic-name({Context.PlayerName})**, try again 2 minutes later.");
                }
            });
        }
    }
}