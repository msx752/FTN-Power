namespace Fortnite.Model.Responses.QueryProfile
{
    public class ItemAttribute
    {
        public bool _private { get; set; }
        public int building_slot_used { get; set; }
        public string devName { get; set; }
        public string event_instance_id { get; set; }
        public object event_purchases { get; set; }
        public bool favorite { get; set; }
        public bool item_seen { get; set; }
        public int level { get; set; }
        public int max_level_bonus { get; set; }
        public string platform { get; set; }
        public string squad_id { get; set; } = "";
        public int squad_slot_idx { get; set; }
        public int xp { get; set; }
        public string personality { get; set; }
        public string set_bonus { get; set; }
        public int completion_destroy_gnome { get; set; }
        public int completion_build_any_structure { get; set; }
        public int completion_questcollect_survivoritemdata { get; set; }
        public int completion_interact_treasurechest { get; set; }
        public int completion_kill_husk_smasher { get; set; }
        public int completion_quick_complete { get; set; }
        public int completion_complete_exploration_1 { get; set; }
        public int completion_s11_holdfastquest_seasonelite_knight { get; set; }
    }
}