using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fortnite.Api;
using FTNPower.Data;
using FTNPower.Core;
using FTNPower.Model;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Interfaces;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using Fortnite.Localization;
using FTNPower.Core.DiscordContext;
using Discord.Rest;

namespace FTNPower.Modules
{
    public class FortniteFunctionLayer : BaseModule
    {
        private IEpicApi _Api = null;

        public FortniteFunctionLayer() : base()
        {

        }

        public IEpicApi Api
        {
            get
            {
                if (_Api == null)
                    _Api = DIManager.Services.GetRequiredService<IEpicApi>();
                return _Api;
            }
        }
        public Task _ShowsPVEDecimals(string onOffState)
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
                if (string.Equals(onOffState, "on", StringComparison.CurrentCultureIgnoreCase))
                {
                    Context.GuildConfig.Owner.PVEDecimalState = true;
                }
                else if (string.Equals(onOffState, "off", StringComparison.CurrentCultureIgnoreCase))
                {
                    Context.GuildConfig.Owner.PVEDecimalState = false;
                }
                else
                {
                    return;
                }
                Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
                Context.Repo.Commit();
                Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
                await ReplyEmbedAsync(Translate.GetBotTranslation(BotTranslationString.ShowsPVEDecimals1, GetLanguage(), onOffState));
            });
        }
        public Task _AutoRemoveRequest(string onOffState)
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
                if (string.Equals(onOffState, "on", StringComparison.CurrentCultureIgnoreCase))
                {
                    Context.GuildConfig.Owner.AutoRemoveRequest = true;
                }
                else if (string.Equals(onOffState, "off", StringComparison.CurrentCultureIgnoreCase))
                {
                    Context.GuildConfig.Owner.AutoRemoveRequest = false;
                }
                else
                {
                    return;
                }
                Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
                Context.Repo.Commit();
                Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
                await ReplyEmbedAsync(Translate.GetBotTranslation(BotTranslationString.AutoRemoveRequest1, GetLanguage(), onOffState));
            });
        }

        public Task _AddMapRoles()
        {
            return Task.Run(() =>
            {
                if (!Context.Guild.CurrentUser.GuildPermissions.ManageRoles)
                {
                    ReplyEmbedErrorAsync("**error occurred while adding map roles**\n" +
                                                "- FTN Power has no permission to adding map roles\n");
                }
                else if (Context.Repo.Guild.Enable(Context.Guild, Context.GuildConfig.Owner.DefaultLanguage))
                {
                    ReplyEmbedAsync("**Successfully map roles are added.**\n" +
                                         "Notes:\n" +
                                         "- Move as far as possible 'FTN Power' role to upwards for managing discord user's informations\n" +
                                         "- You can delete any map role, if you dont need\n" +
                                         "- Be careful to role order while change the position.\n" +
                                         "\n*example role order:*\n" +
                                         "+ FTN Power (move top)\n" +
                                         "+ **any roles**\n" +
                                         "+ Twine Peaks\n" +
                                         "+ Canny Valley\n" +
                                         "+ Plankerton\n" +
                                         "+ Stonewood\n" +
                                         "+ noname\n" +
                                         "+ **any roles**\n", null, null, null, Color.DarkGreen);
                }
                else
                {
                    ReplyEmbedErrorAsync("**error occurred while adding map roles, please contact with Developer**");
                }
            });
        }
        public Task _ClearRestrictionRole()
        {
            return Task.Run(() =>
            {
                List<string> selectedRole = Context.GuildConfig.Owner.RestrictedRoleIds;
                string roles = "";
                foreach (var roleId in selectedRole)
                {
                    ulong rid = ulong.Parse(roleId);
                    var guildRole = Context.Guild.GetRole(rid);
                    roles += $"Id: {roleId}, Name: '{guildRole.Name}'\n";
                }
                Context.GuildConfig.Owner.RestrictedRoleIds.Clear();
                Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
                Context.Repo.Commit();
                Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
                ReplyEmbedAsync($"Removed\n**{roles}**\nnow there is no restriction for **f.pve.....** commands");
            });
        }
        public Task _HelpCmd2()
        {
            return Task.Run(async () =>
            {
                string cmdText = "full command list is moved to website [**https://ftnpower.com**](https://bit.ly/FTNPowerCmds)";
                await ReplyEmbedAsync(cmdText, "FTN Power Commands");
            });
        }
        /*
        public async Task _HelpCmd()
        {
            var cmds1 = "";
            var cmds2 = "";
            var cuser = this.Context.Guild.GetUser(this.Context.User.Id);

            if (cuser.IsServerOwner() || cuser.Username == "Kesintisiz")
            {
                cmds1 += $"__**Server Owner**__\n" +
                        $"**f.lang <TR/EN/RU/ES/DE/PT/FR/NL>** :changes the native bot response language\n" +
                        $"**f.pve.maproles.add** :manually adds none/Stonewood/Plankerton/CannyValley/TwinePeaks map roles\n" +
                        $"**f.restrict.role <ROLE NAME>** :specific access to **f.pve...** commands for usage\n" +
                        $"**f.restrict.role.remove <ROLE NAME>** :removes specific role from access list\n" +
                        $"**f.restrict.role.clear** :everyone in discord can use **f.pve....** commands\n\n";
            }

            if (cuser.GuildPermissions.Administrator || cuser.IsServerOwner() || cuser.Username == "Kesintisiz")
            {
                cmds1 += $"  __**Administrator**__\n" +
                        $"**f.state.decimals <ON/OFF>** :(default:on)controls the state of displaying decimal point of power level at current discord\n" +
                        $"**f.state.autoremove <ON/OFF>** :(default:off)remove user request after FTN Power responds\n" +
                        $"**f.stat** :how many user are there in discord server\n" +
                        $"**f.helpme <MSG>** :sends private help message to FTN Power Developer\n" +
                        $"**f.check.webhook** :checks webhook configurations for Mission Alert and Daily Llama\n" +
                        $"**f.discord.info** :shows current discord's bot information\n\n";
            }

            if (cuser.GuildPermissions.ManageNicknames || cuser.GuildPermissions.ManageRoles || cuser.Username == "Kesintisiz")
            {
                cmds1 += $"  __*needs ManageRoles + ManageNicknames Permisions to use(can be moderator)*__\n" +
                        $"**f.user.mode @MENTION <PVE/PVP>** :changes mentioned user's game mode(like f.mode)\n" +
                        $"**f.user.info @MENTION** :CHANGES mentioned user's information\n" +
                        $"**f.user.name @MENTION <EPIC NAME>** :sets a name to mentioned user (like f.name)\n" +
                        $"**f.user.update @MENTION** :adds user to queue for account power check (like f.update/f.up)\n\n";
            }

            cmds2 += $"  __**PVE User**__\n" +
                     $"**f.verify** :profile verification via FTN Power\n" +
                     $"**f.mode <PVE/PVP>** :changes your game mode\n" +
                     $"**f.pve** :current user's stats\n" +
                     $"**f.pve <EPIC NAME>** :specified user stats\n" +
                     $"**f.pve.survivors** :calculation of current squads resources\n" +
                    $"**f.pr <EPIC NAME>** OR **f.pve.resources <EPIC NAME>** :specified user's resources\n" +
                    $"**f.pr** OR **f.pve.resources** :list of current user's resources\n" +
                    //  $"**f.pa** OR **f.pve.alerts** :list of current user's Alerts\n" +
                    //    $"**f.pa <EPIC NAME>** OR **f.pve.alerts <EPIC NAME>** :specific user's Alerts\n" +
                    $"**f.ps** OR **f.pve.missions** :pve mission alerts\n" +
                    $"**f.pm** OR **f.pve.missions <vbuck/legendary/epic/legend survivor/legend perk/epic perk>** :filtered mission alerts\n" +
                    $"**f.patch** :Fortnite Patch Notes\n" +
                    $"**f.lock** :locks name changes in typed discord by FTN Power\n" +
                    $"**f.unlock** :un-locks name changes in typed discord by FTN Power\n" +
                    $"**f.link <EPIC NAME>** OR **f.name <EPIC NAME>** :sets/links a fortnite username(default:PVE)\n" +
                    $"**f.unlink** :removes your Epic Name from bot\n" +
                    $"**f.up** OR **f.update** :updates user's current GameMode status (according to discord nickname)\n" +
                    $"**f.name.tag <LEFT/RIGHT>** :(default:LEFT)shows user power at the end of name or beginning of the name\n" +
                    $"**f.donate** OR **f.info** :donation and help links\n" +
                    $"**f.top** :shows current discord's Fortnite pve top list\n" +
                    $"**f.top.global** :shows global Fortnite pve top list (which discord uses FTN Power)\n\n";

            cmds2 += $"  __**PVP User**__\n" +
                     $"**f.pvp.name <EPIC NAME>** :sets/links a fortnite username(PVP) (and adds to queue)\n" +
                     $"**f.pvp.up** OR **f.up** :updates user's current GameMode status (according to discord nickname)\n" +
                     $"**f.pvp** :current user's battle royale match stats\n" +
                     $"**f.pvp <EPIC NAME>** :specified user's battle royale match stats\n\n";

            if (cmds1 != "")
            {
                await this.ReplyEmbedAsync(cmds1, "FTN Power Commands");
            }
            if (cmds2 != "")
            {
                await this.ReplyEmbedAsync(cmds2, "FTN Power Commands");
            }

            return;
        }
        */
        public Task _NameChange(string mention, [Remainder]string newName)
        {
            return Task.Run(async () =>
            {
                if (string.IsNullOrWhiteSpace(newName))
                    return;

                SocketGuildUser cuser = null;
                bool anotherUser = false;
                if (Context.Message.MentionedUsers.Count == 1 && !string.IsNullOrWhiteSpace(mention))
                {
                    anotherUser = true;
                    var u = Context.Message.MentionedUsers.First();

                    cuser = Context.Guild.GetUser(u.Id);
                }
                else
                {
                    cuser = Context.Guild.GetUser(Context.User.Id);
                }
                if (cuser.IsBot)
                {
                    return;
                }
                newName = newName.TrimStart('<').TrimEnd('>');
                if (!newName.ValidName())
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.ContainsNotAllowed, GetLanguage(), cuser.Mention));
                    Log.Write(Serilog.Events.LogEventLevel.Error, "user {UserName}({UserId}) in guild {GuildId} has no valid n-length, current is {UserNameLength} Text:'{WrongText}'", cuser.Username, cuser.Id, cuser.Guild.Id, newName.Length, newName);
                    return;
                }

                var uid = cuser.Id.ToString();
                var guid = cuser.Guild.Id.ToString();
                var usr = Context.DiscordUser;
                if (anotherUser)
                {
                    usr = Context.Repo.Db<FortniteUser>()
                                      .GetById(uid);
                }
                if (usr == null)
                {
                    usr = await Context.Repo.User.AddOrGetUserAsync(uid, guid, GameUserMode.PVE);
                }
                NameState ns = usr.NameStates.FirstOrDefault(p => p.FortniteUserId == uid && p.DiscordServerId == guid);
                if (ns == null)
                {
                    ns = await Context.Repo.User.AddOrGetNameStateAsync(usr, guid);
                }
                else if (ns.LockName)
                {
                    await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.YourNameIsLocked, GetLanguage(), cuser.Mention));
                    return;
                }
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 10, 0), usr, cuser, ns);
                if (pstate.Value == null)
                    return;

                IUserMessage msg = null;
                var checkVerificationOfNewName = Api.GetUserIdByName(newName);
                if (checkVerificationOfNewName.Value == null)
                {
                    msg = await ReplyEmbedPriorityAsync(ToTranslate(BotTranslationString.NameWillBeUpdated, GetLanguage(), cuser.Mention, newName), pstate);
                    await msg?.SetErrorAsync();
                    return;
                }

                if (Context.Repo.Db<FortniteUser>().All().Any(f => f.VerifiedProfile && f.IsValidName && f.EpicId == checkVerificationOfNewName.Value.id && f.Id != uid))
                {
                    await ReplyEmbedErrorAsync($"you can not take {checkVerificationOfNewName.Value.displayName} Username, it is __already verified__ by account owner.");
                    return;
                }
                bool profileNotFoundButNameExists = false;
                if (Context.GuildConfig.Owner.DefaultGameMode == GameUserMode.PVE && Context.DiscordUser.GameUserMode == GameUserMode.NULL ||
                    Context.DiscordUser.GameUserMode == GameUserMode.PVE)
                {
                    Context.DiscordUser.VerifiedProfile = false;
                    Context.Repo.Db<FortniteUser>().Update(Context.DiscordUser);
                    Context.Repo.Commit();
                    msg = await ReplyEmbedPriorityAsync(ToTranslate(BotTranslationString.NameWillBeUpdated, GetLanguage(), cuser.Mention, newName), pstate);
                    var mapUser = await Api.GetPVEProfileByName(checkVerificationOfNewName.Value.displayName);
                    if (mapUser.Value != null)
                    {
                        await Context.Repo.UpdateDatabasePVEProfileAsync(mapUser, cuser, msg, Context.DiscordUser.NameTag, ns.LockName, Context.GuildConfig.Owner.PVEDecimalState);
                        return;
                    }
                    else
                    {
                        profileNotFoundButNameExists = checkVerificationOfNewName.Value != null;
                    }
                }
                else if (Context.GuildConfig.Owner.DefaultGameMode == GameUserMode.PVP_WIN_ALL && Context.DiscordUser.GameUserMode == GameUserMode.NULL ||
                    Context.DiscordUser.GameUserMode == GameUserMode.PVP_WIN_ALL)
                {
                    var mapUserPVP = await Api.GetPVPProfileByName(checkVerificationOfNewName.Value.displayName);
                    if (mapUserPVP.Value != null)
                    {
                        if (mapUserPVP.Value.IsPrivate)
                        {
                            msg = await ReplyEmbedPriorityAsync(ToTranslate(BotTranslationString.PvpProfilIsNotPublic, GetLanguage(), cuser.Mention), pstate);
                        }
                        else
                        {
                            Context.DiscordUser.VerifiedProfile = false;
                            Context.Repo.Db<FortniteUser>().Update(Context.DiscordUser);
                            Context.Repo.Commit();
                            msg = await ReplyEmbedPriorityAsync(ToTranslate(BotTranslationString.NameWillBeUpdated, GetLanguage(), cuser.Mention, newName), pstate);
                            await Context.Repo.UpdateDatabasePVPProfileAsync(mapUserPVP, cuser, msg, Context.DiscordUser.NameTag, ns.LockName);
                            return;
                        }
                    }
                    else
                    {
                        profileNotFoundButNameExists = checkVerificationOfNewName.Value != null;
                    }
                }
                if (profileNotFoundButNameExists)
                {
                    await msg?.SetConsolePlayerAsync();
                }
                else
                {
                    await msg?.SetErrorAsync();
                }
            });
        }

        public Task _NameTag(FortniteUser usr, SocketGuildUser cuser, string tag)
        {
            return Task.Run(async () =>
            {
                if (cuser.IsServerOwner())
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.CanNotUpdateDiscordOwner, GetLanguage(), cuser.Mention));
                    return;
                }
                if (!usr.IsValidName)
                {
                    await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), cuser.Mention));
                    return;
                }
                var guid = cuser.Guild.Id.ToString();
                var ns = usr.NameStates.FirstOrDefault(p => p.DiscordServerId == guid);
                var pstate = await CheckUserPriority(new TimeSpan(0, 0, 0, 30), usr, cuser, ns);
                if (pstate.Value == null)
                    return;

                if (string.Equals(tag, "right", StringComparison.InvariantCultureIgnoreCase) || string.Equals(tag, "sağ", StringComparison.InvariantCultureIgnoreCase))
                {
                    usr.NameTag = true;
                }
                else if (string.Equals(tag, "left", StringComparison.InvariantCultureIgnoreCase) || string.Equals(tag, "sol", StringComparison.InvariantCultureIgnoreCase))
                {
                    usr.NameTag = false;
                }
                else
                {
                    await ReplyEmbedErrorAsync("unknown parameter please write **f.name.tag left** or **f.name.tag right**");
                    return;
                }
                Context.Repo.Db<FortniteUser>().Update(usr);
                Context.Repo.Commit();
                string newPowerName = null;
                if (usr.GameUserMode == GameUserMode.PVE)
                {

                    newPowerName = Context.Repo.ModifyPVETag(Context.PlayerName, Context.AccountPowerLevel, usr.NameTag, Context.GuildConfig.Owner.PVEDecimalState, false);

                }
                else
                {
                    newPowerName = Context.Repo.ModifyPVPTag(Context.PlayerName, Context.TotalPVPRankedWins, usr.NameTag, false);
                }
                var msg = await ReplyEmbedAsync("**Name Tag is successfully changed.**");
                if (!ns.LockName)
                {
                    await cuser.ModifyAsync((o) =>
                    {
                        o.Nickname = newPowerName;
                    });
                }
                else
                {
                    await msg.SetLockAsync();
                }
            });
        }

        public Task _RemoveRestrictionRole(string roleName)
        {
            return Task.Run(() =>
            {
                var selectedRole = Context.Guild.Roles
                    .FirstOrDefault(p => p.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
                if (selectedRole != null)
                {
                    if (Context.GuildConfig.Owner.RestrictedRoleIds.Remove(selectedRole.Id.ToString()))
                    {
                        Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
                        Context.Repo.Commit();
                        Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
                        ReplyEmbedAsync($"'{roleName}' is successfully removed from restriction list.", null, null, null, Color.Green);
                    }
                    else
                    {
                        ReplyEmbedErrorAsync($"'{roleName}' role is not found in restriction list.");
                    }
                }
                else
                {
                    ReplyEmbedErrorAsync($"'{roleName}' role is not found at your discord.");
                }
            });
        }

        public Task _SetRestrictionRole(string roleName)
        {
            return Task.Run(() =>
            {
                var selectedRole = Context.Guild.Roles
                    .FirstOrDefault(p => p.Name.Equals(roleName, StringComparison.InvariantCultureIgnoreCase));
                if (selectedRole != null)
                {
                    if (!Context.GuildConfig.Owner.RestrictedRoleIds.Contains(selectedRole.Id.ToString()))
                    {
                        Context.GuildConfig.Owner.RestrictedRoleIds.Add(selectedRole.Id.ToString());
                        Context.Repo.Db<GuildConfig>().Update(Context.GuildConfig);
                        Context.Repo.Commit();
                        Context.Redis.JsonDelete(Context.Redis.Key<GuildConfig>(Context.GuildConfig.Id));
                    }
                    ReplyEmbedAsync($"**{selectedRole.Name}**[{selectedRole.Id}] is allowed for **f.pve.....** commands", null, null, null, Color.Green);
                }
                else
                {
                    ReplyEmbedErrorAsync($"'{roleName}' role is not found at your discord.");
                }
            });
        }

        public Task _UpdateName(string mention)
        {
            return Task.Run(async () =>
            {
                Regex r = new Regex("<@!?(\\d+)>", RegexOptions.Singleline);
                var m = r.Match(mention);
                if (m.Success)
                {

                    RestGuildUser cuser = Context.DiscordRestApi.GetGuildUserAsync(Context.Guild.Id, m.Groups[1].Value.ToUlong()).Result;
                    if (cuser == null)
                    {
                        return;
                    }
                    if (cuser.IsServerOwner())
                    {
                        await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.CanNotUpdateDiscordOwner, GetLanguage(), cuser.Mention));
                        return;
                    }

                    var uid = cuser.Id.ToString();
                    {
                        var usr = Context.Repo.Db<FortniteUser>()
                                                   .GetById(uid);
                        NameState ns = usr.NameStates.FirstOrDefault(p => p.FortniteUserId == uid && p.DiscordServerId == Context.Guild.Id.ToString());
                        if (usr == null)
                        {
                            await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd1, GetLanguage(), cuser.Mention));
                            return;
                        }
                        else if (ns == null)
                        {
                            ns = await Context.Repo.User.AddOrGetNameStateAsync(usr, Context.Guild.Id.ToString());
                        }

                        if (!usr.IsValidName)
                        {
                            await ReplyEmbedErrorAsync(Translate.GetBotTranslation(BotTranslationString.FirstlyUseAnotherCmd2, GetLanguage(), cuser.Mention));

                            return;
                        }
                        var pstate = await CheckUserPriority(new TimeSpan(0, 0, 10, 0), usr, cuser, ns);
                        if (pstate.Value == null)
                            return;
                        var msg = await ReplyEmbedPriorityAsync(ToTranslate(BotTranslationString.UpdateRequestReceived, GetLanguage(), mention), pstate);
                        if (!string.IsNullOrWhiteSpace(usr.EpicId))
                        {
                            if (usr.GameUserMode == GameUserMode.PVE)
                            {
                                FortnitePVEProfile fortniteProfile = Context.Repo.Db<FortnitePVEProfile>()
                                                                                 .All()
                                                                                 .FirstOrDefault(f => f.EpicId == usr.EpicId);
                                await SetNewNameViaMode<FortnitePVEProfile>(cuser, usr, msg, fortniteProfile);
                            }
                            else
                            {
                                FortnitePVPProfile fortniteProfile = Context.Repo.Db<FortnitePVPProfile>()
                                                                                 .All()
                                                                                 .FirstOrDefault(f => f.EpicId == usr.EpicId);
                                await SetNewNameViaMode<FortnitePVPProfile>(cuser, usr, msg, fortniteProfile);
                            }
                        }
                        else
                        {
                            await msg.SetErrorAsync();
                        }
                    }
                }
                else
                {
                    ReplyEmbedErrorAsync($"invalid user mention.").Wait();
                }
            });
        }

        public Task CheckRestriction(Func<Task> action)
        {
            if (Context.GuildConfig.Owner.RestrictedRoleIds.Count == 0)
            {
                return action();
            }
            else
            {
                SocketGuildUser cuser = (SocketGuildUser)Context.User;
                bool hasThatRole = false;
                foreach (var role in cuser.Roles)
                {
                    if (Context.GuildConfig.Owner.RestrictedRoleIds.Contains(role.Id.ToString()))
                    {
                        hasThatRole = true;
                        break;
                    }
                }

                if (hasThatRole || cuser.GuildPermissions.Administrator || cuser.IsServerOwner())
                {
                    return action();
                }
                else
                {
                    return ReplyEmbedErrorAsync($"only allowed roles can use in here.");
                }
            }
        }

        public async Task<KeyValuePair<bool, PriorityTable>> CheckUserPriority(TimeSpan? requestSecondLimit, FortniteUser usr, IGuildUser cuser, NameState ns = null, DateTimeOffset? lastMassUpdate = null)
        {
            if (!requestSecondLimit.HasValue)
                requestSecondLimit = new TimeSpan(0, 0, 10, 0);
            TimeSpan reminder = DateTimeOffset.UtcNow - usr.LastUpDateTime;
            if (lastMassUpdate.HasValue)
                reminder = DateTimeOffset.UtcNow - lastMassUpdate.Value;
            var pStateResult = Context.Repo.Priority.IsPartnerGuild(cuser.Guild.Id);
            if (pStateResult.Key == false)
            {
                pStateResult = Context.Repo.Priority.IsPartnerUser(cuser.Id);
            }
            if (pStateResult.Key && pStateResult.Value.GetPriorityState() != PriorityState.Normal)
            {
                if (!lastMassUpdate.HasValue)
                    requestSecondLimit = new TimeSpan(0, 0, 0, 1);
            }
            if (reminder.TotalSeconds > requestSecondLimit.Value.TotalSeconds)
            {
                return pStateResult;
            }
            else
            {
                var ts = requestSecondLimit.Value.Add(new TimeSpan(0, 0, 0, 1)) - reminder;
                await ReplyEmbedErrorAsync(ToTranslate(BotTranslationString.YouHaveToWaitTime1, GetLanguage(), cuser.Mention,
                    ts.Hours, ts.Minutes, ts.Seconds));
                return new KeyValuePair<bool, PriorityTable>(false, null);
            }
        }

        public bool HasPVEMode()
        {
            return Context.DiscordUser.GameUserMode == GameUserMode.PVE || Context.AccountPowerLevel > 1;
        }


        public async Task<bool> SetNewNameViaMode<T>(IGuildUser cuser, FortniteUser usr, IUserMessage msg, IFortniteProfile classPveOrPvp, bool updateFromMyDb = false) where T : class
        {
            var ns = usr.NameStates.First(f => f.DiscordServerId == cuser.Guild.Id.ToString());
            if (usr.GameUserMode == GameUserMode.PVE)
            {
                if (!updateFromMyDb)
                {
                    var mapUser = await Api.GetPVEProfileById(usr.EpicId);
                    if (mapUser.Value != null)
                    {
                        return await Context.Repo.UpdateDatabasePVEProfileAsync(mapUser, cuser, msg, usr.NameTag, ns.LockName, Context.GuildConfig.Owner.PVEDecimalState);
                    }
                }
                else
                {
                    Context.Repo.Db<FortniteUser>()
                                .Update(usr);
                    Context.Repo.Commit();
                    var pveTable = (FortnitePVEProfile)classPveOrPvp;
                    return await Context.Repo.UpdateDiscordPVEProfileAsync(pveTable, usr.NameTag, cuser, msg, ns.LockName, Context.GuildConfig.Owner.PVEDecimalState);
                }
                await msg.SetErrorAsync();
                return false;
            }
            else if (usr.GameUserMode == GameUserMode.PVP_WIN_ALL)
            {
                if (!updateFromMyDb)
                {
                    var mapUser = await Api.GetPVPProfileById(usr.EpicId);
                    if (mapUser.Value != null)
                    {
                        if (mapUser.Value.IsPrivate)
                        {
                            msg = await ReplyEmbedAsync(ToTranslate(BotTranslationString.PvpProfilIsNotPublic, GetLanguage(), cuser.Mention));
                        }
                        return await Context.Repo.UpdateDatabasePVPProfileAsync(mapUser, cuser, msg, usr.NameTag, ns.LockName);
                    }
                }
                else
                {
                    Context.Repo.Db<FortniteUser>()
                                .Update(usr);
                    Context.Repo.Commit();
                    var pvpTable = (FortnitePVPProfile)classPveOrPvp;
                    return await Context.Repo.UpdateDiscordPVPProfileAsync(pvpTable, usr.NameTag, cuser, msg, ns.LockName);
                }
                await msg.SetErrorAsync();
                return false;
            }
            return false;
        }

        public Task AutoUpdateSelectedGuildUsers(string guildId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    var rbbtmqsvc = DIManager.Services.GetRequiredService<IRedisService>();
                    rbbtmqsvc.PushDiscordById(Context.Repo, "UserUpdatingQueue", guildId);
                    await ReplyEmbedAsync($"**Auto User Updating is triggered.**");
                }
                catch (Exception e)
                {
                    await ReplyEmbedErrorAsync($"[UserUpdater]{e.Message}");
                    Log.Exception(e);
                }
            });
        }
    }
}