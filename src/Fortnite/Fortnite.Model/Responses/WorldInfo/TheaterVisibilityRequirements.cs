namespace Fortnite.Model.Responses.WorldInfo
{
    public class TheaterVisibilityRequirements
    {
        public int commanderLevel { get; set; }
        public int personalPowerRating { get; set; }
        public int maxPersonalPowerRating { get; set; }
        public int partyPowerRating { get; set; }
        public int maxPartyPowerRating { get; set; }
        public string activeQuestDefinition { get; set; }
        public string questDefinition { get; set; }
        public ObjectiveStatHandle objectiveStatHandle { get; set; }
        public string uncompletedQuestDefinition { get; set; }
        public string itemDefinition { get; set; }
        public string eventFlag { get; set; }
    }
}