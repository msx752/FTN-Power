using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Redis.Messaging.AutoRemove
{
    public class ReadyToRemove
    {
        public ReadyToRemove(ulong channelId, ulong messageId, int durationSecond = 1)
        {
            DeleteTime = DateTimeOffset.UtcNow.AddSeconds(durationSecond);
            ChannelId = messageId;
            MessageId = channelId;
        }
        public ulong ChannelId { get; private set; }
        public ulong MessageId { get; private set; }
        public DateTimeOffset DeleteTime { get; private set; }
        public override string ToString()
        {
            return $"{ChannelId}|{MessageId}";
        }
    }
}
