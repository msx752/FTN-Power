using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Fortnite.Localization;
using FTNPower.Core.DomainService;
using FTNPower.Data;
using FTNPower.Model.Enums;
using FTNPower.Model.Interfaces;
using FTNPower.Model.Tables;
using Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FTNPower.Queue
{
    public class PriorityManager
    {
        private Thread th;

        public PriorityManager(IDiscordRestApi discordRestApi, IFTNPowerRepository fTNPowerRepository, IJsonStringLocalizer translation)
        {
            DiscordRestApi = discordRestApi;
            Translation = translation;
            Repo = fTNPowerRepository;
        }

        public IDiscordRestApi DiscordRestApi { get; }

        public IFTNPowerRepository Repo { get; }
        public IJsonStringLocalizer Translation { get; }

        public void StartPriorityTimer()
        {
            th?.Abort();
            th = new Thread(OnTimedEvent);
            th.Priority = ThreadPriority.BelowNormal;
#if RELEASE
            th.Start();
            Global.Log.Information("{lt}: PriorityValidation is Started.","Service");
#endif
        }

        private async void OnTimedEvent()
        {
            List<PriorityTable> plist = new List<PriorityTable>();
            while (true)
            {
                try
                {
                    plist.Clear();
                    plist.AddRange(Repo.Priority.Priorities.Where(f => !f.Notified).ToList());
                    for (var index = 0; index < plist.Count; index++)
                    {
                        var priority = plist[index];
                        if (priority.Remining.TotalDays >= 0 && priority.Remining.TotalDays <= 2.1)
                        {
                            try
                            {
                                RestUser usr = null;
                                string namefor = "";
                                string gid = "";
                                if (priority.State == PriorityState.User)
                                {

                                    usr = DiscordRestApi.GetApi.GetUserAsync(priority.GetUlongId()).Result;
                                    if (usr == null)
                                        continue;
                                    namefor = $"{usr.Username}#{usr.Discriminator}";
                                    gid = $" 'please specify your UserId(**{priority.GetUlongId()}**) on description'";
                                }
                                else if (priority.State == PriorityState.Guild)
                                {
                                    var guild = DiscordRestApi.GetApi.GetGuildAsync(priority.GetUlongId()).Result;
                                    if (guild == null)
                                        continue;
                                    usr = DiscordRestApi.GetApi.GetUserAsync(guild.OwnerId).Result;
                                    if (usr == null)
                                        continue;
                                    namefor = $"{guild.Name}";
                                    gid = $" 'please specify your DiscordServerId(**{priority.GetUlongId()}**) on description'";
                                }
                                else
                                {
                                    throw new Exception("undefined PriorityState for PriorityManager");
                                }

                                DateTime dtx = new DateTime();
                                dtx = dtx.Add(priority.Remining);
                                EmbedBuilder embed = new EmbedBuilder()
                                {
                                    Author = new EmbedAuthorBuilder()
                                    {
                                        IconUrl = DiscordRestApi.GetApi.CurrentUser.GetAvatarUrl(),
                                        Name = DiscordRestApi.GetApi.CurrentUser.Username
                                    },
                                    Color = Color.Green,
                                    Title = $"REMINDER FOR FTN POWER DISCORD PLAN",
                                    Description =
                                        $"\n Priority Feature plan for '**{namefor}**'[**{priority.State}**] \n__expires in__ **{dtx.Month - 1}**Months **{dtx.Day - 1}**days **{dtx.Hour}**hours\n\nif you like, you can get this discord-plan again (1 month for all discord members). [Plans](https://ftnpower.com/home/plans){gid}\nthank you for using **FTN Power** :heart:\n\n",
                                    Footer = new EmbedFooterBuilder()
                                    {
                                    }
                                };
                                priority.Notified = true;
                                if (priority.State == PriorityState.Guild)
                                {
                                    Repo.ResetFeatures(priority.GetUlongId().ToString());
                                }
                                var dm = await usr.GetOrCreateDMChannelAsync();
                                var msg = await dm.SendMessageAsync(string.Empty, false, embed.Build(), Core.Utils.RequestOption);
                                await dm.CloseAsync(Core.Utils.RequestOption);
                                Log.Information("{lt}: customer {CustomerState} name with '{CustomerName}', it's id {PriorityId}", "Service", priority.State.ToString(), namefor, priority.Id);

                                var usr2 = DiscordRestApi.GetApi.GetUserAsync(193749607107395585).Result;
                                var dm2 = await usr2.GetOrCreateDMChannelAsync();
                                var msg2 = await dm2.SendMessageAsync(string.Empty, false, embed.Build(), Core.Utils.RequestOption);
                                await dm2.CloseAsync(Core.Utils.RequestOption);

                                Thread.Sleep(10);
                            }
                            catch (Exception e)
                            {
                                Log.Write(Serilog.Events.LogEventLevel.Warning, "{lt}: Owner couldn't informed about subscription expiration priority is {PriorityId}", "Service", priority.Id);
                                var usr2 = DiscordRestApi.GetApi.GetUserAsync(193749607107395585).Result;
                                var dm2 = await usr2.GetOrCreateDMChannelAsync();
                                EmbedBuilder embed = new EmbedBuilder()
                                {
                                    Title = $"ERROR WHILE CHECKING FTN POWER DISCORD PLAN",
                                    Description = $"ERROR_WHILE_CHECKING_PRIORITY_ID_{priority.Id}_[{e.Message}],**so they couldn't informed**",
                                    Color = Color.DarkRed
                                };
                                var msg2 = await dm2.SendMessageAsync(string.Empty, false, embed.Build(), Core.Utils.RequestOption);
                                await dm2.CloseAsync(Core.Utils.RequestOption);
                                continue;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Exception(e, Serilog.Events.LogEventLevel.Fatal);
                }
                finally
                {
                    Thread.Sleep(new TimeSpan(0, 5, 0));
                }
            }
        }
    }
}
