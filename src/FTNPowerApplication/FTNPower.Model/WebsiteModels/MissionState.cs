using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FTNPower.Model.WebsiteModels
{
    public class ChannelItem
    {
        private string roleId = "";
        private string channelId = "";

        public MissionType MissionType { get; set; }
        public string ChannelId { get => channelId ?? ""; set => channelId = value ?? ""; }
        public string RoleId { get => roleId ?? ""; set => roleId = value ?? ""; }
        public override string ToString()
        {
            return ((int)MissionType).ToString();
        }
    }
    public class MissionState
    {
        public MissionState()
        {
            Channels = new List<ChannelItem>();
            Active = false;
        }
        public bool Active { get; set; }
        /// <summary>
        /// MissionType, channelId, RoleIdToMention
        /// </summary>

        [MaxLength(25)]
        public List<ChannelItem> Channels { get; set; }
        public ChannelItem Get(MissionType type)
        {
            var dat = Channels.FirstOrDefault(f => f.MissionType == type);
            if (dat == null)
            {
                dat = new ChannelItem();
                dat.ChannelId = "";
                dat.RoleId = "";
            }
            return dat;
        }
        public bool Contains(MissionType type)
        {
            return Channels.Any(f => f.MissionType == type);
        }
    }
}
