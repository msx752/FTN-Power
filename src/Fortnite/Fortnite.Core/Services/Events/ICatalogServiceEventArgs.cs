using Fortnite.Model.Enums;
using fortniteLib.Responses.Catalog;

namespace Fortnite.Core.Services.Events
{
    public interface ICatalogServiceEventArgs
    {
        Catalog Catalog { get; set; }
        CatalogType[] Type { get; }
    }
}