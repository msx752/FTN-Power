using FTNPower.Model.Enums;
using FTNPower.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FTNPower.Model.Tables
{
    [Table("FortnitePVPProfiles")]
    public class FortnitePVPProfile : IFortniteProfile
    {
        public FortnitePVPProfile()
        {

        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        public string EpicId { get; set; }
        [StringLength(50)]
        public string PlayerName { get; set; }
        public int PvpWinSolo { get; set; }
        public int PvpWinDuo { get; set; }
        public int PvpWinSquad { get; set; }
        public int PvpCurrentModeWins(GameUserMode mode)
        {
            if (mode == GameUserMode.PVP_WIN_ALL)
            {
                return PvpWinSolo + PvpWinSquad + PvpWinDuo;
            }
            else if (mode == GameUserMode.PVP_WIN_SOLO)
            {
                return PvpWinSolo;
            }
            else if (mode == GameUserMode.PVP_WIN_DUO)
            {
                return PvpWinDuo;
            }
            else if (mode == GameUserMode.PVP_WIN_SQUAD)
            {
                return PvpWinSquad;
            }
            else
            {
                return 0;
            }
        }
    }
}

