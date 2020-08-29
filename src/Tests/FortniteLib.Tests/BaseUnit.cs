
using Discord;
using Fortnite.Api;
using Fortnite.Localization;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Static;
using FTNPower.Core.ApplicationService;
using FTNPower.Core.DomainService;
using FTNPower.Data.Migrations;
using FTNPower.Static.Repositories;
using FTNPower.Static.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Fortnite.Tests
{
    public class BaseUnit
    {
        public IJsonStringLocalizer JsonStringLocalizer
        {
            get
            {
                return Global.DIManager.Services.GetRequiredService<IJsonStringLocalizer>();
            }
        }
        public IFTNPowerRepository Repo
        {
            get
            {
                return Global.DIManager.Services.GetRequiredService<IFTNPowerRepository>();
            }
        }
        public BaseUnit()
        {
            var cultureInfo = new CultureInfo("en-GB");
            cultureInfo.NumberFormat.CurrencySymbol = "£";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            SurvivorStaticData.LoadStatics();
            MissionsStaticData.LoadStatics();
            Api = new EpicApi();
            Api.StartVerifier().Wait();
            FriendsApi = new EpicFriendListApi();
            FriendsApi.StartVerifier().Wait();
            Global.DIManager.BuildService(Service =>
            {
                Service.AddTransient<BotContext>();
                Service.AddSingleton<IJsonStringLocalizer, JsonStringLocalizer>();
                Service.AddTransient<IUnitOfWork, UnitOfWork>();
                Service.AddTransient<IFTNPowerRepository, FTNPowerRepository>();
                Service.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
#if RELEASE
                        .AddJsonFile("appsettings.Production.json", optional: true)
#elif DEBUG
                        .AddJsonFile("appsettings.Development.json", optional: true)
#endif
                        .AddEnvironmentVariables()
                        .Build());
            });
        }

        public EpicFriendListApi FriendsApi { get; set; }
        public EpicApi Api { get; set; }

        public async Task<KeyValuePair<string, IQueryProfile>> UserPVEQueryProfile(string username)
        {
            var profile = await Api.GetPVEProfileByName(username);
            return profile;
        }

        public async Task<KeyValuePair<string, fortniteLib.Responses.Pvp.BattleRoyaleStats>> UserPVPQueryProfile(string username)
        {
            var profile = await Api.GetPVPProfileByName(username);
            return profile;
        }
    }
}