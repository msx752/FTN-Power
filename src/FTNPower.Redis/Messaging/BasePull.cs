
using FTNPower.Core.Interfaces;
using FTNPower.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTNPower.Redis.Messaging
{
    public class BasePull<T> where T : class
    {
        private Thread _thread = null;
        public BasePull(string queueName, IRedisService redisService)
        {
            Redis = redisService ?? throw new ArgumentNullException();
            QueueName = queueName ?? throw new ArgumentNullException();
            DelayRetry = new TimeSpan(0, 0, 1, 0);
            DelayAfterError = new TimeSpan(0, 0, 10);
            DelayOnSucceed = new TimeSpan(0, 0, 2);
            AutoDelete = true;
        }
        public string QueueName { get; private set; }
        public IRedisService Redis { get; set; }
        /// <summary>
        /// default: 1 minutes to  try if queue empty
        /// </summary>
        public TimeSpan DelayRetry { get; set; }
        /// <summary>
        /// default 10 second to try after error
        /// </summary>
        public TimeSpan DelayAfterError { get; set; }
        /// <summary>
        /// default 4 seconds
        /// </summary>
        public TimeSpan DelayOnSucceed { get; set; }
        public bool AutoDelete { get; set; }
        public virtual void Start()
        {
            _thread = new Thread(Worker);
            _thread.Priority = ThreadPriority.Normal;
            _thread.Start();
        }
        internal event Func<BasePullEventArgs, Task> OnException = null;
        internal event Func<T, Task> OnAction = null;
        private void Worker()
        {
            var rdb = Redis.Connection.GetDatabase();
            while (true)
            {
                RedisValue result = RedisValue.Null;
                try
                {

                    var currentElementCounts = (int)rdb.ListLength(QueueName);
                    if (currentElementCounts == 0)
                    {
                        Thread.Sleep(DelayRetry);
                        continue;
                    }
                    result = rdb.ListRightPop(QueueName);
                    if (!result.HasValue || result.IsNullOrEmpty)
                    {
                        Thread.Sleep(DelayRetry);
                        continue;
                    }
                    T currentItem = result.ToObject<T>();
                    if (OnAction != null)
                        OnAction(currentItem).Wait();
                    Thread.Sleep(DelayOnSucceed);
                    continue;
                }
                catch (Exception e)
                {
                    if (OnException != null)
                        OnException(new BasePullEventArgs(e, result)).Wait();
                    Thread.Sleep(DelayAfterError);
                }
            }
        }
    }
}
