using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FTNPower.Image.Processing.Models
{
    public class StwStoreModel
    {
        [Required]
        public List<RawApiRequestItem> STWSpecialEventStorefront { get; set; }
        [Required]
        public List<RawApiRequestItem> STWRotationalEventStorefront { get; set; }
    }
}
