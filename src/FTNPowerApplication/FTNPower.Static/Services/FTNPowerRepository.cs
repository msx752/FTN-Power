using Discord;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Static;
using FTNPower.Core;
using FTNPower.Core.ApplicationService;
using FTNPower.Core.DomainService;
using FTNPower.Core.DomainService.SubRepositories;
using FTNPower.Core.Interfaces;
using FTNPower.Data.Tables;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTNPower.Static.Services
{
    public class FTNPowerRepository : IFTNPowerRepository, IDisposable
    {
        private IRedisService _redis;
        private IUnitOfWork _uow;
        private BlacklistRepo _blacklist;
        private GuildRepo _guild;
        private PriorityRepo _priority;
        private StoredProcedureRepo _storedProcedure;
        private UserRepo _user;

        public FTNPowerRepository(IUnitOfWork uow, IRedisService redisService)
        {
            _uow = uow;
            _redis = redisService;
        }

        public BlacklistRepo Blacklist
        {
            get
            {
                if (_blacklist == null)
                    _blacklist = new BlacklistRepo(_uow, _redis);
                return _blacklist;
            }
        }


        public GuildRepo Guild
        {
            get
            {
                if (_guild == null)
                    _guild = new GuildRepo(_uow, _redis);
                return _guild;
            }
        }

        private BotConfigRepo _botconfig;
        public BotConfigRepo Bot
        {
            get
            {
                if (_botconfig == null)
                    _botconfig = new BotConfigRepo(_uow, _redis);
                return _botconfig;
            }
        }
        public PriorityRepo Priority
        {
            get
            {
                if (_priority == null)
                    _priority = new PriorityRepo(_uow, _redis);
                return _priority;
            }
        }

        public StoredProcedureRepo StoredProcedure
        {
            get
            {
                if (_storedProcedure == null)
                    _storedProcedure = new StoredProcedureRepo(_uow, _redis);
                return _storedProcedure;
            }
        }

        public UserRepo User
        {
            get
            {
                if (_user == null)
                    _user = new UserRepo(_uow, _redis);
                return _user;
            }
        }

        public int Commit()
        {
            return _uow.Commit();
        }
        public IEFRepository<TEntity> Db<TEntity>() where TEntity : class
        {
            return _uow.Db<TEntity>();
        }

        public void LockName(string guildId, string userId)
        {
            LockState(guildId, userId, true);
        }

        public string ModifyPVETag(string PlayerName, double AccountPowerLevel, bool NameTag, bool PVEDecimals, bool forceLeft = false)
        {
            var energy = AccountPowerLevel.ToString();

            if (PVEDecimals == false)
                energy = Math.Round(AccountPowerLevel, 0, MidpointRounding.AwayFromZero).ToString();

            string newNameWithTag = $"{energy}{Bot.Config.Vars.Lightning} {PlayerName}";
            if (forceLeft == false && NameTag)
                newNameWithTag = $"{PlayerName} {Bot.Config.Vars.Lightning}{energy}";
            return newNameWithTag;
        }

        public string ModifyPVPTag(string PlayerName, int currentTotalWins, bool NameTag, bool forceLeft = false)
        {
            string newNameWithTag = $"{ currentTotalWins.ToString()}{Bot.Config.Vars.Trophy} {PlayerName}";
            if (forceLeft == false && NameTag)
                newNameWithTag = $"{PlayerName} {Bot.Config.Vars.Trophy}{ currentTotalWins.ToString()}";
            return newNameWithTag;
        }

        public void ResetFeatures(string discordId)
        {
            var guild = Guild.GetConfig(discordId);
            if (guild != null)
            {
                guild.Owner.AutoRemoveRequest = false;
                guild.Owner.PVEDecimalState = true;
                guild.Admin.LlamaSates.Active = false;
                guild.Admin.MissionStates.Active = false;
                guild.Admin.StwStoreStates.Active = false;
                guild.Admin.BrStoreStates.Active = false;
                guild.Event.EliteFrostnite2019s.Active = false;
                guild.Event.MythicSKStates.Active = false;
                Db<GuildConfig>().Update(guild);
                Commit();
                _redis.JsonDelete(_redis.Key<GuildConfig>(guild.Id));
            }
        }

        public void UnLockName(string guildId, string userId)
        {
            LockState(guildId, userId, false);
        }

        public async Task<bool> UpdateDatabasePVEProfileAsync(KeyValuePair<string, IQueryProfile> profile, IGuildUser guser, IUserMessage msg = null, bool nameTag = false, bool IsNameLocked = false, bool PVEDecimals = true)
        {
            if (_uow.Db<BlackListUser>().Where(p => p.Id == guser.Id.ToString()).Any())
                return false;

            var svvrs = await profile.Value.GetSurvivors();
            var svvrsResource = await svvrs.CalcSurvivorFORTs();
            var resources = await profile.Value.CalcResearchFORTs();
            var AccountPowerLevel = await SurvivorStaticData.CalcEnergyByFORT(svvrsResource + resources);
            var mSchematics = profile.Value.AmountOfMythicSchematics();
            var frostnite2019 = profile.Value.DoneEliteFrostnite2019();
            var mockPveProfile = new FortnitePVEProfile
            {
                EpicId = profile.Value.profileChanges.First().profile.accountId,
                PlayerName = profile.Key,
                AccountPowerLevel = AccountPowerLevel,
                NumMythicSchematics = mSchematics,
                EliteFortnite2019 = frostnite2019
            };

            if (AccountPowerLevel < 16)
                mockPveProfile.Map = MapRoles.Stonewood;
            else if (AccountPowerLevel < 46)
                mockPveProfile.Map = MapRoles.Plankerton;
            else if (AccountPowerLevel < 70)
                mockPveProfile.Map = MapRoles.CannyValley;
            else
                mockPveProfile.Map = MapRoles.TwinePeaks;

            StatAttribute stats = profile.Value.profileChanges.First().profile.stats["attributes"];
            if (stats.rewards_claimed_post_max_level.HasValue)
            {
                if (stats != null)
                    mockPveProfile.CommanderLevel = (stats.level + stats.rewards_claimed_post_max_level.Value);
            }
            else
            {
                if (stats != null)
                    mockPveProfile.CommanderLevel = stats.level;
            }
            if (stats.collection_book != null)
                mockPveProfile.CollectionBookLevel = stats.collection_book.maxBookXpLevelAchieved;

            UpdateDatabaseUserProfile(mockPveProfile.EpicId, guser, msg, nameTag, GameUserMode.PVE);
            StoredProcedure.SP_TABLE_FortnitePVEProfile_Update(mockPveProfile);
            await UpdateDiscordPVEProfileAsync(mockPveProfile, nameTag, guser, msg, IsNameLocked, PVEDecimals);
            return true;
        }

        public async Task<bool> UpdateDatabasePVPProfileAsync(KeyValuePair<string, global::fortniteLib.Responses.Pvp.BattleRoyaleStats> profile, IGuildUser guser, IUserMessage msg = null, bool nameTag = false, bool IsNameLocked = false)
        {
            if (_uow.Db<BlackListUser>().Where(p => p.Id == guser.Id.ToString()).Any())
                return false;

            var mockPVPProfile = new FortnitePVPProfile
            {
                PlayerName = profile.Key,
                EpicId = profile.Value.accountId,
                PvpWinSolo = profile.Value.stats.BR_Placetop1(MatchType.solo, Platform.all, true),
                PvpWinDuo = profile.Value.stats.BR_Placetop1(MatchType.duo, Platform.all, true),
                PvpWinSquad = profile.Value.stats.BR_Placetop1(MatchType.squad, Platform.all, true)
            };
            UpdateDatabaseUserProfile(mockPVPProfile.EpicId, guser, msg, nameTag, GameUserMode.PVP_WIN_ALL);
            StoredProcedure.SP_TABLE_FortnitePVPProfile_Update(mockPVPProfile);
            await UpdateDiscordPVPProfileAsync(mockPVPProfile, nameTag, guser, msg, IsNameLocked);
            return true;
        }

        public FortniteUser UpdateDatabaseUserProfile(string EpicId, IGuildUser guser, IUserMessage msg = null, bool nameTag = false, GameUserMode gameUserMode = GameUserMode.PVE)
        {
            var mockDiscordUser = new FortniteUser
            {
                Id = guser.Id.ToString(),
                EpicId = EpicId,
                NameTag = nameTag,
                IsValidName = true,
                GameUserMode = gameUserMode
            };
            StoredProcedure.SP_TABLE_FortniteUser_Update(mockDiscordUser);
            return mockDiscordUser;
        }

        public async Task<bool> UpdateDiscordPVEProfileAsync(FortnitePVEProfile mockUser, bool nameTag, IGuildUser guser, IUserMessage msg = null, bool IsNameLocked = false, bool PVEDecimals = true)
        {
            if (guser.IsServerOwner())
                return true;

            var gconfig = Guild.GetConfig(guser.Guild.Id.ToString());
            var UserRoles = await guser.GetUserRolesAsync(gconfig.Owner.DefaultLanguage);
            var addRole = true;
            var curUser = await guser.Guild.GetCurrentUserAsync(CacheMode.AllowDownload, Core.Utils.RequestOption);
            if (curUser.GuildPermissions.ManageRoles)
            {
                try
                {
                    foreach (var role in UserRoles)
                    {
                        if (role.Key == mockUser.Map)
                        {
                            addRole = false;
                            continue;
                        }
                        guser.RemoveRoleAsync(role.Value, Core.Utils.RequestOption).Wait();
                    }
                }
                catch (Exception e)
                {
                }

                if (!gconfig.Event.MythicSKStates.Active)//mythic storm king role remover
                {
                    try
                    {
                        ulong msk_roleId = ulong.Parse(gconfig.Event.MythicSKStates.RoleIdToMythicSK);
                        var userRole = guser.RoleIds.Any(id => id == msk_roleId);
                        if (userRole)
                        {
                            var role = guser.Guild.GetRole(msk_roleId);
                            guser.RemoveRoleAsync(role, Core.Utils.RequestOption).Wait();
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
                if (!gconfig.Event.EliteFrostnite2019s.Active)//elite frostnite 2019 role remover
                {
                    try
                    {
                        ulong msk_roleId = ulong.Parse(gconfig.Event.EliteFrostnite2019s.RoleId);
                        var userRole = guser.RoleIds.Any(id => id == msk_roleId);
                        if (userRole)
                        {
                            var role = guser.Guild.GetRole(msk_roleId);
                            guser.RemoveRoleAsync(role, Core.Utils.RequestOption).Wait();
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            var mn_NickName = curUser.GuildPermissions.ManageNicknames;
            var nm_Role = curUser.GuildPermissions.ManageRoles;

            if (msg != null && IsNameLocked)
                await msg.SetLockAsync();

            if ((mn_NickName || nm_Role))
            {
                try
                {
                    //
                    if (mockUser.NumMythicSchematics > 0 && gconfig.Event.MythicSKStates.Active)
                    {
                        ulong msk_roleId = ulong.Parse(gconfig.Event.MythicSKStates.RoleIdToMythicSK);
                        var userRole = guser.RoleIds.Any(id => id == msk_roleId);
                        if (!userRole)
                        {
                            var guild_msk_role = guser.Guild.GetRole(msk_roleId);
                            guser.AddRoleAsync(guild_msk_role).Wait();
                        }
                    }
                    if (mockUser.EliteFortnite2019 && gconfig.Event.EliteFrostnite2019s.Active)
                    {
                        ulong msk_roleId = ulong.Parse(gconfig.Event.EliteFrostnite2019s.RoleId);
                        var userRole = guser.RoleIds.Any(id => id == msk_roleId);
                        if (!userRole)
                        {
                            var guild_efn_role = guser.Guild.GetRole(msk_roleId);
                            guser.AddRoleAsync(guild_efn_role).Wait();
                        }
                    }
                    //
                    var newPowerName = ModifyPVETag(mockUser.PlayerName, mockUser.AccountPowerLevel, nameTag, PVEDecimals);
                    var sameName = newPowerName != guser?.Nickname;

                    if (!addRole && !sameName)
                        return true;
                    else if (!addRole && IsNameLocked)
                        return true;

                    guser.ModifyAsync((o) =>
                    {
                        if (curUser.GuildPermissions.ManageNicknames && IsNameLocked == false)
                            o.Nickname = newPowerName;

                        if (addRole && curUser.GuildPermissions.ManageRoles)
                        {
                            var CurrentGuildMapRoles = guser.Guild.GetMapRolesAsync(gconfig.Owner.DefaultLanguage).Result;
                            var rl = CurrentGuildMapRoles.FirstOrDefault(p => p.Key == mockUser.Map);
                            if (rl.Value != null)
                                guser.AddRoleAsync(rl.Value, Core.Utils.RequestOption).Wait();
                        }
                    }, Core.Utils.RequestOption).Wait();

                    if (msg != null && guser.Id == guser.Guild.OwnerId)
                        await msg.SetErrorAsync();
                }
                catch (Exception e)
                {
                    if (msg != null)
                    {
                        await msg.SetErrorAsync();
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<bool> UpdateDiscordPVPProfileAsync(FortnitePVPProfile mockUser, bool nameTag, IGuildUser guser, IUserMessage msg = null, bool IsNameLocked = false)
        {
            if (guser.IsServerOwner())
                return true;

            var curUser = await guser.Guild.GetCurrentUserAsync(CacheMode.AllowDownload, Core.Utils.RequestOption);
            var mn_NickName = curUser.GuildPermissions.ManageNicknames;

            if (msg != null && IsNameLocked)
                await msg.SetLockAsync();

            if (mn_NickName)
            {
                try
                {
                    var newWinName = ModifyPVPTag(mockUser.PlayerName, mockUser.PvpCurrentModeWins(GameUserMode.PVP_WIN_ALL), nameTag);
                    var changeName = newWinName != guser?.Nickname;

                    if (!changeName)
                        return true;
                    else if (IsNameLocked)
                        return true;

                    guser.ModifyAsync((o) =>
                    {
                        if (curUser.GuildPermissions.ManageNicknames && IsNameLocked == false)
                            o.Nickname = newWinName;
                    }, Core.Utils.RequestOption).Wait();
                }
                catch (Exception e)
                {
                    if (msg != null)
                    {
                        await msg.SetErrorAsync();
                        return false;
                    }
                }
            }
            return true;
        }

        private void LockState(string guildId, string userId, bool isLocked)
        {
            var nstate = _uow.Db<NameState>()
                .Where(p => p.DiscordServerId == guildId && p.FortniteUserId == userId)
                .FirstOrDefault();

            if (nstate == null)
            {
                nstate = new NameState
                {
                    DiscordServerId = guildId,
                    FortniteUserId = userId,
                    InQueue = false,
                    LockName = isLocked
                };
                _uow.Db<NameState>()
                    .Add(nstate);
            }
            else
            {
                nstate.LockName = isLocked;
                _uow.Db<NameState>()
                    .Update(nstate);
            }
            _uow.Commit();
        }

        private bool disposedValue = false; 
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _blacklist = null;
                    _guild = null;
                    _priority = null;
                    _storedProcedure = null;
                    _user = null;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}