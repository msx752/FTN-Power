using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FTNPower.Model.Tables
{
    public class PaypalTxn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(30)]
        public string TxnId { get; set; }

        public string DiscordUserId { get; set; }
    }
}
