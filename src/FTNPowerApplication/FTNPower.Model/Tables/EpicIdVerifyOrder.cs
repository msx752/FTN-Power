using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FTNPower.Model.Tables
{
    /// <summary>
    /// prevent duplicate verification at a specific moment
    /// </summary>
    [Table("VerifyOrders")]
    public class EpicIdVerifyOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(50)]
        public string Id { get; set; }
    }
}
