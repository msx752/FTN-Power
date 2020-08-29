using Fortnite.Model.Enums;
using FTNPower.Model;
using FTNPower.Model.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FTNPower.Model.Tables
{
    [Table("FortnitePVEProfiles")]
    public class FortnitePVEProfile : IFortniteProfile
    {
        private double _accountPowerLevel;
        public FortnitePVEProfile()
        {
            EliteFortnite2019 = false;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        public string EpicId { get; set; }
        [StringLength(50)]
        public string PlayerName { get; set; }
        public double AccountPowerLevel
        {
            get { return Math.Round(_accountPowerLevel, 2); }
            set { _accountPowerLevel = value; }
        }
        public MapRoles Map { get; set; }
        public int CommanderLevel { get; set; }
        public int CollectionBookLevel { get; set; }

        public int NumMythicSchematics { get; set; }

        public bool EliteFortnite2019 { get; set; }
        public override string ToString()
        {
            return PlayerName ?? "nulL";
        }
        [NotMapped]
        [JsonIgnore]
        public int GetIntegerPower
        {
            get
            {
                return Utils.GetIntegerPower(AccountPowerLevel);
            }
        }
    }
}
