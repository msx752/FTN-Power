using Fortnite.Model.Enums;

namespace Fortnite.Core.Interfaces
{
    public interface IAlertModifierItem
    {
        string AssetId { get; set; }
        ItemClass Class { get; set; }
        bool? IsPositiveMutation { get; set; }
        string ItemType { get; set; }
    }
}