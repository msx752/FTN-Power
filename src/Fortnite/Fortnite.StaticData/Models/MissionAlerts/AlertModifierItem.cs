using Fortnite.Core.Interfaces;
using Fortnite.Model.Enums;
using Fortnite.Static;

namespace Fortnite.Static.Models.MissionAlerts
{
    public class AlertModifierItem : IAlertModifierItem
    {
        public AlertModifierItem()
        {
            AssetId = "?";
        }

        private string itemType = null;

        public string ItemType
        {
            get
            {
                return itemType;
            }
            set
            {
                itemType = value;
                this.DefineAsset();
            }
        }

        public string AssetId { get; set; }
        public ItemClass Class { get; set; }
        public bool? IsPositiveMutation { get; set; }
    }
}