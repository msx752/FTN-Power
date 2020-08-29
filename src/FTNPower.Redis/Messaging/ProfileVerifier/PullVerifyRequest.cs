using Discord;
using Fortnite.Api;
using FTNPower.Core;
using FTNPower.Core.Interfaces;
using FTNPower.Data.Migrations;
using FTNPower.Model;
using FTNPower.Model.Interfaces;
using FTNPower.Model.Tables;
using FTNPower.Model.WebsiteModels;
using FTNPower.Redis.Messaging;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace FTNPower.Redis.Messaging.ProfileVerifier
{
    public class PullVerifyRequest : BasePull<ReadyToVerify>
    {
        public PullVerifyRequest(IEpicFriendListApi fapilst, BotContext context, IDiscordRestApi discordApi, IRedisService redisService) : base("ProfileVerificationQueue", redisService)
        {
            DiscordApi = discordApi;
            Context = context;
            FriendListApi = fapilst;
            DelayRetry = new TimeSpan(0, 0, 0, 0, 250);
            DelayAfterError = new TimeSpan(0, 0, 1);
            DelayOnSucceed = DelayRetry;
            OnException += PullMessage_OnException;
            OnAction += PullMessage_OnAction;
        }

        private BotContext Context { get; set; }
        private IDiscordRestApi DiscordApi { get; set; }
        private IEpicFriendListApi FriendListApi { get; set; }
        public override void Start()
        {
            Global.Log.Information("{lt}: {RedisServiceName} is started", "Service", GetType().Name);
            base.Start();
        }

        private Task PullMessage_OnAction(ReadyToVerify arg)
        {
            if (arg.JustDeQueue)
            {
                FriendListApi.DeclineFriendRequest(arg.EpicId).Wait();
                return Task.CompletedTask;
            }
            var spanTime = DateTimeOffset.UtcNow - arg.Expire;
            if (spanTime.TotalSeconds >= 0 && FriendListApi.IsAuthorized)
            {
                try
                {
                    var usr = DiscordApi.GetApi.GetUserAsync(arg.DiscordUserId.ToUlong()).Result;
                    var channel = (IMessageChannel)DiscordApi.GetApi.GetChannelAsync(arg.ChannelId).Result;
                    IUserMessage msg = null;
                    try
                    {
                        msg = (IUserMessage)channel.GetMessageAsync(arg.MessageId, options: Core.Utils.RequestOption).Result;
                    }
                    catch (Exception e)
                    {
                        return Task.CompletedTask;
                    }

                    if (usr != null)
                    {
                        FortniteUser dbAccount = Context.FortniteUsers.Find(arg.DiscordUserId);
                        if (dbAccount == null)
                        {
                            msg?.SetErrorAsync()?.Wait();
                            return Task.CompletedTask;
                        }

                        var verificationResult = FriendListApi.DeclineFriendRequest(arg.EpicId).Result;
                        if (verificationResult)
                        {
                            if (dbAccount.IsValidName)
                            {
                                dbAccount.VerifiedProfile = true;
                                dbAccount.LastUpDateTime = DateTimeOffset.UtcNow;
                                Context.Entry(dbAccount).State = EntityState.Modified;
                                Context.SaveChanges();
                                msg?.SetSuccessAsync()?.Wait();
                            }
                            else
                            {
                                msg?.SetQuestionMarkAsync()?.Wait();
                            }
                        }
                        else
                        {
                            msg?.SetErrorAsync()?.Wait();
                        }
                    }
                    else
                    {
                        msg?.SetErrorAsync()?.Wait();
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    var verifyFinished = Context.VerifyOrders.Find(arg.EpicId);
                    if (verifyFinished != null)
                    {
                        Context.VerifyOrders.Remove(verifyFinished);
                        Context.SaveChanges();
                    }
                }
            }
            else
            {
                Redis
                    .ListLeftPush(QueueName, arg, flags: CommandFlags.FireAndForget);
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