using System;
using System.Collections.Generic;

namespace Fortnite.Model.Responses.QueryProfile
{
    public interface IMissionCategory
    {
        List<string> missionAlertGuids { get; set; }
        List<DateTime> lastClaimedTimes { get; set; }
        int nextClaimIndex { get; set; }
    }

    public class MissionAlertRedemptionRecord
    {
        public ClaimDataMap claimDataMap { get; set; }
    }

    public class NewMissionIntroductionCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class HalloweenCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class GeneralCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class Survival3DayCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class GroupMissionCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class ElemementalZoneNatureCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class ElemementalZoneWaterCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class StormLowCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class HordeCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class MegaAlertCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class MiniBossCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class Survival7DayCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class ElemementalZoneFireCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class StormCategory : IMissionCategory
    {
        public List<string> missionAlertGuids { get; set; }
        public List<DateTime> lastClaimedTimes { get; set; }
        public int nextClaimIndex { get; set; }
    }

    public class ClaimDataMap
    {
        public NewMissionIntroductionCategory NewMissionIntroductionCategory { get; set; }
        public HalloweenCategory HalloweenCategory { get; set; }
        public GeneralCategory GeneralCategory { get; set; }
        public Survival3DayCategory Survival3DayCategory { get; set; }
        public GroupMissionCategory GroupMissionCategory { get; set; }
        public ElemementalZoneNatureCategory ElemementalZoneNatureCategory { get; set; }
        public ElemementalZoneWaterCategory ElemementalZoneWaterCategory { get; set; }
        public StormLowCategory StormLowCategory { get; set; }
        public HordeCategory HordeCategory { get; set; }
        public MegaAlertCategory MegaAlertCategory { get; set; }
        public MiniBossCategory MiniBossCategory { get; set; }
        public Survival7DayCategory Survival7DayCategory { get; set; }
        public ElemementalZoneFireCategory ElemementalZoneFireCategory { get; set; }
        public StormCategory StormCategory { get; set; }
    }
}