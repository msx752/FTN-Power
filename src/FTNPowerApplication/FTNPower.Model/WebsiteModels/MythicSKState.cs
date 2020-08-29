using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FTNPower.Model.WebsiteModels
{
    public class MythicSKState
    {
        public MythicSKState()
        {
            RoleIdToMythicSK = "";
            Active = false;
        }
        public bool Active { get; set; }

        [StringLength(25)]
        public string RoleIdToMythicSK { get; set; }
    }
}
