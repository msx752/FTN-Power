namespace FTNPower.Model.WebsiteModels
{
    public class EventConf
    {
        public EventConf()
        {
            MythicSKStates = new MythicSKState();
        }
        public MythicSKState MythicSKStates { get; set; }


        private EliteFrostnite2019 @eliteFrostnite2019s = null;
        public EliteFrostnite2019 EliteFrostnite2019s
        {
            get
            {
                if (@eliteFrostnite2019s == null)
                    @eliteFrostnite2019s = new EliteFrostnite2019();
                return @eliteFrostnite2019s;
            }
            set
            {
                @eliteFrostnite2019s = value;
            }
        }
    }
}
