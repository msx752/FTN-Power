using Discord;
using Discord.Commands;
using Fortnite.Localization;
using FTNPower.Core;
using FTNPower.Core.DiscordContext.Preconditions;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FTNPower.Modules.PVE
{
    public class PveServerOwnerModule : PveFunctionLayer
    {
        public PveServerOwnerModule() : base()
        {
        }

        [GuildOwnerAttribute]
        [Command("pve.haritalar.ekle")]
        [Alias("pve.maproles.add")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task AddMapRoles()
        {
            return _AddMapRoles();
        }

        [GuildOwnerAttribute]
        [Command("restrict.role.clear")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task ClearRestrictionRole()
        {
            return _ClearRestrictionRole();
        }

        [GuildOwnerAttribute]
        [Alias("discord.mod")]
        [Command("discord.mode")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public Task GameMode(string gameMode)
        {
            return Task.Run(async () =>
            {
                if (!string.IsNullOrWhiteSpace(gameMode) && (gameMode.Equals("pvp", StringComparison.InvariantCultureIgnoreCase) || gameMode.Equals("pve", StringComparison.InvariantCultureIgnoreCase)))
                {
                    if (gameMode.Equals("pvp", StringComparison.InvariantCultureIgnoreCase) &&
                        Context.GuildConfig.Owner.DefaultGameMode != GameUserMode.PVP_WIN_ALL)
                    {
                        Context.GuildConfig.Owner.DefaultGameMode = GameUserMode.PVP_WIN_ALL;
                    }
                    else if (gameMode.Equals("pve", StringComparison.InvariantCultureIgnoreCase) &&
                       Context.GuildConfig.Owner.DefaultGameMode != GameUserMode.PVE)
                    {
                        Context.GuildConfig.Owner.DefaultGameMode = GameUserMode.PVE;
                    }
                    else
                    {
                        return;
                    }
                    Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
                    Context.Repo.Commit();
                    Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
                    await Context.Message.SetSuccessAsync();
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.IncorrectGameMode, GetLanguage()));
                }
            });
        }

        [GuildOwnerAttribute]
        [Command("restrict.role.remove")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task RemoveRestrictionRole([Remainder] string roleName = null)
        {
            return _RemoveRestrictionRole(roleName);
        }

        [GuildOwnerAttribute]
        [Command("dil")]
        [Alias("lang")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task SetLanguage([Remainder] string newName = null)
        {
            if (string.IsNullOrWhiteSpace(newName) || newName.Length > 3)
                return Task.CompletedTask;
            Enum.TryParse(newName, true, out GuildLanguage newLang);

            var oldLang = GetLanguage();
            if (oldLang == newLang)
                return ReplyEmbedAsync(ToTranslate(BotTranslationString.SameLanguage));

            var mapRoles = Context.Guild.GetMapRolesAsync(oldLang).Result;
            foreach (var r in mapRoles)
            {
                r.Value.ModifyAsync((rp) =>
                {
                    if (r.Value.Name == Translate.GetBotTranslation(BotTranslationString.Stonewood, oldLang))
                    {
                        rp.Name = Translate.GetBotTranslation(BotTranslationString.Stonewood, newLang);
                    }
                    else if (r.Value.Name == Translate.GetBotTranslation(BotTranslationString.Plankerton, oldLang))
                    {
                        rp.Name = Translate.GetBotTranslation(BotTranslationString.Plankerton, newLang);
                    }
                    else if (r.Value.Name == Translate.GetBotTranslation(BotTranslationString.CannyValley, oldLang))
                    {
                        rp.Name = Translate.GetBotTranslation(BotTranslationString.CannyValley, newLang);
                    }
                    else if (r.Value.Name == Translate.GetBotTranslation(BotTranslationString.TwinePeaks, oldLang))
                    {
                        rp.Name = Translate.GetBotTranslation(BotTranslationString.TwinePeaks, newLang);
                    }
                    else if (r.Value.Name == Translate.GetBotTranslation(BotTranslationString.noname, oldLang))
                    {
                        rp.Name = Translate.GetBotTranslation(BotTranslationString.noname, newLang);
                    }
                });
            }
            Context.GuildConfig.Owner.DefaultLanguage = newLang;
            Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
            Context.Repo.Commit();
            Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
            return ReplyEmbedAsync(ToTranslate(BotTranslationString.SetGuildLanguage, newLang, newLang));
        }

        [GuildOwnerAttribute]
        [Command("restrict.role")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task SetRestrictionRole([Remainder] string roleName = null)
        {
            return _SetRestrictionRole(roleName);
        }
    }
}