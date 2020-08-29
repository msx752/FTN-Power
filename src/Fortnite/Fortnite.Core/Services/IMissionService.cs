using Fortnite.Core.Interfaces;
using Fortnite.Core.Services.Events;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.WorldInfo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fortnite.Core.Services
{

    public interface IMissionService
    {
        bool IsWorldReady { get; set; }
        IEnumerable<IMissionX> MissionsList { get; set; }
        IEnumerable<IMissionX> Top10MissionsList { get; set; }
        DateTimeOffset UpdateTime { get; }
        DateTimeOffset UpdateTime2 { get; }
        Func<IMissionServiceEventArgs, Task> WebhookCallback { get; }

        IEnumerable<IMissionX> Get4xEyeOfStorm(WorldName world = WorldName.Twine_Peaks);
        IEnumerable<IMissionX> Get4xLightningInABottle(WorldName world = WorldName.Twine_Peaks);
        IEnumerable<IMissionX> Get4xPureDrop(WorldName world = WorldName.Twine_Peaks);
        IEnumerable<IMissionX> Get4xStormShard(WorldName world = WorldName.Twine_Peaks);
        IEnumerable<IMissionX> GetEpicMaps();
        IEnumerable<IMissionX> GetEpicPerk(WorldName world);
        IEnumerable<IMissionX> GetLegendaryMaps(WorldName world);
        IEnumerable<IMissionX> GetLegendaryPerk(WorldName world);
        IEnumerable<IMissionX> GetLegendarySurvivors();
        AllMissions GetMissions(WorldInfo world, IEnumerable<World> worlds);
        IEnumerable<IMissionX> GetMythicMaps(WorldName world);
        IEnumerable<IMissionX> GetMythicSurvivors();
        IEnumerable<IMissionX> GetVbuckMaps();
        Task<IEnumerable<IMissionX>> GetWebhookMissions(bool getNewest = true);
        IEnumerable<World> GetWorlds(List<Theater> theaters);
        bool LoadWorldInfo(bool forceToLoad = false);
        IEnumerable<IMissionX> MissionMaping(List<World> worlds, AllMissions allMissions);
        void StartWebhookTimer();
        string ToJson();
        IEnumerable<IMissionX> TopMissions();
    }
}
