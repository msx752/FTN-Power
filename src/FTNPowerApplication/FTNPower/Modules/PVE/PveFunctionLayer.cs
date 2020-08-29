using Discord;
using Discord.WebSocket;
using fortniteLib;
using fortniteLib.Responses;
using Microsoft.Extensions.DependencyInjection;
using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FTNPower.Model.Enums;
using FTNPower.Data;
using Fortnite.External;
using Fortnite.External.Api.Interfaces;
using Fortnite.External.Responses.BDailyStore;
using Fortnite.Static;
using Fortnite.Model.Enums;
using FTNPower.Model.Tables.StoredProcedures;
using FTNPower.Model;
using FTNPower.Model.Tables;
using Fortnite.Core.ResponseModels;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Localization;
using FTNPower.Modules;
using FTNPower.Core;
using Image.Core;
using Newtonsoft.Json;
using RestSharp.Extensions;
using System.IO;
using StackExchange.Redis;

namespace FTNPower.Modules.PVE
{
    public abstract class PveFunctionLayer : FortniteFunctionLayer
    {
        public Task _lock(string guildId, string userId)
        {
            return Task.Run(() =>
            {
                var pstate = CheckUserPriority(new TimeSpan(0, 0, 0, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id)).Result;
                if (pstate.Value == null)
                    return;
                if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                {
                    ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures)).Wait();
                    return;
                }
                Context.Repo.LockName(guildId, userId);
                ReplyEmbedAsync(Translate.GetBotTranslation(BotTranslationString.NameIsLocked, GetLanguage(), $"<@{userId}>")).Wait();
            });
        }
        public Task _unlock(string guildId, string userId)
        {
            return Task.Run(() =>
            {
                var pstate = CheckUserPriority(new TimeSpan(0, 0, 0, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id)).Result;
                if (pstate.Value == null)
                    return;
                Context.Repo.UnLockName(guildId, userId);
                ReplyEmbedAsync(Translate.GetBotTranslation(BotTranslationString.NameIsUnLocked, GetLanguage(), $"<@{userId}>")).Wait();
            });
        }
        public Task _ResentBrDaily()
        {
            return Task.Run(async () =>
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 0, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;
                var exApi = DIManager.Services.GetRequiredService<IExternalApi>();
                var result = exApi.GetBattleRoyaleDailyStore();
                var img = await result.Value?.GetBrDailyImageAsync();
                FortniteEventHandler.BrStoreWebhook(Context.Guild.Id, result.Value?.GetBrDailyImageName(), result.Value?.GetBrDailyTitle(), Context.GuildConfig).Wait();
            });
        }
        public Task _ResentStwStore()
        {
            return Task.Run(async () =>
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 0, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;
                string fileName = await Api.SaveStwStoreToLocalAsync(false);
                string title = DateTimeOffset.UtcNow.SetStwStoreTitle();
                FortniteEventHandler.StwStoreWebhook(Context.Guild.Id, fileName, title, Context.GuildConfig).Wait();
            });
        }
        public PveFunctionLayer()
            : base()
        {
        }

        public Task _PveplayerList(ulong? guildId = null, bool IsBR = true)
        {
            return Task.Run(async () =>
           {
               if (!guildId.HasValue)
               {
                   guildId = Context.Guild.Id;
               }
               SocketGuild guild = Context.Client.GetGuild(guildId.Value);
               var list = guild.Users.Where(f => f.Activity.IsFortnitePlayer()).ToList();
               string ingameList = $"**now playing FORTNITE in discord [{list.Count} users]**\r\n\r\n";
               foreach (var VARIABLE in list)
               {
                   var ga = VARIABLE.Activity.GetFAcvitity();
                   if (ga == null)
                       continue;
                   var ts = ga.Timestamps;
                   if (ts == null)
                       continue;
                   var s = ts.Start;
                   var n = VARIABLE.Nickname;
                   if (n == null)
                   {
                       n = $"{VARIABLE.Username}#{VARIABLE.Discriminator}";
                   }
                   var currentline = $"elapsed:**{(DateTimeOffset.UtcNow - s)?.ToString("h'h 'm'm 's's'")}**, {n}\r\n";
                   if (ingameList.Length + currentline.Length < 1900)
                   {
                       ingameList += currentline;
                   }
                   else
                   {
                       await ReplyEmbedAsync(ingameList + "\n");
                       ingameList = currentline;
                   }
               }
               await ReplyEmbedAsync(ingameList);
           });
        }

        public Task _PveSurvivorReqirements()
        {
            if (!HasPVEMode())
                return Task.CompletedTask;
            return CheckRestriction(async () =>
             {
                 var pstate = await CheckUserPriority(new TimeSpan(0, 0, 8, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                 if (pstate.Value == null)
                     return;
                 if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                 {
                     await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                     return;
                 }

                 var profile = await Api.GetPVEProfileById(Context.DiscordUser.EpicId);
                 var svvrs = await profile.Value.GetSurvivors();
                 var squads = await svvrs.GetSurvivorSquads();
                 svvrs = null;
                 var evolutionAccess = profile.Value.profileChanges.First().profile.items.Count(f => f.Value.templateId.StartsWith("HomebaseNode:questreward_evolution"));
                 int maxLevel = 10;
                 maxLevel = maxLevel + evolutionAccess * 10;
                 EmbedBuilder emb = new EmbedBuilder();
                 emb.Color = Color.Orange;
                 emb.Title = ToTranslate(BotTranslationString.PS_Title, maxLevel);
                 var playerNameWithEnergy = $"**{Fortnite.Core.Utils.GetIntegerPower(Context.AccountPowerLevel)}**:zap:**{Context.PlayerName}**";
                 emb.Description = ToTranslate(BotTranslationString.PS_Description, playerNameWithEnergy);
                 var invisLine = 3 - squads.Count() % 3;
                 Dictionary<string, int> totalRequirements = new Dictionary<string, int>();

                 foreach (var squad in squads)
                 {
                     var squadId = "";
                     string requirementList = "";
                     Dictionary<string, int> Requirements = new Dictionary<string, int>();
                     foreach (var survivor in squad.Survivors)
                     {
                         if (squadId == "")
                             squadId = survivor.SquadId;
                         var rqiremnts = survivor.CalcResourceRequirements(maxLevel);
                         foreach (var rq in rqiremnts)
                         {
                             if (Requirements.ContainsKey(rq.Key))
                             {
                                 Requirements[rq.Key] += rq.Value;
                             }
                             else
                             {
                                 Requirements.Add(rq.Key, rq.Value);
                             }

                             if (totalRequirements.ContainsKey(rq.Key))
                             {
                                 totalRequirements[rq.Key] += rq.Value;
                             }
                             else
                             {
                                 totalRequirements.Add(rq.Key, rq.Value);
                             }
                         }
                     }
                     if (Requirements.Count == 0)
                     {
                         requirementList = $"**{ToTranslate(BotTranslationString.PS_Max)}**";
                     }
                     else
                     {
                         foreach (var requirement in Requirements)
                         {
                             var storedImage = SurvivorStaticData.AssetBot2[requirement.Key];
                             string quantity = requirement.Value.ToString("#,##0.##");
                             requirementList += $"<:{storedImage.EmojiId}:{storedImage.EmojiId}>x{quantity}\n";
                         }
                     }
                     var storedCurrentItem = SurvivorStaticData.AssetBot2[squadId];
                     var cTeamName = Translate.GetBotTranslation(squadId, GetLanguage());
                     emb.AddField($"<:{storedCurrentItem.EmojiId}:{storedCurrentItem.EmojiId}>{cTeamName}", requirementList, true);
                     //break;
                 }
                 string totalReq = "";
                 if (totalRequirements.Count != 0)
                 {
                     foreach (var requirement in totalRequirements)
                     {
                         var Image2 = SurvivorStaticData.AssetBot2[requirement.Key];
                         var q = requirement.Value;
                         var queryName = $"AccountResource:{requirement.Key}";
                         var item = profile.Value.profileChanges.First().profile.items
                              .FirstOrDefault(f => f.Value.templateId == queryName);
                         string quantity = "";
                         if (item.Value != null)
                         {
                             q = q - item.Value.quantity;
                         }

                         if (q <= 0)
                         {
                             quantity = ":white_check_mark:";
                         }
                         else
                         {
                             quantity = q.ToString("#,##0.##");
                         }
                         totalReq += $"<:{Image2.EmojiId}:{Image2.EmojiId}>x{quantity}\n";
                     }
                     emb.AddField(ToTranslate(BotTranslationString.PS_Needs), totalReq, true);
                     invisLine--;
                 }
                 for (int i = 0; i < invisLine; i++)
                 {
                     emb.AddField(SurvivorStaticData.InvinsibleImage(), SurvivorStaticData.InvinsibleImage(), true);
                 }
                 await ReplyEmbedAsync(emb, pstate);
             });
        }

        public async Task _ServerInfo(ulong serverId)
        {
            try
            {
                var selectedGuild = Context.Client.GetGuild(serverId);
                if (selectedGuild == null)
                {
                    await ReplyEmbedErrorAsync($"`{serverId}` **is not found.**\n");
                    return;
                }
                var userList = selectedGuild.Users.Where(s => !s.IsBot).AsEnumerable();
                var userCount = userList.Count();
                var onlineUserCount = userList.Count(p => p.IsAvailable());
                int UserHasMapRole = 0;
                int UserHasEnergy = 0;
                var language = GetLanguage(serverId);
                var serverMapRoles = selectedGuild.GetMapRolesAsync(language).Result;
                var botConfig = Context.Redis.JsonGet<BotConfig>();
                foreach (SocketGuildUser usr in userList)
                {
                    foreach (var role in serverMapRoles)
                    {
                        if (usr.Roles.Any(p => p.Name == role.Value.Name))
                        {
                            UserHasMapRole++;
                            break;
                        }
                    }
                    var nick = usr?.Nickname;
                    if (nick != null)
                    {
                        if (nick.Contains(botConfig.Vars.Lightning))
                        {
                            UserHasEnergy++;
                        }
                    }
                }

                var dserver = Context.Repo.Db<DiscordServer>().GetById(serverId.ToString());
                EmbedBuilder embed = new EmbedBuilder();
                embed.Author = GetVerificationAuthor();
                embed.Description = "Discord Server Information";
                embed.AddField("Id", dserver.Id);
                embed.AddField("Name", selectedGuild.Name);
                embed.AddField("CanManageRoles", selectedGuild.CurrentUser.GuildPermissions.ManageRoles, true);
                embed.AddField("CanManageNicknames", selectedGuild.CurrentUser.GuildPermissions.ManageNicknames, true);
                embed.AddField("CanEmbedLinks", selectedGuild.CurrentUser.GuildPermissions.EmbedLinks, true);
                embed.AddField("CanAddReactions", selectedGuild.CurrentUser.GuildPermissions.AddReactions, true);
                embed.AddField("CanSendMessages", selectedGuild.CurrentUser.GuildPermissions.SendMessages, true);
                embed.AddField("CanManageWebhooks", selectedGuild.CurrentUser.GuildPermissions.ManageWebhooks, true);
                embed.AddField("CanUseExternalEmojis", selectedGuild.CurrentUser.GuildPermissions.UseExternalEmojis, true);
                embed.AddField("Language", language, true);
                embed.AddField("UserCount", userCount, true);
                embed.AddField("OnlineUserCount", $"{userCount}/**{onlineUserCount}**");
                embed.AddField("UserHasMapRole", $"{UserHasMapRole}");
                embed.AddField("UserHasEnergy", $"{UserHasMapRole}/**{UserHasEnergy}**");
                embed.AddField("RoleCount", $"5/**{serverMapRoles.Count}**");
                embed.AddField("Roles", $"*{string.Join(", ", serverMapRoles.Values.Select(p => p.Name).ToArray())}*");
                var pstate = Context.Repo.Priority.GetPriorityTable(dserver.Id, PriorityState.Guild);
                if (pstate == null)
                {
                    embed.AddField("Pro Plan", $"**Not Active**");
                }
                else
                {
                    embed.AddField("Pro Plan", pstate.ExpiresIn);
                }
                embed.Color = Color.Green;
                await ReplyEmbedAsync(embed);
            }
            catch (Exception e)
            {
                await ReplyEmbedErrorAsync($"{e.Message}\n");
            }
        }

        public async Task _StatusAsync(IGuild guild)
        {
            try
            {
                var roles = guild.GetMapRolesAsync(GetLanguage(guild.Id)).Result;
                roles.Remove(roles.FirstOrDefault(s => s.Key == MapRoles.noname).Key);
                EmbedBuilder embed = new EmbedBuilder() { };
                embed.Title = guild.Name;
                embed.Description = "discord informations";
                int roleCount = roles.Count;
                int roleIncrement = 1;
                var users = guild.GetUsersAsync().Result;
                foreach (var role in roles)
                {
                    IEnumerable<IGuildUser> currentMapUser = users.Where(p => p.RoleIds.FirstOrDefault(x => x == role.Value.Id) != 0);
                    embed.AddField(role.Value.Name, $"**{currentMapUser.Count(p => p.IsAvailable() && !p.IsBot)}**/{currentMapUser.Count(s => !s.IsBot)}", roleIncrement != roleCount);
                    roleIncrement++;
                }
                embed.AddField("TOTAL USER", users.Count(s => !s.IsBot), true);
                embed.AddField("TOTAL ONLINE", users.Count(s => s.IsAvailable() && !s.IsBot), true);
                await ReplyEmbedAsync("Discord informations", guild.Name, embed.Fields);
            }
            catch (Exception e)
            {
                await ReplyEmbedErrorAsync($"{e.Message}\n");
            }
        }

        //requires refactoring
        public Task _Top2(ulong? guildId = null)
        {
            return Task.Run(async () =>
            {
                try
                {
                    string redisKey = "f.top-";
                    var redisExpiry = new TimeSpan(1, 0, 0);
                    if (guildId.HasValue)
                    {
                        redisExpiry = new TimeSpan(0, 5, 0);
                        redisKey += guildId;
                    }
                    var pstate = await CheckUserPriority(new TimeSpan(0, 0, 0, 5), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                    if (pstate.Value == null)
                        return;
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal && guildId.HasValue == true)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    var keyResult = (Byte[])this.Context.Redis.Connection.GetDatabase().StringGet(redisKey);
                    if (keyResult != null)
                    {
                        await ReplyImageAsync(new MemoryStream(keyResult));
                        keyResult = null;
                        return;
                    }
                    List<EpicTopUser> lst = new List<EpicTopUser>();

                    if (guildId.HasValue)
                    {
                        lst = Context.Repo.StoredProcedure.SP_LocalTop20Async(guildId.Value.ToString());
                        bool removedItem = false;
                        foreach (var user in lst)
                        {
                            var exists = await Context.DiscordRestApi.GetGuildUserAsync(guildId.Value, user.Uid);
                            if (exists == null)
                            {
                                removedItem = true;
                                Context.Repo.StoredProcedure.SP_RemoveNameStateForDiscord(Context.Guild.Id.ToString(), user.Id);
                            }
                        }
                        if (removedItem)
                            lst = Context.Repo.StoredProcedure.SP_LocalTop20Async(guildId.Value.ToString());
                    }
                    else
                    {
                        lst = Context.Repo.StoredProcedure.SP_GlobalTop20();
                    }
                    var intro = guildId == null ? "Global" : "Discord";
                    System.Drawing.Image bitmap = Bitraphic.Draw(460, 510, (o) =>
                    {
                        var backgroundColor = System.Drawing.Color.FromArgb(32, 34, 36);
                        var titleColor = System.Drawing.Color.Snow;
                        var columnColor = System.Drawing.Color.FromArgb(255, 250, 212);

                        o.Fill(backgroundColor);
                        System.Drawing.SizeF slboard = System.Drawing.SizeF.Empty;
                        o.Text($"{intro} Leaderboards", "Anton", 18f, System.Drawing.FontStyle.Regular, null, (t) => { t.ForeColor = titleColor; slboard = t.MeasureString; t.Point(((t.BaseWidth / 2) - (slboard.Width / 2)), 2); });
                        System.Drawing.SizeF snum = System.Drawing.SizeF.Empty;
                        o.Text("#", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = columnColor; snum = t.MeasureString; t.Point(5, slboard.Height);
                        });
                        o.Text("Epic Name", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = columnColor; t.Point(57, slboard.Height);
                        });
                        o.Text("PL", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = columnColor; t.Point(210, slboard.Height);
                        });
                        o.Text("Commander Lv", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = columnColor; t.Point(260, slboard.Height);
                        });
                        o.Text("Collection Lv", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                        {
                            t.ForeColor = columnColor; t.Point(365, slboard.Height);
                        });

                        var lineHeight = (slboard.Height + snum.Height) - 5;
                        for (int i = 0; i < lst.Count; i++)
                        {
                            EpicTopUser u = lst[i];
                            o.Image(0, (int)lineHeight, 460, 30, (img1) =>
                            {
                                System.Drawing.SizeF sNum = System.Drawing.Size.Empty;
                                System.Drawing.SizeF sName = System.Drawing.Size.Empty;
                                var sf = new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.NoWrap)
                                {
                                    Alignment = System.Drawing.StringAlignment.Center,
                                    LineAlignment = System.Drawing.StringAlignment.Center
                                };
                                img1.Text($"{((double)i + (double)1).ToString("00")}", "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                                {
                                    t.ForeColor = titleColor; sNum = t.MeasureString; t.Point(5, 0);
                                });
#if DEBUG
                                img1.Text(u.PlayerName, "Arial", 11f, System.Drawing.FontStyle.Regular, sf, (t) =>
#else

                                   img1.Text(u.PlayerName, "Ubuntu", 11f, System.Drawing.FontStyle.Regular, sf, (t) =>
#endif
                                {
                                    sName = t.MeasureString;
                                    t.ForeColor = titleColor;
                                    t.Point(25, -3, 180, img1.BaseHeight);
                                });
                                img1.Text(u.AccountPowerLevel.ToString("0.00"), "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                                {
                                    t.ForeColor = titleColor; t.Point(202, 0, 48, img1.BaseHeight);
                                });

                                img1.Text(u.CommanderLevel.ToString("0,000").TrimStart(new char[] { '0', ',' }), "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                                  {
                                      t.ForeColor = titleColor; t.Point(290, 0, 50, img1.BaseHeight);
                                  });

                                img1.Text(u.CollectionBookLevel.ToString("0,000").TrimStart(new char[] { '0', ',' }), "Anton", 12f, System.Drawing.FontStyle.Regular, (t) =>
                                {
                                    t.ForeColor = titleColor; t.Point(390, 0, 50, img1.BaseHeight);
                                });

                                lineHeight += (sNum.Height - 4);
                            });
                        }
                    });

                    var bstream = Bitraphic.SaveToStream(bitmap);
                    this.Context.Redis.Connection.GetDatabase().StringSet(redisKey, bstream.ToArray(), expiry: redisExpiry);
                    await ReplyImageAsync(bstream);
                }
                catch (Exception e)
                {
                    await ReplyEmbedErrorAsync($"{e.Message}\n");
                }
            });
        }

        public Task _UserGameInfo(string userName)
        {
            if (Context.Message.MentionedUsers.Count != 0)
            {
                return Task.CompletedTask;
            }
            return Task.Run(async () =>
            {
                var epic = Api.GetUserIdByName(userName);
                if (epic.Value == null)
                {
                    await ReplyEmbedErrorAsync($"**{userName}** not found.");
                    return;
                }
                var response = Api.GetUserGameInfo(epic.Value.id);

                var vv = Context.User.Activity.GetFAcvitity();
                if (response.Key == "success" && response.Value != null)
                {
                    Lookup lobbyOwner = null;
                    var rslt = Api.GetUserNameByIds(response.Value.publicPlayers.ToArray());

                    var issd = response.Value.attributes.GetZoneInstance().IsSSD;
                    var id = response.Value.publicPlayers[0];
                    if (response.Value.publicPlayers.Count == 1 && issd)
                    {
                        lobbyOwner = rslt.Value.First(f => f.id == id);
                    }
                    else if (response.Value.publicPlayers.Count == 2 && issd)
                    {
                        lobbyOwner = rslt.Value.First(f => f.id == id);
                    }
                    else if (response.Value.publicPlayers.Count > 0 & response.Value.publicPlayers.Count <= 4 && issd)
                    {
                        lobbyOwner = rslt.Value.First(f => f.id == id);
                    }
                    var names = rslt.Value.Select(p => p.displayName).ToList();
                    string ingameList = $"**FORTNITE {response.Value.attributes.GAMEMODE_s?.Replace("FORT", "")} IN-GAME**\r\n";
                    foreach (var name in names)
                    {
                        if (issd && name == lobbyOwner?.displayName)
                        {
                            ingameList += " *" + name + "*\r\n";
                        }
                        else
                        {
                            ingameList += "*" + name + "*\r\n";
                        }
                    }
                    ingameList += $"\r\nmap: {response.Value.attributes.GetZoneInstance().GetMapName()}";
                    if (issd)
                    {
                        var mapUser = await Api.GetPVEProfileById(lobbyOwner.id);
                        var ssdList = mapUser.Value.GetSSDs(response.Value.attributes.GetZoneInstance().theaterId);
                        ingameList += $"\r\napprox.Level: **{ssdList.First().First().Level}**";
                        ingameList += $"\r\nzone: **{ssdList.First().Key}**";
                    }
                    ingameList += $"\r\nallowJoin: {response.Value.allowJoinInProgress}";
                    ingameList += $"\r\nallowInvites: {response.Value.allowInvites}";
                    //ingameList += $"\r\nserver-region:{response.Value.attributes.REGION_s}";
                    //ingameList += $"\r\nserver-subregion: {response.Value.attributes.SUBREGION_s}";
                    //ingameList += $"\r\npool:{response.Value.attributes.MATCHMAKINGPOOL_s}";
                    //ingameList += $"\r\ntimeElapsed: **{(DateTimeOffset.UtcNow - response.Value.lastUpdated).ToString("h'h 'm'm 's's'")}**";
                    ingameList += $"\r\nstarted: **{response.Value.started}**";
                    ingameList += $"\r\nregion: **{response.Value.attributes.REGION_s}**";
                    ingameList += $"\r\nsub-region: **{response.Value.attributes.SUBREGION_s}**";
                    //ingameList += $"\r\nserverIP:{response.Value.serverAddress.Substring(0, 3)}..";
                    //ingameList += $"\r\nserverPORT:{response.Value.serverPort.ToString().Substring(0, 2)}..";
                    //ingameList += $"\r\nzoneThemeClass:{response.Value.attributes.ZONEINSTANCEID_s}";
                    await ReplyEmbedAsync(ingameList);
                }
                else
                {
                    await ReplyEmbedErrorAsync($"game not found for **{userName}**");
                }
            });
        }

        public async Task _UserInfo(string mention)
        {
            try
            {
                if (!HasPVEMode())
                    return;
                Regex r = new Regex("<@!?(\\d+)>", RegexOptions.Singleline);
                var m = r.Match(mention);
                if (m.Success)
                {
                    {
                        var uid = m.Groups[1].Value;
                        var user = Context.Repo.Db<FortniteUser>().GetById(uid);
                        if (user == null)
                            return;

                        SocketGuildUser duser = Context.Guild.GetUser(m.Groups[1].Value.ToUlong());
                        if (duser == null)
                            return;

                        EmbedBuilder embed = new EmbedBuilder();
                        embed.Author = GetVerificationAuthor();
                        embed.Description = "User Information";
                        double queueLenght = -1, IndexInQueue = -1;
                        double TotalSeconds = -1, IndexSeconds = -1;
                        bool InQueue = user.NameStates.Any(p => p.InQueue);
                        if (InQueue && false)
                        {
                            var users = Context.Repo.Db<NameState>().Where(p => p.InQueue && !p.LockName).OrderBy(c => c.FortniteUser.LastUpDateTime);
                            queueLenght = users.Count();
                            int index = users.ToList().FindIndex(f => f.FortniteUserId == user.Id) + 1;
                            if (index != -1)
                            {
                                IndexInQueue = index;
                                double Totalcross = queueLenght;
                                var botConfig = Context.Redis.JsonGet<BotConfig>();
                                if (queueLenght > botConfig.Vars.QueueLength)
                                {
                                    Totalcross = queueLenght / botConfig.Vars.QueueLength;
                                }
                                TotalSeconds = Totalcross * 75;
                                if (index > botConfig.Vars.QueueLength)
                                {
                                    double Indexcross = index / botConfig.Vars.QueueLength + 0.5;
                                    IndexSeconds = Indexcross * 75;
                                }
                                else
                                {
                                    IndexSeconds = 1.5 * 75;
                                }
                            }
                        }
                        var pveProfile = Context.Repo.Db<FortnitePVEProfile>().Where(f => f.EpicId == user.EpicId).FirstOrDefault();
                        var uname = pveProfile.PlayerName;
                        if (string.IsNullOrWhiteSpace(uname))
                            uname = "?";
                        embed.AddField("PlayerName", $"{uname}", true);
                        if (pveProfile.AccountPowerLevel > 0)
                        {
                            embed.AddField("Energy:zap:", $"{pveProfile.GetIntegerPower}", true);
                        }
                        embed.AddField("HasValidName", $"{user.IsValidName}", true);
                        embed.Footer = new EmbedFooterBuilder()
                        {
                            Text = $"Last update, {user.LastUpDateTime.ToString("HH:mm:ss")} (GMT +0)"
                        };
                        if (user.IsValidName)
                            embed.Color = Color.Green;
                        else if (InQueue)
                            embed.Color = Color.Teal;
                        else
                            embed.Color = Color.DarkRed;
                        await ReplyEmbedAsync(embed);
                    }
                }
            }
            catch (Exception e)
            {
                await ReplyEmbedErrorAsync($"{e.Message}\n");
            }
            return;
        }

        public Task AllPriorities(bool onlyForAone = false)
        {
            return Task.Run(async () =>
            {
                string tableName = "Priority Table";
                string pList = "";
                var lst = Context.Repo.Priority.Priorities;
                int counter = 0;
                foreach (var p in lst)
                {
                    try
                    {
                        var ts = p.Deadline - DateTimeOffset.UtcNow;
                        bool expired = ts.TotalMilliseconds < 0;
                        string dline = "";
                        string name = "";
                        if (expired)
                        {
                            dline = "**Expired**";
                        }
                        else
                        {
                            DateTime dtx = new DateTime();
                            dtx = dtx.Add(ts);
                            dline = $"Expires: **{$"{dtx.Year - 1}Y {dtx.Month - 1}M {dtx.Day}d {dtx.Hour}h"}**";
                        }

                        if (p.State == PriorityState.User)
                        {
                            var usr = Context.DiscordRestApi.GetApi.GetUserAsync(p.GetUlongId()).Result;
                            name = $"**{usr?.Username}#{usr?.Discriminator}**";
                        }
                        else
                        {
                            var g = Context.DiscordRestApi.GetApi.GetGuildAsync(p.GetUlongId()).Result;
                            if (g != null)
                            {
                                name = $"**{g.Name}**";
                            }
                            else
                            {
                                name = $"**unknown guild**";
                            }
                        }
                        pList += $"<@{p.Id.Substring(1)}>,[{dline}] [Name:'{name}']\n";
                        if (counter == 10)
                        {
                            counter = 0;
                            await ReplyEmbedAsync(pList, tableName);
                            pList = "";
                        }
                        else
                        {
                            counter++;
                        }
                    }

                    catch (Exception)

                    {
                        continue;
                    }
                }
                if (pList.Length > 0)
                {
                    await ReplyEmbedAsync(pList, tableName);
                }
            });
        }

        public Task GameMode(ulong userId, string gameMode)
        {
            return Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(gameMode) && (gameMode.Equals("pvp", StringComparison.InvariantCultureIgnoreCase) || gameMode.Equals("pve", StringComparison.InvariantCultureIgnoreCase)))
                {
                    var cUser = Context.Guild.GetUser(userId);
                    if (cUser.NotNull())
                    {
                        FortniteUser cDiscordUser = Context.Repo.Db<FortniteUser>().GetById(cUser.Id.ToString());
                        if (cDiscordUser.NotNull())
                        {
                            if (cDiscordUser.IsValidName)
                            {
                                var uid = cUser.Id.ToString();
                                var guid = Context.Guild.Id.ToString();
                                bool IsSuccess = false;
                                bool IsSpamCommand = false;

                                NameState ns = Context.Repo.Db<NameState>().Where(p => p.FortniteUserId == uid && p.DiscordServerId == guid).FirstOrDefault();
                                if (ns == null)
                                {
                                    ns = await Context.Repo.User.AddOrGetNameStateAsync(cDiscordUser, guid);
                                }
                                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 10, 0), cDiscordUser, cUser, ns);
                                if (pstate.Value == null)
                                    return;

                                if (pstate.Value.State == PriorityState.Normal)
                                {
                                    IsSpamCommand = (DateTimeOffset.UtcNow - cDiscordUser.LastUpDateTime).TotalMinutes <= 30;
                                }
                                if (gameMode.Equals("pvp", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (cDiscordUser.GameUserMode != GameUserMode.PVP_WIN_ALL)
                                    {
                                        cDiscordUser.GameUserMode = GameUserMode.PVP_WIN_ALL;
                                        var pvpProfile = Context.Repo.Db<FortnitePVPProfile>().Where(f => f.EpicId == cDiscordUser.EpicId).FirstOrDefault();
                                        if (pvpProfile.PvpCurrentModeWins(cDiscordUser.GameUserMode) <= 1)
                                        {
                                            IsSpamCommand = false;//there is no pvp information, so get data from epic games
                                        }
                                        IsSuccess = await SetNewNameViaMode<FortnitePVPProfile>(cUser, cDiscordUser, Context.Message, pvpProfile, IsSpamCommand);
                                    }
                                    else
                                        IsSuccess = true;
                                }
                                else if (gameMode.Equals("pve", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (cDiscordUser.GameUserMode != GameUserMode.PVE)
                                    {
                                        cDiscordUser.GameUserMode = GameUserMode.PVE;
                                        var pveProfile = Context.Repo.Db<FortnitePVEProfile>().Where(f => f.EpicId == cDiscordUser.EpicId).FirstOrDefault();
                                        if (pveProfile.AccountPowerLevel <= 1)
                                        {
                                            IsSpamCommand = false;//there is no pve information, so get data from epic games
                                        }
                                        IsSuccess = await SetNewNameViaMode<FortnitePVEProfile>(cUser, cDiscordUser, Context.Message, pveProfile, IsSpamCommand);
                                    }
                                    else
                                        IsSuccess = true;
                                }
                                else
                                {
                                    return;
                                }
                                if (IsSuccess)
                                {
                                    await Context.Message.SetSuccessAsync();
                                }
                            }
                            else
                            {
                                await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                            }
                        }
                    }
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.IncorrectGameMode, GetLanguage()));
                }
            });
        }

        public async Task PveProfile(bool myInfo, string userName = null)
        {
            if (!HasPVEMode())
                return;
            if (myInfo || !string.IsNullOrWhiteSpace(userName))
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;

                string PlayerName = "";
                if (myInfo)
                {
                    PlayerName = Context.PlayerName;
                }
                else
                {
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    PlayerName = userName;
                }
                KeyValuePair<string, IQueryProfile> profile = await Api.GetPVEProfileByName(PlayerName);
                if (profile.Value != null)
                {
                    var svvrs = await profile.Value.GetSurvivors();
                    var svvrsResource = await svvrs.CalcSurvivorFORTs();
                    var resources = await profile.Value.CalcResearchFORTs();
                    var AccountPowerLevel = await SurvivorStaticData.CalcEnergyByFORT(svvrsResource + resources);
                    var prfl = profile.Value.profileChanges.First().profile;
                    var achievement_explorezones = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_explorezones");
                    var achievement_playwithothers = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_playwithothers");
                    var achievement_killmistmonsters = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_killmistmonsters");
                    var achievement_loottreasurechests = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_loottreasurechests");
                    var achievement_savesurvivors = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_savesurvivors");
                    var achievement_buildstructures = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_buildstructures");
                    var achievement_destroygnomes = prfl.items.FirstOrDefault(f => f.Value.templateId == "Quest:achievement_destroygnomes");
                    var collectionresource_nodegatetoken01 = prfl.items.FirstOrDefault(f => f.Value.templateId == "Token:collectionresource_nodegatetoken01");
                    var technology = prfl.items.FirstOrDefault(f => f.Value.templateId == "Stat:technology");
                    var resistance = prfl.items.FirstOrDefault(f => f.Value.templateId == "Stat:resistance");
                    var fortitude = prfl.items.FirstOrDefault(f => f.Value.templateId == "Stat:fortitude");
                    var offense = prfl.items.FirstOrDefault(f => f.Value.templateId == "Stat:offense");
                    StatAttribute stats = prfl.stats["attributes"];
                    EmbedBuilder emb = new EmbedBuilder();

                    emb.AddField(ToTranslate(BotTranslationString.P_PowerLevel), $":zap:{AccountPowerLevel}", true);
                    emb.AddField(ToTranslate(BotTranslationString.P_ChestOpened), achievement_loottreasurechests.Value?.attributes.completion_interact_treasurechest.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_collectionLevel), (stats.collection_book.maxBookXpLevelAchieved + 1).ToString("#,##0.##"), true);
                    if (stats.rewards_claimed_post_max_level.HasValue)
                    {
                        emb.AddField(ToTranslate(BotTranslationString.P_CommanderLevel), (stats.level + stats.rewards_claimed_post_max_level.Value).ToString("#,##0.##"), true);
                    }
                    else
                    {
                        emb.AddField(ToTranslate(BotTranslationString.P_CommanderLevel), stats?.level.ToString("#,##0.##"), true);
                    }
                    emb.AddField(ToTranslate(BotTranslationString.P_DaysLoggedin), stats?.daily_rewards.totalDaysLoggedIn.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_GnomDestroy), achievement_destroygnomes.Value.attributes?.completion_destroy_gnome.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_MistMonsterKilled), achievement_killmistmonsters.Value?.attributes.completion_kill_husk_smasher.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_PlayWithOthers), achievement_playwithothers.Value?.attributes.completion_quick_complete.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_StructuresBuilt), achievement_buildstructures.Value?.attributes.completion_build_any_structure.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_SurvivorSaved), achievement_savesurvivors.Value?.attributes.completion_questcollect_survivoritemdata.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_ZoneExplored), achievement_explorezones.Value.attributes.completion_complete_exploration_1.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_StoredResearch), collectionresource_nodegatetoken01.Value?.quantity.ToString("#,##0.##"), true);
                    emb.AddField(ToTranslate(BotTranslationString.P_FortitudeResearch), fortitude.Value?.quantity, true);
                    emb.AddField(ToTranslate(BotTranslationString.P_OffenseResearch), offense.Value?.quantity, true);
                    emb.AddField(ToTranslate(BotTranslationString.P_ResistanceResearch), resistance.Value?.quantity, true);
                    emb.AddField(ToTranslate(BotTranslationString.P_TechnologyResearch), technology.Value?.quantity, true);
                    await ReplyEmbedAsync(ToTranslate(BotTranslationString.P_Title, profile.Key), "", emb.Fields, emb.Footer, Color.LightOrange, 0,
                        new KeyValuePair<bool, PriorityTable>(false, null));
                    if (pstate.Value == null)
                    {
                        Context.Repo.StoredProcedure.SP_User_LastUpdateTime(Context.DiscordUser.Id);
                    }
                }
                else
                {
                    await Context.Message.SetErrorAsync();
                }
            }
        }

        public async Task ResourcesList(bool myInfo, string userName = null)
        {
            if (!HasPVEMode())
                return;
            if (myInfo || !string.IsNullOrWhiteSpace(userName))
            {
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 7, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;

                string PlayerName = "";
                if (myInfo)
                {
                    PlayerName = Context.PlayerName;
                }
                else
                {
                    if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                    {
                        await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                        return;
                    }
                    PlayerName = userName;
                }

                var profile = await Api.GetPVEProfileByName(PlayerName);
                if (profile.Value != null)
                {
                    var resources = await profile.Value.GetResources(GetLanguage().ToString());
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Author = GetVerificationAuthor(),
                        Color = Color.Green
                    };
                    for (int i = 0; i < resources.Count; i++)
                    {
                        using (var resource = resources[i])
                        {
                            if (string.IsNullOrWhiteSpace(resource.name))
                            {
                                Log.Warning("Undefined name of resourceId {ResourceId}, please define it", resource.id);
                                continue;
                            }
                            if (resource.id.Contains("eventcurrency_") && resource.id != "eventcurrency_scaling")
                                continue;
                            embed.AddField($"{resource.name}", $"<:{resource.id}:{resource.img}> **{resource.quantity:#,##0.##}**", true);
                        }
                    }
                    await ReplyEmbedAsync(null, ToTranslate(BotTranslationString.PR_Title, profile.Key), embed.Fields, null, embed.Color.Value, 0, pstate);
                    if (pstate.Value == null)
                        Context.Repo.StoredProcedure.SP_User_LastUpdateTime(Context.DiscordUser.Id);
                }
                else
                {
                    await Context.Message.SetErrorAsync();
                }
            }
        }
    }
}