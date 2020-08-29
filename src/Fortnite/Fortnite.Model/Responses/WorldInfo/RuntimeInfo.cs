using System.Collections.Generic;

namespace Fortnite.Model.Responses.WorldInfo
{
    public class RuntimeInfo
    {
        public string theaterType { get; set; }
        public TheaterTags theaterTags { get; set; }
        public TheaterVisibilityRequirements theaterVisibilityRequirements { get; set; }
        public Requirements requirements { get; set; }
        public string requiredSubGameForVisibility { get; set; }
        public bool bOnlyMatchLinkedQuestsToTiles { get; set; }
        public string worldMapPinClass { get; set; }
        public string theaterImage { get; set; }
        public TheaterImages theaterImages { get; set; }
        public TheaterColorInfo theaterColorInfo { get; set; }
        public string socket { get; set; }
        public MissionAlertRequirements missionAlertRequirements { get; set; }
        public List<MissionAlertRequirements> missionAlertCategoryRequirements { get; set; }
    }
}