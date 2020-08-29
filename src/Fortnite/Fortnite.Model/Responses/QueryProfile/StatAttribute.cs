using System;

namespace Fortnite.Model.Responses.QueryProfile
{
    public class StatAttribute
    {
        public bool allowed_to_receive_gifts { get; set; }
        public bool allowed_to_send_gifts { get; set; }
        public object ban_history { get; set; }
        public string current_mtx_platform { get; set; }
        public object daily_purchases { get; set; }
        public object gift_history { get; set; }
        public object import_friends_claimed { get; set; }
        public object in_app_purchases { get; set; }
        public int inventory_limit_bonus { get; set; }
        public bool mfa_enabled { get; set; }
        public object monthly_purchases { get; set; }
        public string mtx_affiliate { get; set; }
        public int mtx_grace_balance { get; set; }
        public object mtx_purchase_history { get; set; }
        public object weekly_purchases { get; set; }
        public MissionAlertRedemptionRecord mission_alert_redemption_record { get; set; }
        public ResearchLevel research_levels { get; set; }
        public int level { get; set; }
        public DailyRewards daily_rewards { get; set; }
        public GamePlayStat[] gameplay_stats { get; set; }
        public CollectionBook collection_book { get; set; }
        public int? rewards_claimed_post_max_level { get; set; }
    }

    public class CollectionBook
    {
        public string[] pages { get; set; }
        public int maxBookXpLevelAchieved { get; set; }
    }

    public class GamePlayStat
    {
        public GamePlayStat()
        {
        }

        public string statName { get; set; }
        public int statValue { get; set; }
    }

    public class DailyRewards
    {
        public int nextDefaultReward { get; set; }
        public int totalDaysLoggedIn { get; set; }
        public DateTimeOffset lastClaimDate { get; set; }
    }
}