using FTNPower.Core.ApplicationService;
using FTNPower.Core.Interfaces;
using FTNPower.Data.Migrations;
using FTNPower.Model.Tables;
using FTNPower.Model.Tables.StoredProcedures;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FTNPower.Core.DomainService.SubRepositories
{
    public class StoredProcedureRepo
    {
        private readonly IRedisService _redis;
        private readonly IUnitOfWork _uow;

        public StoredProcedureRepo(IUnitOfWork uow, IRedisService redisService)
        {
            _uow = uow;
            _redis = redisService;
        }

        public int CallStoredProcedure(string query, params SqlParameter[] parameters)
        {
            try
            {
                return _uow.Query(query, parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<T> CallStoredProcedureQuery<T>(string query, params SqlParameter[] parameters) where T : class
        {
            try
            {
                return _uow.Query<T>(query, parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<EpicTopUser> SP_GlobalTop20()
        {
            return CallStoredProcedureQuery<EpicTopUser>("dbo.SP_GlobalTop20Users");
        }

        public void SP_IJTABLE_UserNotValid(string guildId, string userId)
        {
            var GuildId = new SqlParameter("@GuildId", guildId);
            var UserId = new SqlParameter("@UserId", userId);
            CallStoredProcedure("dbo.SP_IJTABLE_UserNotValid @GuildId, @UserId", GuildId, UserId);
        }

        public List<ReadyToUpdate> SP_ListOfReadyToUpdate()
        {
            return CallStoredProcedureQuery<ReadyToUpdate>("dbo.SP_ListOfReadyToUpdate");
        }

        public List<EpicTopUser> SP_LocalTop20Async(string guildId)
        {
            var clientid = new SqlParameter("@GuildId", guildId);
            return CallStoredProcedureQuery<EpicTopUser>("dbo.SP_LocalTop20Users @GuildId", clientid);
        }

        public void SP_NameState_ClearQueueByGuildId(string guildId)
        {
            var GuildId = new SqlParameter("@GuildId", guildId);
            CallStoredProcedure("dbo.SP_NameState_ClearQueueByGuildId @GuildId", GuildId);
        }

        public void SP_NameState_InQueue(string guildId, string userId, bool inQueue)
        {
            var GuildId = new SqlParameter("@GuildId", guildId);
            var UserId = new SqlParameter("@UserId", userId);
            var InQueue = new SqlParameter("@InQueue", inQueue);
            CallStoredProcedure("dbo.SP_NameState_InQueue @GuildId, @UserId, @InQueue", GuildId, UserId, InQueue);
        }

        /// <summary>
        /// kullanıcı discorddan çıktığında NameState verisi bu kullanıcı için geçersiz böylece siliyoruz ve top 20 düzgün çalışıyor
        /// </summary>
        /// <param name="guildId"></param>
        /// <param name="userId"></param>

        public void SP_RemoveNameStateForDiscord(string guildId, string userId)
        {
            var GuildId = new SqlParameter("@GuildId", guildId);
            var UserId = new SqlParameter("@UserId", userId);
            CallStoredProcedure("dbo.SP_RemoveNameStateForDiscord @GuildId, @UserId", GuildId, UserId);
        }

        public int SP_TABLE_FortnitePVEProfile_Update(FortnitePVEProfile user)
        {
            var EpicId = new SqlParameter("@EpicId", user.EpicId);
            var PlayerName = new SqlParameter("@PlayerName", user.PlayerName);
            var AccountPowerLevel = new SqlParameter("@AccountPowerLevel", user.AccountPowerLevel);
            var Map = new SqlParameter("@Map", user.Map);
            var CommanderLevel = new SqlParameter("@CommanderLevel", user.CommanderLevel);
            var CollectionBookLevel = new SqlParameter("@CollectionBookLevel", user.CollectionBookLevel);
            var NumMythicSchematics = new SqlParameter("@NumMythicSchematics", user.NumMythicSchematics);
            var EliteFortnite2019 = new SqlParameter("@EliteFortnite2019", user.EliteFortnite2019);
            return CallStoredProcedure("dbo.SP_TABLE_FortnitePVEProfile_Update @EpicId, @PlayerName, @AccountPowerLevel, @Map, @CommanderLevel, @CollectionBookLevel, @NumMythicSchematics, @EliteFortnite2019",
                EpicId, PlayerName, AccountPowerLevel, Map, CommanderLevel, CollectionBookLevel, NumMythicSchematics, EliteFortnite2019);
        }

        public int SP_TABLE_FortnitePVPProfile_Update(FortnitePVPProfile user)
        {
            var EpicId = new SqlParameter("@EpicId", user.EpicId);
            var PlayerName = new SqlParameter("@PlayerName", user.PlayerName);
            var PvpWinSolo = new SqlParameter("@PvpWinSolo", user.PvpWinSolo);
            var PvpWinDuo = new SqlParameter("@PvpWinDuo", user.PvpWinDuo);
            var PvpWinSquad = new SqlParameter("@PvpWinSquad", user.PvpWinSquad);
            return CallStoredProcedure("dbo.SP_TABLE_FortnitePVPProfile_Update @EpicId, @PlayerName, @PvpWinSolo, @PvpWinDuo, @PvpWinSquad",
                EpicId, PlayerName, PvpWinSolo, PvpWinDuo, PvpWinSquad);
        }

        public int SP_TABLE_FortniteUser_Update(FortniteUser user)
        {
            var Id = new SqlParameter("@Id", user.Id)
            {
                Size = 20,
                SqlDbType = System.Data.SqlDbType.NVarChar
            };
            var EpicId = new SqlParameter("@EpicId", user.EpicId)
            {
                Size = 50,
                SqlDbType = System.Data.SqlDbType.NVarChar,
                IsNullable = true,
            };

            if (user.EpicId == null)
                EpicId.Value = DBNull.Value;

            var NameTag = new SqlParameter("@NameTag", user.NameTag);
            var IsValidName = new SqlParameter("@IsValidName", user.IsValidName);
            var GameUserMode = new SqlParameter("@GameUserMode", user.GameUserMode);
            return CallStoredProcedure("dbo.SP_TABLE_FortniteUser_Update @Id, @EpicId, @NameTag, @IsValidName, @GameUserMode",
                Id, EpicId, NameTag, IsValidName, GameUserMode);
        }

        public void SP_User_LastUpdateTime(string userId)
        {
            var UserId = new SqlParameter("@UserId", userId);
            CallStoredProcedure("dbo.SP_User_LastUpdateTime @UserId", UserId);
        }

        public void SP_User_ValidState(string userId, bool isValid)
        {
            var UserId = new SqlParameter("@UserId", userId);
            var IsValid = new SqlParameter("@IsValid", isValid);
            _ = CallStoredProcedure("dbo.SP_User_ValidState @UserId, @IsValid", UserId, IsValid);
        }

        public void SP_User_VerifiedProfile(string userId, bool verifiedProfile)
        {
            var Id = new SqlParameter("@Id", userId);
            var VerifiedProfile = new SqlParameter("@VerifiedProfile", verifiedProfile);
            CallStoredProcedure("dbo.SP_User_LastUpdateTime @Id, @VerifiedProfile", Id, VerifiedProfile);
        }
    }
}