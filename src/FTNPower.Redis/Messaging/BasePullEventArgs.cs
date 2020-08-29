using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Redis.Messaging
{
    public class BasePullEventArgs : EventArgs
    {
        public Exception Error { get; set; }
        public RedisValue Result { get; set; }
        public BasePullEventArgs(Exception e, RedisValue result)
        {
            Error = e;
            Result = result;
        }
    }
}
