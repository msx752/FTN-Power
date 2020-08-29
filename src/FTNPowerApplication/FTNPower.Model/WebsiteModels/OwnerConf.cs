using Fortnite.Localization;
using FTNPower.Model.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FTNPower.Model.WebsiteModels
{
    public class OwnerConf
    {
        public OwnerConf()
        {
            AutoRemoveRequest = false;
            DefaultGameMode = GameUserMode.PVE;
            PVEDecimalState = true;
            RestrictedRoleIds = new List<string>();
            DefaultLanguage = GuildLanguage.EN;
        }
        [DefaultValue(0)]
        public GuildLanguage DefaultLanguage { get; set; }
        public bool AutoRemoveRequest { get; set; }
        [DefaultValue(GameUserMode.PVE)]
        public GameUserMode DefaultGameMode { get; set; }
        [DefaultValue(true)]
        public bool PVEDecimalState { get; set; }
        [MaxLength(5)]
        public List<string> RestrictedRoleIds { get; set; }
    }
}
