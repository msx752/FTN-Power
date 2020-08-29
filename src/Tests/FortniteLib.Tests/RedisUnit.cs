using FTNPower.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fortnite.Tests
{
    public class RedisUnit
    {
        [Fact]
        public RedisService BasicRedisConnect()
        {
            RedisService redisService = new RedisService();
            var db = redisService.Connection.GetDatabase();
            var sub = redisService.Connection.GetSubscriber();
            Assert.True(db != null);
            Assert.True(sub != null);
            return redisService;
        }
    }
}
