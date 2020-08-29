using FTNPower.Model.Enums;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FTNPower.Model.Tables
{
    public class PriorityTable
    {
        public PriorityTable()
        {
            Id = "n000000000000000000";
            Deadline = DateTimeOffset.UtcNow.AddSeconds(-1);
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(21)]
        public string Id { get; set; }

        public DateTimeOffset Deadline { get; set; }

        public bool AdvertOn { get; set; }
        public string AdvertCustomText { get; set; }

        public bool Notified { get; set; }

        [StringLength(50)]
        public string PromoteCreatorCode { get; set; }

        [JsonIgnore]
        [NotMapped]
        public TimeSpan Remining
        {
            get
            {
                return Deadline - DateTimeOffset.UtcNow;
            }
        }
        [JsonIgnore]
        [NotMapped]
        public string ExpiresIn
        {

            get
            {
                if (Remining.TotalMilliseconds < 0)
                {
                    return $"**Expired** at {Deadline.ToString()}";
                }
                else
                {
                    DateTime dtx = new DateTime();
                    dtx = dtx.Add(Remining);
                    string expr = $"Expires In: **{dtx.Year - 1}**Years **{dtx.Month - 1}**Months **{dtx.Day - 1}**days **{dtx.Hour}**hours";
                    return expr;
                }
            }
        }
        [JsonIgnore]
        [NotMapped]
        public PriorityState State
        {
            get
            {
                if (Id.StartsWith("u", StringComparison.InvariantCultureIgnoreCase))
                {
                    return PriorityState.User;
                }
                else if (Id.StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                {
                    return PriorityState.Guild;
                }
                else
                {
                    return PriorityState.Normal;
                }
            }
        }
    }
}