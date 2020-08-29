using System.ComponentModel;

namespace Fortnite.Model.Enums
{
    public enum Alert : int
    {
        [Description("General")]
        GeneralAlert = 0,
        [Description("Nature")]
        NatureCategory = 1,
        [Description("Water")]
        WaterCategory = 2,
        [Description("StormLow")]
        StormLowCategory = 3,
        [Description("Mega")]
        MegaAlertCategory = 4,
        [Description("Miniboss")]
        MiniBossCategory = 5,
        [Description("FireZone")]
        ElemementalZoneFireCategory = 6,
        [Description("Storm")]
        StormCategory = 7,
        [Description("Dudebro")]
        DudebroAlert = 8,
        [Description("Introduction")]
        NewMissionIntroductionCategory = 9,
        [Description("WaterZone")]
        ElemementalZoneWaterCategory = 10,
        [Description("Nature")]
        ElemementalZoneNatureCategory = 11,
        [Description("FirePassive")]
        Storm_FirePassive = 12,
        [Description("IcePassive")]
        Storm_IcePassive = 13,
        [Description("LightningActive")]
        Storm_LightningActive = 14,
        [Description("StormMiniboss")]
        Storm_MinibossPassive = 15,
        [Description("StormMutant")]
        Storm_MutantStonewoodActive = 16,
        [Description("MegaAlertLightning")]
        MegaAlertCategory_Lightning = 17,
        [Description("FireActive")]
        Storm_FireActive = 18,
        [Description("IceActive")]
        Storm_IceActive = 19,
        [Description("LightningPassive")]
        Storm_LightningPassive = 20,
        [Description("MegaAlertFire")]
        MegaAlertCategory_Fire=21
    }
}