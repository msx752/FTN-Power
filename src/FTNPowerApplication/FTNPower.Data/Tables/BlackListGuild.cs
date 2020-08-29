using FTNPower.Model.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Data.Tables
{
    public class BlackListGuild : IBlackList
    {
        public BlackListGuild()
        {
            Until = DateTimeOffset.UtcNow.AddYears(100);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        public string Id { get; set; }

        public DateTimeOffset Until { get; set; }
    }
}