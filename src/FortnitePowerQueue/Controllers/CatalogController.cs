using Fortnite.Core;
using Fortnite.Core.ModifiedModels;
using Fortnite.Core.Services;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.Catalog;
using fortniteLib.Responses.Catalog;
using FTNPower.Queue.Helpers;
using Microsoft.AspNetCore.Mvc;
using Serialize.Linq.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTNPower.Queue.Controllers
{
    [BasicAuth]
    public class CatalogController : Controller
    {
        public readonly ICatalogService CatalogService;
        public CatalogController(ICatalogService catalogService)
        {
            CatalogService = catalogService;
        }
        [HttpGet]
        public string Index()
        {
            return "";
        }
        [HttpGet]
        [Produces("application/json")]
        public IEnumerable<DailyLlama> DailyLlama()
        {
            if (!CatalogService.IsCatalogReady)
                return new List<DailyLlama>();
            var dllama = Utils.GetDailyLlamas(CatalogService.Catalog);
            return dllama.AsEnumerable();
        }
        [HttpGet]
        [Produces("application/json")]
        public Dictionary<CatalogType, CatalogDataTransferFormat[]> STWStoreSimplified()
        {
            if (!CatalogService.IsCatalogReady)
                return new Dictionary<CatalogType, CatalogDataTransferFormat[]>();
            var dllama = Catalog.GetSTWStoreSimplified(CatalogService.Catalog);
            return dllama;
        }
    }
}
