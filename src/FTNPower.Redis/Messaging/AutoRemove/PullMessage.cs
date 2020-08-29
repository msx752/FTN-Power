using FTNPower.Core.Interfaces;
using FTNPower.Model.Interfaces;
using FTNPower.Redis.Messaging;
using System;
using System.Threading.Tasks;

namespace FTNPower.Redis.Messaging.AutoRemove
{
    public class PullMessage : BasePull<ReadyToRemove>
    {
        public PullMessage(IDiscordRestApi discordApi, IRedisService redisService) : base("AutoDiscordMsgRemove", redisService)
        {
            DiscordApi = discordApi;
            DelayRetry = new TimeSpan(0, 0, 0, 1, 0);
            DelayAfterError = new TimeSpan(0, 0, 2);
            DelayOnSucceed = DelayRetry;
            OnException += PullMessage_OnException;
            OnAction += PullMessage_OnAction;
        }

        private IDiscordRestApi DiscordApi { get; set; }
        public override void Start()
        {
            Global.Log.Information("{lt}: {RedisServiceName} is started", "Service", GetType().Name);
            base.Start();
        }

        private Task PullMessage_OnAction(ReadyToRemove arg)
        {
            var spanTime = DateTimeOffset.UtcNow - arg.DeleteTime;
            if (spanTime.TotalSeconds >= 0)
            {
                DiscordApi.DeleteTextMessage(arg.ChannelId, arg.MessageId).Wait();
            }
            else
            {
                Redis.ListLeftPush(QueueName, arg);
            }
            return Task.CompletedTask;
        }

        private Task PullMessage_OnException(BasePullEventArgs arg)
        {
            Global.Log.Exception(arg.Error, exceptionNote: $"Service is {GetType().Name}");
            return Task.CompletedTask;
        }
    }
}