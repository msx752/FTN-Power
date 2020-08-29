
using FTNPower.Core.Interfaces;
using FTNPower.Data;
using Global;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Linq;

namespace FTNPower.Redis
{
    public class RedisService : IRedisService
    {
        private readonly object _lock_Connection_SET = new object();
        private readonly ConfigurationOptions configuration = null;
        private Lazy<IConnectionMultiplexer> _Connection = null;
        public RedisService()
        {
            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);
            var rconf = DIManager.Services.GetRedisConfigs();
            configuration = new ConfigurationOptions()
            {
                EndPoints = { { rconf.EndPoint, rconf.Port }, },
                Password = rconf.Password,
                ClientName = rconf.ClientName,
                ConnectTimeout = 5000,
                SyncTimeout = 1000,
                ReconnectRetryPolicy = new LinearRetry(5000),
                AbortOnConnectFail = false,
            };
            if (true)
            {
                configuration.ConnectTimeout = 15000;
                configuration.SyncTimeout = 10000;
                configuration.ReconnectRetryPolicy = new LinearRetry(20000);
            }

            _Connection = new Lazy<IConnectionMultiplexer>(() =>
            {
                IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configuration);
                redis.ErrorMessage += _Connection_ErrorMessage;
                redis.InternalError += _Connection_InternalError;
                redis.ConnectionFailed += _Connection_ConnectionFailed;
                redis.ConnectionRestored += _Connection_ConnectionRestored;
                return redis;
            });
        }
        /// <summary>
        /// ListLeftPush with flag is FireAndForget
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long ListLeftPush(string queueName, object value, CommandFlags flags = CommandFlags.FireAndForget)
        {
            return Connection
                .GetDatabase()
                .ListLeftPush(queueName, value.ToJsonString(), flags: flags);
        }
        public IConnectionMultiplexer Connection
        {
            get
            {
                return _Connection.Value;
            }
        }

        private void _Connection_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Global.Log.Error("Redis: Connection is failed, type: {RedisFailureType}", e.FailureType);
        }

        private void _Connection_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Global.Log.Error("Redis: Connection is restored, type: {RedisFailureType}", e.FailureType);
        }

        private void _Connection_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Global.Log.Error("Redis: Connection has error, errmsg: {RedisErrorMessage}", e.Message);
        }

        private void _Connection_InternalError(object sender, InternalErrorEventArgs e)
        {
            Global.Log.Error("Redis: Connection has internal error");
        }
        public string Key<T>(object id)
        {
            return $"{typeof(T)}|{id}";
        }
        public string Key<T>()
        {
            return $"{typeof(T)}";
        }
        public bool JsonSet(RedisKey key, object value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (value == null) return false;
            string json = JsonConvert.SerializeObject(value);
            return Connection.GetDatabase().StringSet(key, json, expiry, when, flags);
        }
        public bool JsonSet<T>(T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            if (value == null) return false;
            return JsonSet(Key<T>(), value, expiry, when, flags);
        }
        public T JsonGet<T>(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            RedisValue str = Connection.GetDatabase().StringGet(key, flags);
            return str.ToObject<T>();
        }
        public T JsonGet<T>(CommandFlags flags = CommandFlags.None)
        {
            return JsonGet<T>(Key<T>(), flags);
        }
        public bool JsonDelete(RedisKey key, CommandFlags flags = CommandFlags.None)
        {
            return Connection.GetDatabase().KeyDelete(key, flags);
        }
        public bool JsonDelete<T>(CommandFlags flags = CommandFlags.None)
        {
            return JsonDelete(Key<T>(), flags);
        }
    }
}