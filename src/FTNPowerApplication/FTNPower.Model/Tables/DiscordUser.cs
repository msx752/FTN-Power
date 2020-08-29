using FTNPower.Model;
using FTNPower.Model.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables
{
    [Table("FortniteUsers")]
    public class FortniteUser
    {

        public FortniteUser()
        {
            NameStates = new List<NameState>();
            GameUserMode = GameUserMode.NULL;
            LastUpDateTime = DateTimeOffset.UtcNow.AddDays(-1);
            VerifiedProfile = false;
            NameTag = false;
            IsValidName = false;
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(20)]
        public string Id { get; set; }

        public bool IsValidName { get; set; }

        public DateTimeOffset LastUpDateTime { get; set; }

        public ICollection<NameState> NameStates { get; set; }


        [StringLength(50)]
        public string EpicId { get; set; }
        public GameUserMode GameUserMode { get; set; }


        [NotMapped]
        [JsonIgnore]
        public ulong Uid { get { return Id.ToUlong(); } }

        /// <summary>
        /// default (left)(false)
        /// </summary>
        public bool NameTag { get; set; }

        public bool VerifiedProfile { get; set; }



        public override string ToString()
        {
            return Id ?? "nulL";
        }

    }
}