using Discord;
using Discord.Rest;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Core.ModifiedModels;
using Fortnite.Localization;
using Fortnite.Model.Enums;
using Fortnite.Static;
using FTNPower.Model.Interfaces;
using Global;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Static
{
    public static class Utils
    {
        public static void SendPlanInformation(this IDiscordRestApi discordRestApi, ulong discordId, ulong duserId, string priorityState, TimeSpan Remining)
        {
            try
            {
                RestGuildUser usr = null;
                RestGuild gld = null;
                string namefor = "";
                string priorityId = "";
                string title = "? Updated";
                if (priorityState == "User")
                {
                    usr = discordRestApi.GetGuildUserAsync(discordId, duserId).Result;
                    namefor = $"{usr?.Username}#{usr?.Discriminator}";
                    priorityId = $"{priorityState}-{usr.Id}";
                    title = $"INDIVIDUAL PLAN IS UPDATED";
                }
                else if (priorityState == "Guild")
                {
                    gld = discordRestApi.GetApi.GetGuildAsync(discordId).Result;
                    usr = gld?.GetOwnerAsync().Result;
                    namefor = $"{gld?.Name}";
                    priorityId = $"{priorityState}-{gld.Id}";
                    title = $"PRO PLAN IS UPDATED";
                }

                DateTime dtx = new DateTime();
                dtx = dtx.Add(Remining);
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder()
                    {
                        IconUrl = discordRestApi.GetApi.CurrentUser.GetAvatarUrl(),
                        Name = discordRestApi.GetApi.CurrentUser.Username
                    },
                    Color = Color.Green,
                    Title = title
                };
                embed.Description += $"Type: **{priorityState}**\n" +
                                     $"Name: {namefor}\n" +
                                     $"Expires In: **{dtx.Year - 1}**Years **{dtx.Month - 1}**Months **{dtx.Day - 1}**days **{dtx.Hour}**hours\n";

                embed.Description += "\n\n";
                try
                {
                    RestDMChannel dm = usr.GetOrCreateDMChannelAsync().Result;
                    Embed bembed = embed.Build();
                    RestUserMessage msg = dm.SendMessageAsync(string.Empty, false, bembed).Result;
                    dm.CloseAsync().Wait();
                }
                catch (Exception e)
                {
                }

            }
            catch (Exception ee)
            {
                throw ee;
            }
        }

        public static Task<Dictionary<MapRoles, IRole>> GetUserRolesAsync(this IGuildUser user, GuildLanguage lang)
        {
            if (user == null)
            {
                return Task.FromResult(new Dictionary<MapRoles, IRole>());
            }
            return Task.Run(() =>
            {
                var curGuild = user.Guild;
                var Translate = DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
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
