using Discord;
using Discord.Rest;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Core.Services.Events;
using Fortnite.External.ServiceStore.Events;
using Fortnite.Model.Enums;
using FTNPower.Core.DomainService;
using FTNPower.Data;
using FTNPower.Model.Interfaces;
using FTNPower.Model.Tables;
using FTNPower.Model.WebsiteModels;
using Global;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Core
{
    public static class FortniteEventHandler
    {
        public static Task MissionCallback(IMissionServiceEventArgs e)
        {
            return Task.Run(() =>
            {
                using (var context = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
                {
                    try
                    {
                        var ptable = context.Priority.GetValidGuildPartners();
                        foreach (var priority in ptable)
                        {
                            try
                            {
                                var gid = priority.GetUlongId().ToString();
                                var gConfig = context.Guild.GetConfig(gid);
                                if (gConfig == null)
                                {
                                    gConfig = new GuildConfig();
                                    gConfig.Id = gid;
                                    context.Db<GuildConfig>().Add(gConfig);
                                    continue;
                                }

                                MissionWebhook(priority.GetUlongId(), gConfig, e.Missions).Wait();
                            }
                            catch (Exception exception1)
                            {
                                Log.Exception(exception1, exceptionNote: $"PriorityId is {priority.Id}");
                            }
                        }
                        Log.Information("Webhook: Mission is Finished");
                    }
                    catch (Exception exception2)
                    {
                        Log.Exception(exception2, Serilog.Events.LogEventLevel.Fatal);
                    }
                }
            });
        }

        public static Task MissionWebhook(ulong guid, GuildConfig gConfig, IEnumerable<IMissionX> missions)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (gConfig == null || gConfig.Admin.MissionStates.Active == false || missions.Count() == 0)
                        return;

                    var discordApi = DIManager.Services.GetRequiredService<IDiscordRestApi>(); ;
                    var guild = discordApi.GetApi.GetGuildAsync(guid).Result;
                    if (guild == null)
                        return;

                    if (gConfig.Admin.MissionStates.Contains(MissionType.All_TheSame_Channel))
                    {
                        var channelId = gConfig.Admin.MissionStates.Get(MissionType.All_TheSame_Channel).ChannelId;
                        if (string.IsNullOrWhiteSpace(channelId))
                            return;
                        var channel = guild.GetTextChannelAsync(ulong.Parse(channelId)).Result;
                        if (channel == null)
                            return;
                        var sndList1 = missions.Take(11);
                        foreach (var mission in sndList1)
                        {
                            var embedMsg = mission.ToWebhookEmbed(gConfig.Owner.DefaultLanguage);
                            if (embedMsg == null)
                                continue;

                            string mentionTag = "";
                            string vl = gConfig.Admin.MissionStates.Get(MissionType.All_TheSame_Channel).RoleId;
                            RestRole r0 = null;
                            if (vl.Length != 0 && (mission.HasVBuck() || mission.AnyMythic()))
                            {
                                mentionTag = $"<@&{vl}>";
                                r0 = guild.GetRole(ulong.Parse(vl));
                                r0.ModifyAsync((o) =>
                                {
                                    o.Mentionable = true;
                                }).Wait();
                            }
                            channel.SendMessageAsync(mentionTag, false, embedMsg, Utils.RequestOption).Wait();
                            r0?.ModifyAsync((o) =>
                            {
                                o.Mentionable = false;
                            }).Wait();
                        }
                    }
                    else
                    {
                        var grpChannels = gConfig.Admin.MissionStates.Channels.GroupBy(f => f.ChannelId);

                        foreach (var chnl in grpChannels)
                        {
                            var channelId = chnl.Key;
                            if (string.IsNullOrWhiteSpace(channelId))
                                continue;

                            var channel = guild.GetTextChannelAsync(ulong.Parse(channelId)).Result;
                            if (channel == null)
                                continue;
                            Dictionary<int, List<string>> crossCheck = new Dictionary<int, List<string>>();
                            foreach (var missionType in chnl)
                            {
                                bool hasRoleId = !string.IsNullOrWhiteSpace(missionType.RoleId);
                                if (missionType.MissionType == MissionType.VBuck)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasVBuck()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Mythic_Leader_Survivor)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.AnyMythic()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Any_Legendary)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.AnyLegendary()).OrderBy(f => f.OrderNumber).Take(5).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Legendary_Survivor)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasLegendarySurvivor()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Legendary_PerkUp)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasLegendaryPerkUp(WorldName.Twine_Peaks)).OrderBy(f => f.OrderNumber).Take(4).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Legendary_Hero)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasLegendaryHero()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Legendary_Defender)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasLegendaryDefender()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Legendary_Transform)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasLegendaryAnyTransform()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Epic_PerkUp)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasEpicPerkUp(WorldName.Twine_Peaks)).OrderBy(f => f.OrderNumber).Take(4).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Epic_Hero)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasEpicHero()).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Pure_Drop_Of_Rain)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.Has4xPureDropOfRain()).OrderByDescending(f => f.MissionLevel).Take(4).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Eye_Of_Storm)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.Has4xEyeOfStorm()).OrderByDescending(f => f.MissionLevel).Take(4).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Lightning_In_Bottle)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.Has4xLightningInABottle()).OrderByDescending(f => f.MissionLevel).Take(4).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Storm_Shard)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.Has4xStormShard()).OrderByDescending(f => f.MissionLevel).Take(4).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                                else if (missionType.MissionType == MissionType.Epic_Survivor)
                                {
                                    var mentionRoleId = $"<@&{missionType.RoleId}>";
                                    var hashl = missions.Where(f => f.HasEpicSurvivor()).OrderByDescending(f => f.MissionLevel).Take(6).Select(f => f.GetHashCode());
                                    crossCheck = GenCrossCheckTable(crossCheck, hashl, mentionRoleId, hasRoleId);
                                }
                            }
                            foreach (var key in crossCheck.Keys)
                            {
                                var mssn = missions.FirstOrDefault(f => f.GetHashCode() == key);
                                if (mssn == null)
                                    continue;
                                var embd = mssn.ToWebhookEmbed(gConfig.Owner.DefaultLanguage);
                                if (embd == null)
                                    continue;
                                string mentionTag = string.Empty;
                                List<Task> rls = new List<Task>();
                                if (crossCheck[key].Count > 0)
                                {
                                    mentionTag = string.Join(' ', crossCheck[key].ToArray());
                                    foreach (var item in crossCheck[key])
                                    {
                                        try
                                        {
                                            RestRole r1 = guild.GetRole(ulong.Parse(item.Substring(3, item.Length - 4)));
                                            rls.Add(r1.ModifyAsync((o) =>
                                            {
                                                o.Mentionable = true;
                                            }));
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                    Task.WaitAll(rls.ToArray());
                                    rls.Clear();
                                }
                                channel.SendMessageAsync(mentionTag, false, embd, Utils.RequestOption).Wait();
                                foreach (var item in crossCheck[key])
                                {
                                    try
                                    {
                                        RestRole r1 = guild.GetRole(ulong.Parse(item.Substring(3, item.Length - 4)));
                                        rls.Add(r1.ModifyAsync((o) =>
                                        {
                                            o.Mentionable = false;
                                        }));
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                                Task.WaitAll(rls.ToArray());
                            }
                        }
                    }
                }
                catch (Exception exception1)
                {
                    Log.Exception(exception1, Serilog.Events.LogEventLevel.Error);
                }
            });
        }

        private static Dictionary<int, List<string>> GenCrossCheckTable(Dictionary<int, List<string>> crossCheck, IEnumerable<int> hashl, string mentionRoleId, bool HasRoleId)
        {
            foreach (var hs in hashl)
            {
                if (crossCheck.ContainsKey(hs) == false)//new Val
                {
                    var roleLst = new List<string>();
                    if (HasRoleId)//adds role id
                    {
                        roleLst.Add(mentionRoleId);
                    }
                    crossCheck.Add(hs, roleLst);
                }
                else
                {
                    if (HasRoleId)
                    {
                        var roleLst = crossCheck[hs];
                        if (!roleLst.Contains(mentionRoleId))
                        {
                            roleLst.Add(mentionRoleId);
                            crossCheck[hs] = roleLst.Distinct().ToList();
                        }
                    }
                }
            }
            return crossCheck;
        }


        public static Task DailyLlamaCallback(ICatalogServiceEventArgs e)
        {
            return Task.Run(() =>
            {
                using (var context = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
                {
                    try
                    {
                        var ptable = context.Priority.GetValidGuildPartners();
                        if (e.Type.Contains(CatalogType.CardPackStorePreroll) || e.Type.Contains(CatalogType.CardPackStoreGameplay))
                        {
                            var mEmbedList = Fortnite.Core.Utils.GetDailyLlamas(e.Catalog).ToWebhookEmbed();

                            {
                                foreach (var priority in ptable)
                                {
                                    try
                                    {
                                        var guid = priority.GetUlongId().ToString();
                                        var gConfig = context.Guild.GetConfig(guid);
                                        if (gConfig == null)
                                            continue;
                                        DailyLlamaWebhook(priority.GetUlongId(), gConfig, new List<Embed>() { mEmbedList }).Wait();
                                    }
                                    catch (Exception exception1)
                                    {
                                        Log.Exception(exception1, exceptionNote: $"PriorityId is {priority.Id}");
                                    }
                                }
                            }
                        }
                        Log.Information("Webhook: DailyLlama is Finished");
                    }
                    catch (Exception exception2)
                    {
                        Log.Exception(exception2, Serilog.Events.LogEventLevel.Fatal);
                    }
                }
            });
        }

        public static Task DailyLlamaWebhook(ulong guid, GuildConfig gConfig, IEnumerable<Embed> Missions)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (gConfig == null || gConfig.Admin.LlamaSates.Active == false)
                        return;

                    var discordApi = DIManager.Services.GetRequiredService<IDiscordRestApi>(); ;
                    var guild = discordApi.GetApi.GetGuildAsync(guid).Result;
                    if (guild == null)
                        return;

                    var channel = guild.GetTextChannelAsync(ulong.Parse(gConfig.Admin.LlamaSates.ChannelId)).Result;
                    if (channel != null)
                    {
                        foreach (var item in Missions)
                        {
                            if (item == null)
                                continue;
                            string mentionTag = string.Empty;
                            RestRole r0 = null;
                            if (!string.IsNullOrWhiteSpace(gConfig.Admin.LlamaSates.RoleIdToMention))
                            {
                                mentionTag = $"<@&{gConfig.Admin.LlamaSates.RoleIdToMention}>";
                                r0 = guild.GetRole(ulong.Parse(gConfig.Admin.LlamaSates.RoleIdToMention));
                                r0.ModifyAsync((o) =>
                                {
                                    o.Mentionable = true;
                                }).Wait();
                            }
                            channel.SendMessageAsync(mentionTag, false, item, Utils.RequestOption).Wait();
                            r0?.ModifyAsync((o) =>
                            {
                                o.Mentionable = false;
                            }).Wait();
                        }
                    }
                }
                catch (Exception exception1)
                {
                    Log.Exception(exception1, Serilog.Events.LogEventLevel.Error);
                }
            });
        }

        public static Task BrDailyStoreCallback(BrDailyStoreEventArgs e1)
        {
            return Task.Run(() =>
            {
                using (var context = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
                {
                    try
                    {
                        var ptable = context.Priority.GetValidGuildPartners();

                        foreach (var priority in ptable)
                        {
                            try
                            {
                                var guid = priority.GetUlongId().ToString();

                                var gConfig = context.Guild.GetConfig(guid);
                                if (gConfig == null)
                                    continue;

                                BrStoreWebhook(priority.GetUlongId(), e1.StoreFileName, e1.Title, gConfig).Wait();
                            }
                            catch (Exception exception1)
                            {
                                Log.Exception(exception1, exceptionNote: $"PriorityId is {priority.Id}");
                            }
                        }
                        Log.Information("Webhook: BrDailyStore is Finished");
                    }
                    catch (Exception exception2)
                    {
                        Log.Exception(exception2, Serilog.Events.LogEventLevel.Fatal);
                    }
                }
            });
        }
        public static Task BrStoreWebhook(ulong guid, string fileName, string title, GuildConfig gConfig)
        {
            return Task.Run(() =>
            {
                try
                {
                    if (gConfig == null || gConfig.Admin.BrStoreStates.Active == false)
                        return;
                    var discordApi = DIManager.Services.GetRequiredService<IDiscordRestApi>(); ;
                    var guild = discordApi.GetApi.GetGuildAsync(guid).Result;
                    if (guild == null)
                        return;

                    var channel = guild.GetTextChannelAsync(ulong.Parse(gConfig.Admin.BrStoreStates.ChannelId)).Result;
                    if (channel != null)
                    {
                        var embedImage = new EmbedBuilder()
                        {
                            ImageUrl = $"attachment://{fileName}"
                        }.Build();
                        string mentionTag = string.Empty;
                        RestRole r0 = null;
                        if (!string.IsNullOrWhiteSpace(gConfig.Admin.BrStoreStates.RoleIdToMention))
                        {
                            mentionTag = $"<@&{gConfig.Admin.BrStoreStates.RoleIdToMention}>";
                            r0 = guild.GetRole(ulong.Parse(gConfig.Admin.BrStoreStates.RoleIdToMention));
                            r0.ModifyAsync((o) =>
                            {
                                o.Mentionable = true;
                            }).Wait();
                        }
                        channel.SendFileAsync(fileName, $"{mentionTag} {title}", embed: embedImage, options: Utils.RequestOption).Wait();
                        r0?.ModifyAsync((o) =>
                        {
                            o.Mentionable = false;
                        }).Wait();
                    }
                }
                catch (Exception exception1)
                {
                    Log.Exception(exception1, Serilog.Events.LogEventLevel.Error);
                }
            });
        }

        public static Task StwStoreCallback(StwStoreEventArgs e1)
        {
            return Task.Run(() =>
            {
                using (var context = DIManager.Services.GetRequiredService<IFTNPowerRepository>())
                {
                    try
                    {
                        var ptable = context.Priority.GetValidGuildPartners();

                        {
                            foreach (var priority in ptable)
                            {
                                try
                                {
                                    var guid = priority.GetUlongId().ToString();
                                    var gConfig = context.Guild.GetConfig(guid);
                                    if (gConfig == null)
                                        continue;

                                    StwStoreWebhook(priority.GetUlongId(), e1.StoreFileName, e1.Title, gConfig).Wait();
                                }
                                catch (Exception exception1)
                                {
                                    Log.Exception(exception1, exceptionNote: $"PriorityId is {priority.Id}");
                                }
                            }
                            Log.Information("Webhook: StwStore is Finished");
                        }
                    }
                    catch (Exception exception2)
                    {
                        Log.Exception(exception2, Serilog.Events.LogEventLevel.Fatal);
                    }
                }
            });
        }

        public static Task StwStoreWebhook(ulong guid, string fileName, string title, GuildConfig gConfig)
        {
            return Task.Run(() =>
            {
                try
                {
                    fileName = Path.GetFileName(fileName);
                    if (gConfig == null || gConfig.Admin.StwStoreStates.Active == false)
                        return;

                    var discordApi = DIManager.Services.GetRequiredService<IDiscordRestApi>(); ;
                    var guild = discordApi.GetApi.GetGuildAsync(guid).Result;
                    if (guild == null)
                        return;

                    var channel = guild.GetTextChannelAsync(ulong.Parse(gConfig.Admin.StwStoreStates.ChannelId)).Result;
                    if (channel != null)
                    {
                        var embedImage = new EmbedBuilder()
                        {
                            ImageUrl = $"attachment://{fileName}"
                        }.Build();
                        string mentionTag = string.Empty;
                        RestRole r0 = null;
                        if (!string.IsNullOrWhiteSpace(gConfig.Admin.StwStoreStates.RoleIdToMention))
                        {
                            mentionTag = $"<@&{gConfig.Admin.StwStoreStates.RoleIdToMention}>";
                            r0 = guild.GetRole(ulong.Parse(gConfig.Admin.StwStoreStates.RoleIdToMention));
                            r0.ModifyAsync((o) =>
                            {
                                o.Mentionable = true;
                            }).Wait();
                        }
                        channel.SendFileAsync(fileName, $"{mentionTag} {title}", embed: embedImage, options: Utils.RequestOption).Wait();
                        r0?.ModifyAsync((o) =>
                        {
                            o.Mentionable = false;
                        }).Wait();
                    }
                }
                catch (Exception exception1)
                {
                    Log.Exception(exception1, Serilog.Events.LogEventLevel.Error);
                }
            });
        }
    }
}
