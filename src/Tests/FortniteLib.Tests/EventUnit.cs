using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Core.Services;
using fortniteLib.Responses.Catalog;
using Newtonsoft.Json;
using Xunit;

namespace Fortnite.Tests
{
    public class EventBase : BaseUnit
    {
        public EventBase()
        {
            Assert.NotNull(CatalogMapping);

            Assert.NotNull(MissionMapping.MissionsList);
        }

        public ICatalogService CatalogMapping { get; set; }
        public IMissionService MissionMapping { get; set; }
        [Fact]
        public void Event_DailyLlamas()
        {
            var dllama = Utils.GetDailyLlamas(CatalogMapping.Catalog);
            Assert.True(dllama != null);
        }

        [Fact]
        public List<IMissionX> Event_TopMissions()
        {
            List<IMissionX> tList = MissionMapping.TopMissions().Take(10).ToList();


            Assert.True(tList != null);
            return tList;
        }

        [Fact]
        public async Task<IEnumerable<IMissionX>> Event_WebhookMissions()
        {
            var tList = await MissionMapping.GetWebhookMissions();

            Assert.True(tList != null);
            return tList;
        }

        [Fact]
        public void Event_TopMissions_TypeChecher()
        {
            var tList = Event_TopMissions();
            var vbuck1 = tList.Where(f => f.HasVBuck()).ToList();

            var eventMissions = Event_WebhookMissions().Result;
            var vbuck2 = eventMissions.Where(f => f.HasVBuck()).ToList();
            var img = tList.First().Items.First().GetRealItemName();
            Assert.True(tList != null);
        }
        [Fact]
        public void Worldinfo_ToJson()
        {
            string jsonMissions = MissionMapping.ToJson();



        }
        [Fact]
        public void Event_StorePVP()
        {
            throw new NotImplementedException();
        }
        [Fact]
        public void Event_StorePVE()
        {

            var stwstore = Catalog.GetSTWStoreSimplified(CatalogMapping.Catalog);
            var str = JsonConvert.SerializeObject(stwstore, Formatting.Indented);
            Assert.True(!string.IsNullOrWhiteSpace(str));
            //to do
        }
    }
}