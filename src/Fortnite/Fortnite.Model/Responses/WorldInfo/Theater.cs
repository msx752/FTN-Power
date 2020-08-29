using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class Theater
    {
        public WWorldName displayName { get; set; }
        public string uniqueId { get; set; }
        public int theaterSlot { get; set; }
        public bool bIsTestTheater { get; set; }
        public bool bHideLikeTestTheater { get; set; }
        public string requiredEventFlag { get; set; }
        public string missionRewardNamedWeightsRowName { get; set; }
        public WWorldDescription description { get; set; }
        public RuntimeInfo runtimeInfo { get; set; }
        public List<Tile> tiles { get; set; }
        public List<Region> regions { get; set; }
    }
}