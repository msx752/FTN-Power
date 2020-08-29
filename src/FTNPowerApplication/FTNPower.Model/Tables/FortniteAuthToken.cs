using Fortnite.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FTNPower.Model.Tables
{
    [Table("FortniteAuthTokens")]
    public class FortniteAuthToken : AuthToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(35)]
        private string _id = "";
        public string Id { get { return _id; } set { _id = value.ToLowerInvariant(); } }//email address
    }
}
