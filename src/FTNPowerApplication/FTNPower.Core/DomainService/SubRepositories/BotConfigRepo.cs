using FTNPower.Core.ApplicationService;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Tables;
using Global;
using System;

namespace FTNPower.Core.DomainService.SubRepositories
{
    public class BotConfigRepo
    {
        private readonly IRedisService _redis;
        private readonly IUnitOfWork _uow;

        public BotConfigRepo(IUnitOfWork uow, IRedisService redisService)
        {
            _uow = uow;
            _redis = redisService;
        }

        public BotConfig Config
        {
            get
            {
                BotConfig cachedConfig = _redis.JsonGet<BotConfig>();
                if (cachedConfig == null)
                {
                    cachedConfig = _uow.Db<BotConfig>().GetById(DIManager.Services.DiscordBotConfigs().ProjectName);
                    _redis.JsonSet(cachedConfig, expiry: new TimeSpan(6, 0, 0));
                }
                return cachedConfig;
            }
        }
    }
}