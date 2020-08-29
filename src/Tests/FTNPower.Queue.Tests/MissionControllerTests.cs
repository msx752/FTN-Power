using Fortnite.Api;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.External.Api;
using Fortnite.Model.Enums;
using Global;
using Global.ConfigModels;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq.Expressions;
using Xunit;

namespace FTNPower.Queue.Tests
{
    public class MissionControllerTests
    {
        private IConfiguration configuration = null;
        private IFortniteQueueApi fortniteQueueApi = null;
        public MissionControllerTests()
        {
            configuration = new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
#if RELEASE
                           .AddJsonFile("appsettings.Production.json", optional: true)
#elif DEBUG
                           .AddJsonFile("appsettings.Development.json", optional: true)
#endif
                           .Build();
            var fortnitequeueapiconfigs = configuration.GetSection("FortniteQueueApiConfigs").Get<FortniteQueueApiConfigs>();
            fortniteQueueApi = new FortniteQueueApi(fortnitequeueapiconfigs.BasicAuthToken, fortnitequeueapiconfigs.Host, fortnitequeueapiconfigs.Port);
        }
        [Fact]
        public void MissionFilter()
        {
            Expression<Func<IMissionX, bool>> exp0 = p =>
                                                       p.HasMythicSurvivor() ||
                                                       p.HasMythicHero() ||
                                                       p.HasVBuck() ||
                                                       p.HasLegendarySurvivor() ||
                                                       p.HasLegendaryDefender() ||
                                                       p.HasLegendaryHero() ||
                                                       p.HasLegendaryShematic() ||
                                                       p.Has4xEyeOfStorm(WorldName.Twine_Peaks) ||
                                                       p.Has4xLightningInABottle(WorldName.Twine_Peaks) ||
                                                       p.Has4xPureDropOfRain(WorldName.Twine_Peaks) ||
                                                       p.Has4xStormShard(WorldName.Twine_Peaks) ||
                                                       p.HasLegendaryAnyTransform();

            var result = fortniteQueueApi.MissionWhere(exp0);
            Assert.NotNull(result);
        }
        [Fact]
        public void Top10()
        {
            var result = fortniteQueueApi.MissionTop10();
            Assert.NotNull(result);
        }
    }
}
