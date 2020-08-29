using Fortnite.Model.Responses.Catalog;
using System.ComponentModel.DataAnnotations;

namespace FTNPower.Image.Processing.Models
{
    public class RawApiRequestItem : CatalogDataTransferFormat
    {
        [Required]
        public new string templateId { get; set; }

        public override string ToString()
        {
            return templateId;
        }
    }
}