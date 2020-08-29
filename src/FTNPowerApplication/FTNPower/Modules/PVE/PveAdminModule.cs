using Discord;
using Discord.Commands;
using Fortnite.Localization;
using FTNPower.Core;
using FTNPower.Data;
using FTNPower.Model.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FTNPower.Modules.PVE
{
    public class PveAdminModule : PveFunctionLayer
    {
        public PveAdminModule() : base()
        {
        }


        [Command("user.unlock")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public Task UnLockNameAsync(string mention)
        {
            if (Context.Message.MentionedUsers.Count < 1)
            {
                return Task.CompletedTask;
            }
            return _unlock(Context.Guild.Id.ToString(), Context.Message.MentionedUsers.First().Id.ToString());
        }
        [Command("user.lock")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task LockNameAsync(string mention)
        {
            if (Context.Message.MentionedUsers.Count < 1)
            {
                return Task.CompletedTask;
            }
            return _lock(Context.Guild.Id.ToString(), Context.Message.MentionedUsers.First().Id.ToString());
        }

        [Command("state.autoremove")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public Task AutoRemoveRequest(string state)
        {
            return _AutoRemoveRequest(state);
        }
        [Command("state.decimals")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public Task ShowsPVEDecimals(string state)
        {
            return _ShowsPVEDecimals(state);
        }
        [Command("discord.resent.brstore")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.UseExternalEmojis)]
        [RequireBotPermission(GuildPermission.AttachFiles)]
        public Task ResentBrDaily()
        {
            return _ResentBrDaily();
        }

        [Command("helpme")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public Task HelpMe([Remainder]string message)
        {
            return Task.Run(async () =>
            {

                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 0, 0), Context.DiscordUser, Context.Guild.GetUser(Context.User.Id));
                if (pstate.Value == null)
                    return;
                if (pstate.Value.GetPriorityState() == PriorityState.Normal)
                {
                    await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.EnablingToBotFeatures));
                    return;
                }
                try
                {
                    var developer = Context.DiscordRestApi.GetGuildUserAsync(this.Context.Guild.Id, 193749607107395585).Result;
                    var inviteLink = await Context.Guild.DefaultChannel.CreateInviteAsync();

                    var dm = await developer.GetOrCreateDMChannelAsync(Core.Utils.RequestOption);
                    string desc = $"ds[**{Context.Guild.Id}**]*{Context.Guild.Name}*\n" +
                                  $"ch[**{Context.Message.Channel.Id}**]*{Context.Message.Channel.Name}*\n" +
                                  $"invite:{inviteLink.Url}\n\n";
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Author = GetVerificationAuthor(),
                        Color = Color.Green,
                        Title = $"User Help Message",
                        Description = $"{desc}FROM:{Context.User.Mention}, {message}"
                    };
                    var msg = await dm.SendMessageAsync(string.Empty, false, embed.Build(), Core.Utils.RequestOption);
                    await dm.CloseAsync(Core.Utils.RequestOption);
                    await ReplyEmbedAsync($"request has sent to developer");
                }
                catch (Exception e)
                {
                    await ReplyEmbedErrorAsync(e.Message);
                }
            });
        }

        [Command("user.up")]
        [Alias("user.update", "üye.güncel")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task RefreshName(string mention)
        {
            return _UpdateName(mention);
        }
        [Command("discord.bilgi")]
        [Alias("discord.info")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task ServerInfo()
        {
            await _ServerInfo(Context.Guild.Id);
        }

        [Command("üye.kayıt")]
        [Alias("user.name")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task SetUserName(string mention, [Remainder]string name)
        {
            return _NameChange(mention, name);
        }

        [Command("durum")]
        [Alias("stat")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public async Task Status()
        {
            await _StatusAsync(Context.Guild);
        }
        [Alias("user.mod")]
        [Command("user.mode")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task UserGameMode(string mention, [Remainder]string gameMode)
        {
            if (mention.ParseId(out ulong uid))
            {
                return GameMode(uid, gameMode);
            }
            return Task.CompletedTask;
        }

        [Command("üye.bilgi")]
        [Alias("user.info")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.EmbedLinks)]
        public Task UserInfo(string mention)
        {
            return _UserInfo(mention);
        }
    }
}