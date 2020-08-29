using StackExchange.Redis;
using System;

namespace FTNPower.Core.Interfaces
{
    public interface IRedisService
    {
        bool JsonDelete(RedisKey key, CommandFlags flags = CommandFlags.None);
        IConnectionMultiplexer Connection { get; }
        bool JsonDelete<T>(CommandFlags flags = CommandFlags.None);
        T JsonGet<T>(CommandFlags flags = CommandFlags.None);

        T JsonGet<T>(RedisKey key, CommandFlags flags = CommandFlags.None);
        string Key<T>(object id);
        string Key<T>();
        bool JsonSet<T>(T value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None);
        bool JsonSet(RedisKey key, object value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None);

        long ListLeftPush(string queueName, object value, CommandFlags flags = CommandFlags.FireAndForget);
    }
}