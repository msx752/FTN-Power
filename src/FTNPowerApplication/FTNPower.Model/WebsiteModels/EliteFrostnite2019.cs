using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FTNPower.Model.WebsiteModels
{
    public class EliteFrostnite2019
    {
        public EliteFrostnite2019()
        {
            RoleId = "";
            Active = false;
        }
        public bool Active { get; set; }

        [StringLength(25)]
        public string RoleId { get; set; }
    }
}
