using Fortnite.Api;
using Fortnite.Core.Interfaces;
using Fortnite.Core.Services;
using Fortnite.Core.Services.Events;
using Fortnite.Static;
using Fortnite.Static.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Fortnite
{
    public static class Init
    {
        public static IApplicationBuilder UseFortniteApi(this IApplicationBuilder applicationBuilder)
        {
            UseFortniteApi(applicationBuilder.ApplicationServices);
            return applicationBuilder;
        }
        public static Task UseFortniteApi(this IServiceProvider serviceProvider)
        {
            return Task.Run(() =>
            {
                var epicApi = serviceProvider.GetRequiredService<IEpicApi>();
                return epicApi.StartVerifier()
                     .ContinueWith((o) =>
                     {
                         if (o.IsFaulted)
                             throw o.Exception;
                         MissionsStaticData.LoadStatics();
                         SurvivorStaticData.LoadStatics();
                         var missionService = serviceProvider.GetService<IMissionService>();
                         missionService?.StartWebhookTimer();
                         var catalogService = serviceProvider.GetService<ICatalogService>();
                         catalogService?.StartCatalogTimer();
                     });
            });
        }
        public static IApplicationBuilder UseFortniteFriendListApi(this IApplicationBuilder applicationBuilder)
        {
            UseFortniteFriendListApi(applicationBuilder.ApplicationServices);
            return applicationBuilder;
        }
        public static Task UseFortniteFriendListApi(this IServiceProvider serviceProvider)
        {
            return Task.Run(() =>
           {
               var epicApi = serviceProvider.GetRequiredService<IEpicFriendListApi>();
               return epicApi.StartVerifier();
           });
        }
    }
}