using Fortnite.Core.Interfaces;
using System.Collections.Generic;

namespace Fortnite.Static.Models.Survivors
{
    public class SetBonusCalculator : ISetBonusCalculator
    {
        private static readonly Dictionary<string, int> SetBonusAmount = new Dictionary<string, int>()
        {
            {"Homebase.Worker.SetBonus.IsFortitudeLow", 2},
            {"Homebase.Worker.SetBonus.IsResistanceLow", 2},
            {"Homebase.Worker.SetBonus.IsShieldRegenLow", 2},
            {"Homebase.Worker.SetBonus.IsRangedDamageLow", 3},
            {"Homebase.Worker.SetBonus.IsMeleeDamageLow", 3},
            {"Homebase.Worker.SetBonus.IsAbilityDamageLow", 3},
            {"Homebase.Worker.SetBonus.IsTrapDamageLow", 3},
            {"Homebase.Worker.SetBonus.IsTrapDurabilityHigh", 2},
        };

        private readonly Dictionary<string, int> SetBonusCounter = new Dictionary<string, int>();

        public void Increase(string bonus)
        {
            if (bonus == null)
            {
                return;
            }

            if (SetBonusCounter.ContainsKey(bonus))
            {
                SetBonusCounter[bonus] += 1;
            }
            else
            {
                SetBonusCounter.Add(bonus, 1);
            }
        }
    }
}