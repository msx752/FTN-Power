using System.Collections.Generic;

namespace fortniteLib.Responses.Catalog
{
    public class GiftInfo
    {
        public bool bIsEnabled { get; set; }
        public string forcedGiftBoxTemplateId { get; set; }
        public List<object> purchaseRequirements { get; set; }
        public List<object> giftRecordIds { get; set; }
    }
}