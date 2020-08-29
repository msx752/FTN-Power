using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Core.ModifiedModels;
using Fortnite.Localization;
using Fortnite.Model.Enums;
using Fortnite.Static;
using FTNPower.Core.DiscordContext;
using FTNPower.Core.DomainService;
using FTNPower.Core.Interfaces;
using FTNPower.Model;
using FTNPower.Model.Tables;
using FTNPower.Model.Tables.StoredProcedures;
using Global;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FTNPower.Core
{
    public static class Utils
    {
        public static readonly Emoji EmojiLock = new Emoji("🔒");
        public static readonly Emoji EmojiError = new Emoji("❌");
        public static readonly Emoji EmojiConsolePlayer = new Emoji("🎮");
        public static readonly Emoji EmojiQuestionMark = new Emoji("❓");

        public static readonly Emoji EmojiSuccess = new Emoji("✅");

        public static RequestOptions RequestOption { get; } = new RequestOptions()
        {
            RetryMode = RetryMode.AlwaysRetry,
            Timeout = (int)new TimeSpan(0, 0, 30).TotalMilliseconds,
        };
        public static bool ParseId(this string mention, out ulong uid)
        {
            Regex r = new Regex("<@!?(\\d+)>", RegexOptions.Singleline);
            var m = r.Match(mention);
            if (m.Success)
            {
                uid = m.Groups[1].Value.ToUlong();
                return true;
            }
            else
            {
                uid = 0;
                return false;
            }
        }
        public static async Task SetLockAsync(this IUserMessage msg)
        {
            await msg.AddReactionAsync(EmojiLock, Core.Utils.RequestOption);
        }
        public static async Task SetQuestionMarkAsync(this IUserMessage msg)
        {
            await msg.AddReactionAsync(EmojiQuestionMark, RequestOption);
        }

        public static async Task SetSuccessAsync(this IUserMessage msg)
        {
            await msg.AddReactionAsync(EmojiSuccess, RequestOption);
        }
        public static async Task SetErrorAsync(this IUserMessage msg)
        {
            await msg.AddReactionAsync(EmojiError, Core.Utils.RequestOption);
        }
        public static async Task SetConsolePlayerAsync(this IUserMessage msg)
        {
            await msg.AddReactionAsync(EmojiConsolePlayer, Core.Utils.RequestOption);
        }

        public static bool IsAvailable(this IUser user, bool ignoreInvsible = true)
        {
            if (ignoreInvsible)
                return user.Status != UserStatus.Offline;
            else
                return user.Status != UserStatus.Offline && user.Status != UserStatus.Invisible;
        }
        public static bool IsFortnitePlayer(this IActivity activity, bool IsBR = true)
        {
            var a = activity?.GetFAcvitity();
            if (a == null)
                return false;
            return a.Name == "Fortnite"
                   && a.State != null
                   && a.Party != null
                   && a.Type == ActivityType.Playing
                   && a.Details != null
                   && a.Timestamps != null
                   && a.Details.NotNull();
        }
        public static bool NotNull(this object u)
        {
            return u != null;
        }
        public static RichGame GetFAcvitity(this IActivity activity, bool IsBR = true)
        {
            if (activity == null)
                return null;
            if ((typeof(RichGame) != activity.GetType()))
                return null;
            return (RichGame)activity;
        }
        public static bool IsServerOwner(this SocketGuildUser user)
        {
            return user.Guild.OwnerId == user.Id;
        }
        public static bool ValidName(this string newName)
        {
            return (newName.IndexOf(":") > -1 ||
                   newName.IndexOf("?") > -1 ||
                   newName.IndexOf("=") > -1 ||
                   newName.IndexOf(",") > -1 ||
                   newName.IndexOf("\\") > -1 ||
                   newName.IndexOf("/") > -1 ||
                    newName.IndexOf("{") > -1 ||
                    newName.IndexOf("}") > -1 ||
                   string.IsNullOrWhiteSpace(newName) ||
                   newName?.Length > 50) == false;
        }
        public static bool IsServerOwner(this IGuildUser user)
        {
            return user.Guild.OwnerId == user.Id;
        }

        public static void PushDiscordById(this FTNPower.Core.Interfaces.IRedisService redisService, IFTNPowerRepository repository, string queueName, string discordId)
        {
            var ReadyToUpdateList = repository.Db<NameState>()
                                              .All()
                                              .AsNoTracking()
                                              .Where(f => f.LockName == false &&
                                                          f.InQueue == false &&
                                                          f.DiscordServerId == discordId &&
                                                          f.FortniteUser.IsValidName &&
                                                          f.FortniteUser.EpicId != null)
                                             .Select(c => new ReadyToUpdate()
                                             {
                                                 DiscordServerId = c.DiscordServerId,
                                                 EpicId = c.FortniteUser.EpicId,
                                                 FortniteUserId = c.FortniteUserId,
                                                 GameUserMode = c.FortniteUser.GameUserMode,
                                                 NameTag = c.FortniteUser.NameTag
                                             });
            
            List<Task> tList = new List<Task>();
            foreach (var readyToUpdate in ReadyToUpdateList)
            {
                tList.Add(Task.Run(() =>
                {
                    redisService.ListLeftPush(queueName, readyToUpdate, flags: CommandFlags.FireAndForget);
                }));
            }
            Task.WaitAll(tList.ToArray());
            tList.Clear();
        }
        public async static Task<bool> CheckWebhookPermission(this BaseModule module, SocketGuild selectedGuild, IReadOnlyCollection<RestWebhook> webhooks, string webhookName, string WebhookPurpose)
        {
            var webhook = webhooks.FirstOrDefault(p => p.Name == webhookName);
            if (webhook == null)
            {
                if (webhookName == "Fortnite Power Missions")
                {
                    webhook = webhooks.FirstOrDefault(p => p.Name == "Fortnite Power msa1");
                    if (webhook == null)
                    {
                        await module.ReplyEmbedErrorAsync($"Webhook is not found for '**{WebhookPurpose}**', please specify webhook with name **{webhookName}** then give permission to **FTN Power** for that channel");
                        return false;
                    }
                }
                else
                {
                    await module.ReplyEmbedErrorAsync($"Webhook is not found for '**{WebhookPurpose}**', please specify webhook with name **{webhookName}** then give permission to **FTN Power** for that channel");
                    return false;
                }
            }
            var channel = selectedGuild.GetTextChannel(webhook.ChannelId);
            var perms = selectedGuild.CurrentUser.GetPermissions(channel);
            if (!perms.ManageWebhooks || !perms.EmbedLinks || !perms.SendMessages || !perms.UseExternalEmojis)
            {
                await module.ReplyEmbedErrorAsync($"**FTN Power** has no access to channel:[<#{webhook.ChannelId}>] for **{WebhookPurpose}**\n" +
                                           $"ManageWebhooks{perms.ManageWebhooks}\n" +
                                           $"EmbedLinks{perms.EmbedLinks}\n" +
                                           $"SendMessages{perms.SendMessages}\n" +
                                           $"UseExternalEmojis{perms.UseExternalEmojis}\n");
                return false;
            }
            else
            {
                await module.ReplyEmbedAsync($"Notification for '**{WebhookPurpose}**' is **READY** channel:[ <#{webhook.ChannelId}> ]");
                return true;
            }
        }

        public static string ToQueryString<T>(this Expression<Func<T, bool>> exp) where T : class
        {
            if (exp == null) return null;
            var strResult = typeof(Expression).GetProperty("DebugView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(exp.Body) as string;
            var m = new Regex($@"(\((\w+)(\.\w+)?\)\${exp.Parameters.First().Name}\.)", RegexOptions.Singleline).Match(strResult);
            if (m.Success) foreach (Capture item in m.Captures) strResult = strResult.Replace(item.Value, "");
            strResult = strResult.Replace(@$"${exp.Parameters.First().Name}.", $"");
            return strResult;
        }

        public static Task<Dictionary<MapRoles, IRole>> GetMapRolesAsync(this IGuild guild, GuildLanguage lang)
        {
            return Task.Run(() =>
            {
                var Translate = DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
                var rl = guild.Roles.Where(p => p.Name == Translate.GetBotTranslation(BotTranslationString.Stonewood, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.Plankerton, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.CannyValley, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.TwinePeaks, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.noname, lang))
                    .OrderBy(p => p.Name);

                var scrl = new Dictionary<MapRoles, IRole>();
                foreach (var r in rl)
                {
                    if (r.Name == Translate.GetBotTranslation(BotTranslationString.Stonewood, lang))
                    {
                        if (!scrl.ContainsKey(MapRoles.Stonewood))
                            scrl.Add(MapRoles.Stonewood, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.Plankerton, lang))
                    {
                        if (!scrl.ContainsKey(MapRoles.Plankerton))
                            scrl.Add(MapRoles.Plankerton, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.CannyValley, lang))
                    {
                        if (!scrl.ContainsKey(MapRoles.CannyValley))
                            scrl.Add(MapRoles.CannyValley, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.TwinePeaks, lang))
                    {
                        if (!scrl.ContainsKey(MapRoles.TwinePeaks))
                            scrl.Add(MapRoles.TwinePeaks, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.noname, lang))
                    {
                        if (!scrl.ContainsKey(MapRoles.noname))
                            scrl.Add(MapRoles.noname, r);
                    }
                }
                return scrl;
            });
        }
        public static KeyValuePair<bool, string> HasMentionedId(this string text)
        {
            if (text == null)
                text = "x";
            Regex r = new Regex("<@!?(\\d+)>", RegexOptions.Singleline);
            var m = r.Match(text);
            return new KeyValuePair<bool, string>(m.Success, m.Groups[1].Value);
        }

        public static Task<Dictionary<MapRoles, IRole>> GetMissingMapRolesAsync(this IGuild guild, GuildLanguage lang, out List<ReorderRoleProperties> reorder)
        {
            reorder = new List<ReorderRoleProperties>();
            try
            {
                var list = reorder;
                var t = Task.Run(() =>
                {
                    var curUser = guild.GetCurrentUserAsync().Result;

                    var userRoles = guild.Roles.Where(f => curUser.RoleIds.Contains(f.Id));

                    var positionOfMinManagedRol = userRoles
                    .Where(s => s.Permissions.ManageRoles)
                    .OrderByDescending(s => s.Position)
                    .FirstOrDefault();

                    if (positionOfMinManagedRol == null)
                    {
                        return null;
                    }
                    List<Task> taskList = new List<Task>();
                    Dictionary<MapRoles, IRole> roles = new Dictionary<MapRoles, IRole>();

                    var dicRoles = FTNPower.Core.Utils.GetMapRolesAsync(guild, lang).Result;
                    var Translate = DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
                    var lmap4 = Translate.GetBotTranslation(BotTranslationString.TwinePeaks, lang);
                    var lmap3 = Translate.GetBotTranslation(BotTranslationString.CannyValley, lang);
                    var lmap2 = Translate.GetBotTranslation(BotTranslationString.Plankerton, lang);
                    var lmap1 = Translate.GetBotTranslation(BotTranslationString.Stonewood, lang);
                    var lmap0 = Translate.GetBotTranslation(BotTranslationString.noname, lang);

                    var q1 = dicRoles.FirstOrDefault(s =>
                        s.Value.Name == lmap4);
                    if (q1.Value == null)
                    {
                        taskList.Add(guild
                           .CreateRoleAsync(lmap4, GuildPermissions.None, new Color(0xff, 0xa3, 0x00), true,
                               Core.Utils.RequestOption)
                           .ContinueWith(
                               (p) =>
                               {
                                   try
                                   {
                                       roles.Add(MapRoles.TwinePeaks, p.Result);
                                       list.Add(new ReorderRoleProperties(p.Result.Id, positionOfMinManagedRol.Position));
                                   }
                                   catch
                                   {
                                   }
                               }));
                    }
                    else
                    {
                        roles.Add(MapRoles.TwinePeaks, q1.Value);
                        list.Add(new ReorderRoleProperties(q1.Value.Id, positionOfMinManagedRol.Position));
                    }

                    var q2 = dicRoles.FirstOrDefault(s => s.Value.Name == lmap3);
                    if (q2.Value == null)
                    {
                        taskList.Add(guild
                            .CreateRoleAsync(lmap3, GuildPermissions.None, new Color(0xa8, 0x58, 0xf3), true,
                                Core.Utils.RequestOption)
                            .ContinueWith(
                                (o) =>
                                {
                                    try
                                    {
                                        roles.Add(MapRoles.CannyValley, o.Result);
                                        list.Add(new ReorderRoleProperties(o.Result.Id, positionOfMinManagedRol.Position));
                                    }
                                    catch
                                    {
                                    }
                                }));
                    }
                    else
                    {
                        roles.Add(MapRoles.CannyValley, q2.Value);
                        list.Add(new ReorderRoleProperties(q2.Value.Id, positionOfMinManagedRol.Position));
                    }

                    var q3 = dicRoles.FirstOrDefault(s =>
                        s.Value.Name == lmap2);
                    if (q3.Value == null)
                    {
                        taskList.Add(guild
                            .CreateRoleAsync(lmap2, GuildPermissions.None, new Color(0x06, 0x6f, 0xf5), true,
                                Core.Utils.RequestOption)
                            .ContinueWith(
                                (o) =>
                                {
                                    try
                                    {
                                        roles.Add(MapRoles.Plankerton, o.Result);
                                        list.Add(new ReorderRoleProperties(o.Result.Id, positionOfMinManagedRol.Position));
                                    }
                                    catch
                                    {
                                    }
                                }));
                    }
                    else
                    {
                        roles.Add(MapRoles.Plankerton, q3.Value);
                        list.Add(new ReorderRoleProperties(q3.Value.Id, positionOfMinManagedRol.Position));
                    }

                    var q4 = dicRoles.FirstOrDefault(s =>
                        s.Value.Name == lmap1);
                    if (q4.Value == null)
                    {
                        taskList.Add(guild
                            .CreateRoleAsync(lmap1, GuildPermissions.None, new Color(0x4f, 0xe9, 0x0a), true,
                                Core.Utils.RequestOption)
                            .ContinueWith(
                                (o) =>
                                {
                                    try
                                    {
                                        roles.Add(MapRoles.Stonewood, o.Result);
                                        list.Add(new ReorderRoleProperties(o.Result.Id, positionOfMinManagedRol.Position));
                                    }
                                    catch// (Exception e)
                                    {
                                    }
                                }));
                    }
                    else
                    {
                        roles.Add(MapRoles.Stonewood, q4.Value);
                        list.Add(new ReorderRoleProperties(q4.Value.Id, positionOfMinManagedRol.Position));
                    }

                    var q5 = dicRoles.FirstOrDefault(s =>
                        s.Value.Name == lmap0);
                    if (q5.Value == null)
                    {
                        taskList.Add(guild
                            .CreateRoleAsync(lmap0, GuildPermissions.None, new Color(0, 0, 0), false, Core.Utils.RequestOption)
                            .ContinueWith((o) =>
                            {
                                try
                                {
                                    roles.Add(MapRoles.noname, o.Result);
                                    list.Add(new ReorderRoleProperties(o.Result.Id, positionOfMinManagedRol.Position));
                                }
                                catch
                                {
                                }
                            }));
                    }
                    else
                    {
                        roles.Add(MapRoles.noname, q5.Value);
                        list.Add(new ReorderRoleProperties(q5.Value.Id, positionOfMinManagedRol.Position));
                    }

                    if (taskList.Count > 0)
                        Task.WaitAll(taskList.ToArray());
                    return roles;
                });
                t.Wait();
                reorder = list;
                return t;
            }
            catch 
            {
                return null;
            }
        }

        public static Task SetMapRolePermissionsAsync(this IRole mapRole)
        {
            return Task.Run(() =>
            {
                try
                {
                    mapRole.ModifyAsync((r) =>
                    {
                        r.Permissions = new GuildPermissions(createInstantInvite: false,
                            mentionEveryone: false,
                            connect: true,
                            useVoiceActivation: true,
                            speak: false);
                        r.Mentionable = true;
                    }, Core.Utils.RequestOption);
                }
                catch (AggregateException e)
                {
                    Global.Log.Warning("{lt}: insufficient permission to change:{RoleName}, Guild Id:{GuildId}, Guild Name:{GuildName} and it's owner {GuildOwner}", "SetMapRolePermissions", mapRole.Name, mapRole.Guild.Id, mapRole.Guild.Name, mapRole.Guild.OwnerId);
                   
                }
                catch (Exception e)
                {

                }
            });
        }


        public static Embed ToWebhookEmbed(this List<DailyLlama> mission)
        {
            EmbedBuilder embed = new EmbedBuilder();
            var vbuck = SurvivorStaticData.AssetBot2["currency_mtxswap"].EmojiId;
            var tickets = SurvivorStaticData.AssetBot2["campaign_event_currency"].EmojiId;

            foreach (var m in mission)
            {
                var priceIcon = vbuck;
                if (m.GameItem)
                    priceIcon = tickets;
                //
                embed.AddField($"{m.Title}", $"**{(m.Price == 0 ? "FREE" : m.Price.ToString())}** {(m.Price == 0 ? "" : $"<:{priceIcon}:{priceIcon}>")} [ **Limit:** {m.Amount} ]\n{m.Description}\n{SurvivorStaticData.InvinsibleImage()}");
            }
            embed.Color = new Color(230, 176, 170);
            return embed.Build();
        }

        public static Embed ToWebhookEmbed(this IMissionX mission, GuildLanguage language = GuildLanguage.EN)
        {
            int xy = 0;
            EmbedBuilder embed = new EmbedBuilder();
            try
            {
                if (mission.MissionNameInfo.Name == "Launch the Rocket")
                {
                    return null;
                }
                string GroupMission = $"";
                if (mission.IsGroupMission)
                {
                    GroupMission = $"{SurvivorStaticData.Get4xGroupMissionImage()}";
                }

                bool firstAlertSet = false;

                var time = mission.availableUntil.ToStringHMS();
                var Translate = DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
                string availableTime = Translate.GetBotTranslation(BotTranslationString.MA_ExpreIn, language, time);
                List<string> mapRewards = new List<string>();
                List<string> alertRewards = new List<string>();
                string alertNameStr = "";
                foreach (var item in mission.Items)
                {
                    if (item.AlertReward && firstAlertSet == false)
                    {
                        firstAlertSet = true;

                        alertNameStr = mission.MissionCategory.ToDescriptionString();
                    }
                    string amount = "";
                    if (item.AlertReward && item.quantity > 1)
                        amount = $"(**x{item.quantity:#,##0.##}**)";
                    else
                    {
                        if (item.AlertReward && item.quantity == 4)
                            amount = $"(**x4**)";
                        else if (item.AlertReward && item.quantity != 1)
                            amount = $"(**x{item.quantity}**)";
                    }
                    var itmInfo = item.GetRealItemName(false, language.ToString());
                    if (item.AlertReward == false)
                    {
                        mapRewards.Add($"<:{itmInfo.Key}:{itmInfo.Key}> *{itmInfo.Value}* {amount}");
                    }
                    else
                    {
                        alertRewards.Add($"<:{itmInfo.Key}:{itmInfo.Key}> *{itmInfo.Value}* {amount}");
                    }
                }
                string mapLevel = mission.MissionLevel.ToString();
                embed.Title = $"{mapLevel}:zap: {mission.MissionNameInfo.Name} {GroupMission} <:{mission.MissionNameInfo.EmojiId}:{mission.MissionNameInfo.EmojiId}>";
                embed.Description = $"**{Translate.GetBotTranslation(mission.WorldName.ToString().Replace("_", ""), language)}**";
                embed.WithFooter($"{availableTime}");
                embed.AddField(Translate.GetBotTranslation(BotTranslationString.MA_Rewards, language), string.Join("\n", mapRewards.ToArray()), true);
                embed.AddField($"**{alertNameStr}**", string.Join("\n", alertRewards.ToArray()), true);
                string positive_mutations = "";
                string negative_mutations = "";
                int p = 0, n = 0;
                foreach (var mutation in mission.Modifiers)
                {
                    string mId = Fortnite.Static.Utils.GetImage(mutation.AssetId);
                    if (mutation.IsPositiveMutation.Value)
                    {
                        n++;
                        positive_mutations += $"<:{mId}:{mId}> ";
                        if (n % 5 == 0)
                        {
                            positive_mutations += "\n";
                        }
                    }
                    else
                    {
                        p++;
                        negative_mutations += $"<:{mId}:{mId}> ";
                        if (p % 5 == 0)
                        {
                            negative_mutations += "\n";
                        }
                    }
                }

                if (positive_mutations.Length != 0 && negative_mutations.Length != 0)
                {
                    embed.AddField(Translate.GetBotTranslation(BotTranslationString.MA_NegativeMutation, language), negative_mutations, false);
                    embed.AddField(Translate.GetBotTranslation(BotTranslationString.MA_PositiveMutation, language), positive_mutations, true);
                }
                else if (negative_mutations.Length == 0 || positive_mutations.Length != 0)
                {
                    embed.AddField(Translate.GetBotTranslation(BotTranslationString.MA_PositiveMutation, language), positive_mutations, false);
                }
                else if (negative_mutations.Length != 0 || positive_mutations.Length == 0)
                {
                    embed.AddField(Translate.GetBotTranslation(BotTranslationString.MA_NegativeMutation, language), negative_mutations, false);
                }
                if (mission.AnyEpic())
                {
                    embed.Color = new Color(187, 143, 206);
                }

                if (mission.AnyLegendary())
                {
                    embed.Color = new Color(248, 196, 113);
                }

                if (mission.HasVBuck())
                {
                    embed.Color = new Color(133, 193, 233);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("-------------------------------------------");

                Console.WriteLine(JsonConvert.SerializeObject(mission));

                Console.WriteLine("-------------------------------------------");
            }
            return embed.Build();
        }

        public static async void DrawMissions(this BaseModule module, KeyValuePair<bool, PriorityTable> pstate, EmbedBuilder embed, IEnumerable<IMissionX> missions, string search)
        {
            int count = 0;
            foreach (var mission in missions)
            {
                if (mission.MissionNameInfo.Name == "Launch the Rocket")
                {
                    continue;
                }
                count++;
                string GroupMission = $"";
                if (mission.IsGroupMission)
                {
                    GroupMission = $"x4";
                }
                string itemList = "";
                bool firstAlertSet = false;

                string time = "";
                var fraction = mission.availableUntil - DateTimeOffset.UtcNow;
                if ((int)(fraction).TotalHours > 0)
                {
                    time = $"{(int)(fraction).TotalHours}h{(int)(fraction).Minutes }m";
                }
                else if ((int)(fraction).Minutes > 0)
                {
                    time = $"{(int)(fraction).Minutes}m";
                }

                itemList += module.ToTranslate(BotTranslationString.MA_Available, module.GetLanguage(), time);

                foreach (var item in mission.Items)
                {
                    if (item.GetRealItemName().Key == "0")
                    {
                        continue;
                    }

                    if (item.AlertReward && firstAlertSet == false)
                    {
                        firstAlertSet = true;
                        var alertNameStr = mission.MissionCategory.ToDescriptionString();
                        itemList += $"**{alertNameStr}:**\n";
                    }
                    string amount = "";
                    if (item.AlertReward && item.quantity > 1)
                        amount = $"(**x{item.quantity:#,##0.##}**)";
                    else
                    {
                        if (item.AlertReward == false && item.quantity == 4)
                            amount = $"(**x4**)";
                        else if (item.AlertReward == false && item.quantity != 1)
                            amount = $"(**x{item.quantity}**)";
                    }
                    var rname = item.GetRealItemName(true, module.GetLanguage().ToString());
                    itemList += $"<:{rname.Key}:{rname.Key}> *{rname.Value}* {amount}\n";
                }

                itemList += SurvivorStaticData.InvinsibleImage();
                string mapLevel = mission.MissionLevel.ToString();
                embed.AddField($":zap:{mapLevel}<:{mission.MissionNameInfo.EmojiId}:{mission.MissionNameInfo.EmojiId}>{GroupMission} {mission.WorldName.GetName()}", itemList, true);
            }
            if (count == 0)
            {
                await module.ReplyEmbedErrorAsync($"no result found {search}");
                return;
            }

            var frat = 3 - (count % 3);
            for (int i = 0; i < frat; i++)
            {
                embed.AddField(SurvivorStaticData.InvinsibleImage(), SurvivorStaticData.InvinsibleImage(), true);
            }
            await module.ReplyEmbedAsync(null, module.ToTranslate(BotTranslationString.MA_Title, module.GetLanguage()), embed.Fields,
                embed.Footer, embed.Color.Value, 0, pstate);
        }


        public static Task<Dictionary<MapRoles, IRole>> GetUserRolesAsync(this IGuildUser user, GuildLanguage lang)
        {
            if (user == null)
            {
                return Task.FromResult(new Dictionary<MapRoles, IRole>());
            }
            return Task.Run(() =>
            {
                var Translate = DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
                var curGuild = user.Guild;
                var guildRoles = curGuild.Roles.Where(p => p.Name == Translate.GetBotTranslation(BotTranslationString.Stonewood, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.Plankerton, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.CannyValley, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.TwinePeaks, lang) ||
                                                p.Name == Translate.GetBotTranslation(BotTranslationString.noname, lang));

                var rl = guildRoles.Where(f => user.RoleIds.Contains(f.Id));

                var scrl = new Dictionary<MapRoles, IRole>();
                foreach (var r in rl)
                {
                    if (r.Name == Translate.GetBotTranslation(BotTranslationString.Stonewood, lang))
                    {
                        scrl.Add(MapRoles.Stonewood, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.Plankerton, lang))
                    {
                        scrl.Add(MapRoles.Plankerton, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.CannyValley, lang))
                    {
                        scrl.Add(MapRoles.CannyValley, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.TwinePeaks, lang))
                    {
                        scrl.Add(MapRoles.TwinePeaks, r);
                    }
                    else if (r.Name == Translate.GetBotTranslation(BotTranslationString.noname, lang))
                    {
                        scrl.Add(MapRoles.noname, r);
                    }
                }
                return scrl;
            });
        }
    }
}
