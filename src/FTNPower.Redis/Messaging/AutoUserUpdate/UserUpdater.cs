
using Fortnite.Api;
using Fortnite.Model.Responses.QueryProfile;
using fortniteLib.Responses.Pvp;
using FTNPower.Core;
using FTNPower.Core.DomainService;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Enums;
using FTNPower.Model.Interfaces;
using FTNPower.Model.Tables;
using FTNPower.Model.Tables.StoredProcedures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FTNPower.Redis.Messaging.AutoUserUpdate
{
    //switched from rabbitmq > redisservice > raw sql querty, due to the performance issue 
    public class UserUpdater
    {
        private Thread _thread = null;

        public UserUpdater(IFTNPowerRepository fTNPowerRepository, IDiscordRestApi discordApi, Fortnite.Api.IEpicApi epicApi)
        {
            Repo = fTNPowerRepository;
            DiscordApi = discordApi;
            EpicApi = epicApi;
        }

        private Fortnite.Api.IEpicApi EpicApi { get; set; }
        private IDiscordRestApi DiscordApi { get; set; }
        private IFTNPowerRepository Repo { get; set; }

        public void Start()
        {
            Global.Log.Information("{lt}: {RedisServiceName} has been started", "Service", GetType().Name);
            _thread = new Thread(Worker);
            _thread.Priority = ThreadPriority.Normal;
            _thread.Start();
        }

        public IQueryable<string> GetPremiumDiscords()
        {
            var PremiumServers = Repo.Priority.Priorities
                             .ToList()
                             .Where(f => f.Id.StartsWith("s") && f.Deadline > DateTimeOffset.UtcNow)
                             .Select(x => x.Id.Substring(1)).AsQueryable();
            return PremiumServers;
        }
         
        public IQueryable<ReadyToUpdate> RetriewGuildUsers(string discordId)
        {
            IQueryable<ReadyToUpdate> ReadyToUpdateList = Repo.Db<NameState>()
                                           .All()
                                           .AsNoTracking()
                                           .Where(f => f.LockName == false &&
                                                       f.InQueue == false &&
                                                       f.DiscordServerId == discordId &&
                                                       f.FortniteUser.IsValidName &&
                                                       f.FortniteUser.EpicId != null)
                                          .Select(c => new ReadyToUpdate()
                                          {
                                              DiscordServerId = c.DiscordServerId,
                                              EpicId = c.FortniteUser.EpicId,
                                              FortniteUserId = c.FortniteUserId,
                                              GameUserMode = c.FortniteUser.GameUserMode,
                                              NameTag = c.FortniteUser.NameTag
                                          });
            return ReadyToUpdateList;
        }
        private void Worker()
        {
            while (true)
            {
                try
                {
                    IQueryable<string> PremiumServers = GetPremiumDiscords();
                    foreach (string discordId in PremiumServers)
                    {
                        IQueryable<ReadyToUpdate> ReadyToUpdateList = RetriewGuildUsers(discordId);
                        foreach (var arg in ReadyToUpdateList)
                        {
                            try
                            {
                                var guild = DiscordApi.GetApi.GetGuildAsync(arg.GUid).Result;
                                var guser = guild?.GetUserAsync(arg.Uid)?.Result;
                                if (guild == null || guser == null)
                                    continue;
                                KeyValuePair<string, IQueryProfile> responsePve = new KeyValuePair<string, IQueryProfile>();
                                KeyValuePair<string, BattleRoyaleStats> responsePvp = new KeyValuePair<string, BattleRoyaleStats>();
                                if (arg.GameUserMode == GameUserMode.PVE)
                                {
                                    responsePve = EpicApi.GetPVEProfileById(arg.EpicId).Result;
                                }
                                else
                                {
                                    responsePvp = EpicApi.GetPVPProfileById(arg.EpicId).Result;
                                    if (responsePvp.Value == null || responsePvp.Value.IsPrivate)
                                        continue;
                                }
                                if (responsePve.Value == null && responsePvp.Value == null)
                                    continue;
                                if (arg.GameUserMode == GameUserMode.PVE)
                                    Repo.UpdateDatabasePVEProfileAsync(responsePve, guser, null, arg.NameTag, false, arg.PVEDecimals).Wait();
                                else if (arg.GameUserMode == GameUserMode.PVP_WIN_ALL)
                                    Repo.UpdateDatabasePVPProfileAsync(responsePvp, guser, null, arg.NameTag, false).Wait();
                                Thread.Sleep(new TimeSpan(0, 0, 1));
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }
                    }
                    Global.Log.Information("{lt}: {RedisServiceName} has been ended", "Service", GetType().Name);
                    Thread.Sleep(new TimeSpan(6, 0, 0));
                }
                catch (Exception e)
                {
                    Global.Log.Exception(e, exceptionNote: "error while updating the discord users");
                    Thread.Sleep(new TimeSpan(0, 2, 0));
                }
                finally
                {
                }
            }
        }
    }
}