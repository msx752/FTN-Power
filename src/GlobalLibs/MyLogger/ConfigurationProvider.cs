using Global.ConfigModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Global
{
    public static class ConfigurationProvider
    {
        private static DatabaseConfigs _DatabaseConfigs;
        private static DiscordBotConfigs _DiscordBotConfigs;
        private static EpicApiConfigs _EpicApiConfigs;
        private static EpicFriendListApiConfigs _EpicFriendListApiConfigs;
        private static ExternalApiConfigs _ExternalApiConfigs;
        private static FortniteQueueApiConfigs _FortniteQueueApiConfigs;
        private static IConfiguration _IConfiguration;
        private static ImageServiceConfigs _ImageServiceConfigs;
        private static RedisConfigs _RedisConfigs;
        private static SeqConfigs _SeqConfigs;
        private static IConfiguration _IConfiguration_backup;
        public static void SetManualConfiguration(IConfiguration configuration)
        {
            _IConfiguration_backup = configuration;
        }

        public static SeqConfigs SeqConfigs(this IServiceProvider serviceProvider)
        {
            if (_SeqConfigs == null)
                _SeqConfigs = GetConfiguration(serviceProvider).GetSection("SeqConfigs").Get<SeqConfigs>();
            return _SeqConfigs;
        }

        public static DiscordBotConfigs DiscordBotConfigs(this IServiceProvider serviceProvider)
        {
            if (_DiscordBotConfigs == null)
                _DiscordBotConfigs = GetConfiguration(serviceProvider).GetSection("DiscordBotConfigs").Get<DiscordBotConfigs>();
            return _DiscordBotConfigs;
        }

        public static EpicApiConfigs EpicApiConfigs(this IServiceProvider serviceProvider)
        {
            if (_EpicApiConfigs == null)
                _EpicApiConfigs = GetConfiguration(serviceProvider).GetSection("EpicApiConfigs").Get<EpicApiConfigs>();
            return _EpicApiConfigs;
        }

        public static EpicFriendListApiConfigs EpicFriendListApiConfigs(this IServiceProvider serviceProvider)
        {
            if (_EpicFriendListApiConfigs == null)
                _EpicFriendListApiConfigs = GetConfiguration(serviceProvider).GetSection("EpicFriendListApiConfigs").Get<EpicFriendListApiConfigs>();
            return _EpicFriendListApiConfigs;
        }

        public static ExternalApiConfigs ExternalApiConfigs(this IServiceProvider serviceProvider)
        {
            if (_ExternalApiConfigs == null)
                _ExternalApiConfigs = GetConfiguration(serviceProvider).GetSection("ExternalApiConfigs").Get<ExternalApiConfigs>();
            return _ExternalApiConfigs;
        }

        public static FortniteQueueApiConfigs FortniteQueueApiConfigs(this IServiceProvider serviceProvider)
        {
            if (_FortniteQueueApiConfigs == null)
                _FortniteQueueApiConfigs = GetConfiguration(serviceProvider).GetSection("FortniteQueueApiConfigs").Get<FortniteQueueApiConfigs>();
            return _FortniteQueueApiConfigs;
        }

        public static IConfiguration GetConfiguration(this IServiceProvider serviceProvider)
        {
            try
            {
                if (_IConfiguration == null)
                    _IConfiguration = serviceProvider.GetRequiredService<IConfiguration>();
            }
            catch (Exception)
            {
                if (_IConfiguration_backup != null)
                    _IConfiguration = _IConfiguration_backup;
                else throw new NotImplementedException("configuration is not defined corretly");
            }
            return _IConfiguration;
        }

        public static DatabaseConfigs GetDatabaseConfigs(this IServiceProvider serviceProvider)
        {
            if (_DatabaseConfigs == null)
                _DatabaseConfigs = GetConfiguration(serviceProvider).GetSection("DatabaseConfigs").Get<DatabaseConfigs>();
            return _DatabaseConfigs;
        }

        public static RedisConfigs GetRedisConfigs(this IServiceProvider serviceProvider)
        {
            if (_RedisConfigs == null)
                _RedisConfigs = GetConfiguration(serviceProvider).GetSection("RedisConfigs").Get<RedisConfigs>();
            return _RedisConfigs;
        }

        public static ImageServiceConfigs ImageServiceConfigs(this IServiceProvider serviceProvider)
        {
            if (_ImageServiceConfigs == null)
                _ImageServiceConfigs = GetConfiguration(serviceProvider).GetSection("ImageServiceConfigs").Get<ImageServiceConfigs>();
            return _ImageServiceConfigs;
        }
    }
}