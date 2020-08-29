using Fortnite.Model.Enums;
using fortniteLib.Responses.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fortnite.Core.Services.Events
{
    public class CatalogServiceEventArgs : ICatalogServiceEventArgs
    {
        public Catalog Catalog { set; get; }
        public CatalogType[] Type { get; }

        public CatalogServiceEventArgs(Catalog catalog, params CatalogType[] types)
        {
            Type = types;
            Catalog = catalog;
        }
    }
}
