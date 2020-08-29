using FTNPower.Core.Interfaces;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Redis
{
    public static class Utils
    {
        public static T ToObject<T>(this RedisValue bgr)
        {
            if (!bgr.HasValue)
                return default;
            T rtu = JsonConvert.DeserializeObject<T>(bgr);
            return rtu;
        }
    }
}
