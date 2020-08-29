using Fortnite.Core.Interfaces;
using Fortnite.Model.Enums;
using Fortnite.Static;
using System;

namespace Fortnite.Static.Models.Survivors
{
    public class SurvivorX : ISurvivorX
    {
        public string AssetId { get; set; }

        public bool IsLeader
        {
            get
            {
                return SlotId == 0;
            }
        }

        private string _name = null;

        public string Name
        {
            get
            {
                return _name;
            }
            internal set
            {
                _name = value;
            }
        }

        private byte _tier = 0;

        public byte Tier
        {
            get { return _tier; }
            set { _tier = value; }
        }

        private byte _level = 0;

        public byte Level
        {
            get { return _level; }
            set { _level = value; }
        }

        private SurvivorRarity _survivorRarity = SurvivorRarity.None;

        public SurvivorRarity Rarity
        {
            get { return _survivorRarity; }
            set { _survivorRarity = value; }
        }

        private byte _squadSlotId = 0;

        public byte SlotId
        {
            get { return _squadSlotId; }
            set { _squadSlotId = value; }
        }

        private string _squadId = null;

        public string SquadId
        {
            get { return _squadId; }
            set { _squadId = value; }
        }

        public string Personality { get; set; }
        public string SetBonus { get; set; }

        public double Power
        {
            get
            {
                return GetSurvivorPower(SlotId, Rarity, Level, Tier, IsLeaderAppropriate);
            }
        }

        public bool IsLeaderAppropriate { get; set; }

        public override string ToString()
        {
            return $"{Name}"/*|{Type}|{Tier}|{SlotType}|{SurvivorType}"*/;
        }

        public static double GetSurvivorPower(byte SlotId, SurvivorRarity rarity, byte level, byte tier, bool IsLeaderAppropriate)
        {
            var constans = SurvivorStaticData.GetLevel_EvoluationConstants(rarity, SlotId == 0);
            var pw = Math.Round((int)rarity + (level - 1) * constans[0] + (tier - 1) * constans[1],
                MidpointRounding.AwayFromZero);
            if (SlotId == 0 && IsLeaderAppropriate)
            {
                pw = pw * 2;
            }
            return pw;
        }
    }
}