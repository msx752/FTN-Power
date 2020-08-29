using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FTNPower.Model.WebsiteModels
{
    public class StwStoreState
    {
        private string channelId;

        public StwStoreState()
        {
            RoleIdToMention = "";
            Active = false;
        }
        public bool Active { get; set; }
        public string ChannelId { get => channelId ?? ""; set => channelId = value; }

        [StringLength(25)]
        public string RoleIdToMention { get; set; }

    }
}
