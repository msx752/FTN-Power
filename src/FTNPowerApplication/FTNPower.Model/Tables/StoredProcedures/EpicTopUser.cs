using FTNPower.Model;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables.StoredProcedures
{
    public class EpicTopUser
    {
        public double AccountPowerLevel { get; set; }
        public string PlayerName { get; set; }
        public string Id { get; set; }
        public string EpicId { get; set; }
        public int CommanderLevel { get; set; }
        public int CollectionBookLevel { get; set; }

        [NotMapped]
        [JsonIgnore]
        public ulong Uid { get { return Id.ToUlong(); } }
    }
}