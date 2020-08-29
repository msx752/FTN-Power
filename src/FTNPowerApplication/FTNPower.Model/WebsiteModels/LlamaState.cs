using System.ComponentModel.DataAnnotations;

namespace FTNPower.Model.WebsiteModels
{
    public class LlamaState
    {
        private string channelId;

        public LlamaState()
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
