namespace FTNPower.Model.WebsiteModels
{
    public class AdminConf
    {
        public AdminConf()
        {
            MissionStates = new MissionState();
            LlamaSates = new LlamaState();
            BrStoreStates = new BrStoreState();
            StwStoreStates = new StwStoreState();
        }
        public MissionState MissionStates { get; set; }
        public LlamaState LlamaSates { get; set; }
        public BrStoreState BrStoreStates { get; set; }
        public StwStoreState StwStoreStates { get; set; }
    }
}
