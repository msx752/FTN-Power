using Discord;
using Fortnite.Model.Responses.QueryProfile;
using fortniteLib.Responses.Pvp;
using FTNPower.Core.ApplicationService;
using FTNPower.Core.DomainService.SubRepositories;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTNPower.Core.DomainService
{
    public interface IFTNPowerRepository : IDisposable
    {
        BlacklistRepo Blacklist { get; }
        BotConfigRepo Bot { get; }
        GuildRepo Guild { get; }
        PriorityRepo Priority { get; }
        StoredProcedureRepo StoredProcedure { get; }
        UserRepo User { get; }

        int Commit();

        IEFRepository<TEntity> Db<TEntity>() where TEntity : class;

        void LockName(string guildId, string userId);

        string ModifyPVETag(string PlayerName, double AccountPowerLevel, bool NameTag, bool PVEDecimals, bool forceLeft = false);

        string ModifyPVPTag(string PlayerName, int currentTotalWins, bool NameTag, bool forceLeft = false);

        void ResetFeatures(string discordId);

        void UnLockName(string guildId, string userId);

        Task<bool> UpdateDatabasePVEProfileAsync(KeyValuePair<string, IQueryProfile> profile, IGuildUser guser, IUserMessage msg = null, bool nameTag = false, bool IsNameLocked = false, bool PVEDecimals = true);

        Task<bool> UpdateDatabasePVPProfileAsync(KeyValuePair<string, BattleRoyaleStats> profile, IGuildUser guser, IUserMessage msg = null, bool nameTag = false, bool IsNameLocked = false);

        FortniteUser UpdateDatabaseUserProfile(string EpicId, IGuildUser guser, IUserMessage msg = null, bool nameTag = false, GameUserMode gameUserMode = GameUserMode.PVE);

        Task<bool> UpdateDiscordPVEProfileAsync(FortnitePVEProfile mockUser, bool nameTag, IGuildUser guser, IUserMessage msg = null, bool IsNameLocked = false, bool PVEDecimals = true);

        Task<bool> UpdateDiscordPVPProfileAsync(FortnitePVPProfile mockUser, bool nameTag, IGuildUser guser, IUserMessage msg = null, bool IsNameLocked = false);
        new void Dispose();
    }
}