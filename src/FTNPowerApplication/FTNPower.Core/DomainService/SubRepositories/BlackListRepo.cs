using FTNPower.Core.ApplicationService;
using FTNPower.Core.Interfaces;
using FTNPower.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FTNPower.Core.DomainService.SubRepositories
{
    public class BlacklistRepo
    {
        private readonly IRedisService _redis;
        private readonly IUnitOfWork _uow;

        public BlacklistRepo(IUnitOfWork uow, IRedisService redisService)
        {
            _uow = uow;
            _redis = redisService;
        }

        public IEnumerable<BlackListGuild> BlacklistGuilds
        {
            get
            {
                IEnumerable<BlackListGuild> cachedConfig = _redis.JsonGet<List<BlackListGuild>>();
                if (cachedConfig == null)
                {
                    cachedConfig = _uow.Db<BlackListGuild>().All();
                    _redis.JsonSet(cachedConfig, expiry: new TimeSpan(6, 0, 0));
                }
                return cachedConfig;
            }
        }

        public IEnumerable<BlackListUser> BlacklistUsers
        {
            get
            {
                IEnumerable<BlackListUser> cachedConfig = _redis.JsonGet<List<BlackListUser>>();
                if (cachedConfig == null)
                {
                    cachedConfig = _uow.Db<BlackListUser>().All();
                    _redis.JsonSet(cachedConfig, expiry: new TimeSpan(6, 0, 0));
                }
                return cachedConfig;
            }
        }

        public void AddGuildToBlacklist(string guildId)
        {
            if (BlacklistGuilds.Any(p => p.Id == guildId))
                return;

            var bl = new BlackListGuild() { Id = guildId.ToString() };
            _uow.Db<BlackListGuild>().Add(bl);
            _uow.Commit();
            _redis.JsonDelete<List<BlackListGuild>>();
        }

        public void AddUserToBlacklist(string userId)
        {
            if (BlacklistUsers.Any(p => p.Id == userId))
                return;

            var bl = new BlackListUser() { Id = userId.ToString() };
            _uow.Db<BlackListUser>().Add(bl);
            _uow.Commit();
            _redis.JsonDelete<List<BlackListUser>>();
        }

        public void RemoveGuildFromBlacklist(string guildId)
        {
            var gld = BlacklistGuilds.FirstOrDefault(p => p.Id == guildId);
            if (gld != null)
            {
                _uow.Db<BlackListGuild>().Delete(gld);
                _uow.Commit();
                _redis.JsonDelete<List<BlackListGuild>>();
            }
        }

        public void RemoveUserFromBlacklist(string userId)
        {
            var usr = BlacklistUsers.FirstOrDefault(p => p.Id == userId);
            if (usr != null)
            {
                _uow.Db<BlackListUser>().Delete(usr);
                _uow.Commit();
                _redis.JsonDelete<List<BlackListUser>>();
            }
        }
    }
}