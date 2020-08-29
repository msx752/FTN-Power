using System.Collections.Generic;

namespace Fortnite.Core.Interfaces
{
    public interface ISurvivorSquad
    {
        ISurvivorX Leader { get; }
        ISetBonusCalculator SetBonuses { get; }
        double SquadBonus { get; }
        string SquadName { get; }
        IEnumerable<ISurvivorX> Survivors { get; }
        List<double> TeamBonuses { get; }
    }
}