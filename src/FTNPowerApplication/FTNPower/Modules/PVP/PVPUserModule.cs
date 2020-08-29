using Discord;
using Discord.Commands;
using Fortnite.Localization;
using Fortnite.Model.Enums;
using FTNPower.Model.Enums;
using System.Threading.Tasks;

namespace FTNPower.Modules.PVP
{
    public class PVPUserModule : PVPFunctionLayer
    {
        public PVPUserModule() : base()
        {
        }

        [Command("pvp.kayıt")]
        [Alias("pvp.name")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task NameChange([Remainder] string newName)
        {
            if (Context.DiscordUser.GameUserMode == GameUserMode.PVE)
            {
                Context.DiscordUser.GameUserMode = GameUserMode.PVP_WIN_ALL;
            }
            return _NameChange(null, newName);
        }

        [Command("pvp")]
        [Alias("pvp.pc")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task PveProfile()
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await PvpProfile(true, null, Platform.keyboardmouse);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }

        [Command("pvp")]
        [Alias("pvp.pc")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task PveProfile([Remainder]string userName)
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await PvpProfile(false, userName, Platform.keyboardmouse);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.YouShouldHaveValidName, GetLanguage()) + Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }

        [Command("pvp.xbl")]
        [Alias("pvp.ps4", "pvp.ps", "pvp.console")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task PveProfileConsole()
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await PvpProfile(true, null, Platform.gamepad);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }
        [Command("pvp.xbl")]
        [Alias("pvp.ps4", "pvp.ps", "pvp.console")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        public async Task PveProfileConsole([Remainder]string userName)
        {
            await CheckRestriction(async () =>
            {
                if (Context.DiscordUser.IsValidName)
                {
                    await PvpProfile(false, userName, Platform.gamepad);
                }
                else
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.YouShouldHaveValidName, GetLanguage()) + Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), Context.User.Mention));
                }
            });
        }
    }
}