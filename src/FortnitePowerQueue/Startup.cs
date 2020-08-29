using Fortnite;
using Fortnite.Api;
using Fortnite.Core.Services;
using Fortnite.External.Api;
using Fortnite.External.Api.Interfaces;
using Fortnite.External.ServiceStore;
using Fortnite.Localization;
using Fortnite.Static.Services;
using FortnitePowerQueue;
using FTNPower.Core;
using FTNPower.Core.ApplicationService;
using FTNPower.Core.DiscordApi;
using FTNPower.Core.DomainService;
using FTNPower.Core.Interfaces;
using FTNPower.Data.Migrations;
using FTNPower.Model.Interfaces;
using FTNPower.Redis;
using FTNPower.Redis.Messaging.AutoRemove;
using FTNPower.Redis.Messaging.AutoUserUpdate;
using FTNPower.Redis.Messaging.ProfileVerifier;
using FTNPower.Static;
using FTNPower.Static.Repositories;
using FTNPower.Static.Services;
using Global;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FTNPower.Queue
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
#if RELEASE
                        .AddJsonFile("appsettings.Production.json", optional: true)
#elif DEBUG
                        .AddJsonFile("appsettings.Development.json", optional: true)
#endif
                        .AddEnvironmentVariables()
                    .Build())
           .AddTransient<BotContext>()
           .AddTransient<IUnitOfWork, UnitOfWork>()
           .AddTransient<IFTNPowerRepository, FTNPowerRepository>()
           .AddSingleton<IEpicApi, EpicApi>()
           .AddSingleton<IExternalApi, ExternalApi>()
           .AddSingleton<IFortniteQueueApi>(x=> new FortniteQueueApi(x.FortniteQueueApiConfigs().BasicAuthToken, x.FortniteQueueApiConfigs().Host, x.FortniteQueueApiConfigs().Port))

          .AddSingleton<IMissionService>(x => new MissionService(x.GetRequiredService<IEpicApi>(), FortniteEventHandler.MissionCallback))
           .AddSingleton<ICatalogService>(x => new CatalogService(x.GetRequiredService<IEpicApi>(), FortniteEventHandler.DailyLlamaCallback))
           .AddSingleton<IEpicFriendListApi, EpicFriendListApi>()
           .AddSingleton<PriorityManager>()
           .AddSingleton<BrStoreService>()
           .AddSingleton<StwStoreService>()
           .AddSingleton<IRedisService, RedisService>()
           // .AddSingleton<UserUpdater>()  /* deprecated due to the performance issues*/
           .AddSingleton<PullMessage>()
           .AddSingleton<PullVerifyRequest>()
           .AddSingleton<IJsonStringLocalizer, JsonStringLocalizer>();

            services.AddSingleton<IDiscordRestApi>(x => new DiscordRestApi(
                x.GetRequiredService<IFTNPowerRepository>().Bot.Config.Developing,
                x.GetRequiredService<IFTNPowerRepository>().Bot.Config.Token));
            services.AddMvc()
                .AddNewtonsoftJson((o) =>
                {
                    o.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                    o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            Global.DIManager.SetManualService(app.ApplicationServices);
            app.UseRouting();
            app.UseEndpoints(route =>
            {
                route.MapDefaultControllerRoute();
            });

            Global.Log.Information("QueueManager is using '{DatabaseType}' SQL DATABASE", FTNPower.Data.Utils.USE_LOCAL_DBCONTEXT ? "LOCAL" : "LIVE");

            Global.Log.Initialize("FTNPowerQueue");

            DIManager.Services.UseFortniteApi().ContinueWith((o) =>
            {
                DIManager.Services.UseFortniteFriendListApi().ContinueWith((o) =>
                {
                    //DIManager.Services.GetRequiredService<BrStoreService>().StartServices(FortniteEventHandler.BrDailyStoreCallback);/*project has been shutdown*/
                    //DIManager.Services.GetRequiredService<StwStoreService>().StartServices(FortniteEventHandler.StwStoreCallback);/*project has been shutdown*/
                    DIManager.Services.GetRequiredService<PriorityManager>().StartPriorityTimer();
                    // DIManager.Services.GetRequiredService<UserUpdater>().Start(); /* deprecated due to the performance issues*/
                    DIManager.Services.GetRequiredService<PullVerifyRequest>().Start();
                    DIManager.Services.GetRequiredService<PullMessage>().Start();
                    Console.WriteLine("\nwelcome to FortnitePower Queue Manager Application\n");
                });
            });

        }
    }
}
