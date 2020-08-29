using Fortnite.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Fortnite.Static.Models.Survivors
{
    public class SurvivorSquad : ISurvivorSquad
    {
        public ISetBonusCalculator SetBonuses { get; internal set; }
        public string SquadName { get; internal set; }

        public ISurvivorX Leader
        {
            get
            {
                return Survivors.FirstOrDefault(p => p.SlotId == 0);
            }
        }

        public IEnumerable<ISurvivorX> Survivors { get; internal set; }
        public List<double> TeamBonuses { get; internal set; }

        public double SquadBonus
        {
            get
            {
                var s_bonunses = 0;// SetBonuses.CalculateBonuses();
                var t_bonuses = TeamBonuses.Sum();
                var su_bonuses = Survivors.Sum(f => f.Power);
                var sq_bonuses = s_bonunses + t_bonuses + su_bonuses;
                if (Leader != null && t_bonuses == 0)
                {
                    int personality = Survivors.Count(f => !f.IsLeader && f.Personality == Leader.Personality);
                    sq_bonuses += 5 * personality;
                }
                return sq_bonuses;
            }
        }
    }
}