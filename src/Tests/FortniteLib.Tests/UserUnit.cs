using Fortnite.Core;
using Fortnite.Core.Interfaces;
using Fortnite.Localization;
using Fortnite.Model.Enums;
using Fortnite.Model.Responses.QueryProfile;
using Fortnite.Static;
using Fortnite.Static.Models.Combinations;
using fortniteLib.Responses.Pvp;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Fortnite.Tests
{
    public class UserUnit : BaseUnit
    {
        [Fact]
        [System.Obsolete]
        public void User_UserIdByName()
        {

            var usr = Api.GetUserIdByName("Kesintisiz");
            var prof = Api.GetPVEProfileById(usr.Value.id).Result;
            Assert.True(1 >= 0);
        }

        [Fact]
        public async void User_MythicSchematics()
        {
            var profile = await User_PVEQueryProfile();
            var mythicSchematics = profile.Value.AmountOfMythicSchematics();
            Assert.True(mythicSchematics >= 0);
        }
        [Fact]
        public async void User_Alerts()
        {
            var profile = await User_PVEQueryProfile();
            var resources = await profile.Value.GetAlerts();
            Assert.True(resources.Values.Count > 0);
        }

        [Fact]
        public async void User_Energy_Calculate()
        {
            do
            {
                var profile = await User_PVEQueryProfile();
                IEnumerable<IGrouping<string, ISurvivorX>> survivorslots = await profile.Value.GetSurvivors();
                var points = await survivorslots.CalcSurvivorFORTs();
                int research = await profile.Value.CalcResearchFORTs();
                var totalResources = points + research;
                var energy = await SurvivorStaticData.CalcEnergyByFORT(totalResources);
                Assert.True(energy >= 1);
            }
            while (true);
        }

        [Fact]
        public async Task<KeyValuePair<string, IQueryProfile>> User_PVEQueryProfile()
        {
            var profile = await UserPVEQueryProfile("kesintisiz");
            Assert.True(profile.Value != null);
            return profile;
        }

        [Fact]
        public async Task<KeyValuePair<string, BattleRoyaleStats>> User_PVPQueryProfile()
        {
            var profile = await UserPVPQueryProfile("kesintisiz");
            Assert.True(profile.Value != null);
            return profile;
        }

        [Fact]
        public async void User_Resources()
        {
            var profile = await User_PVEQueryProfile();
            var resources = await profile.Value.GetResources(GuildLanguage.EN.ToString());
            Assert.True(resources != null);
        }

        [Fact]
        public async Task<IEnumerable<IGrouping<string, ISurvivorX>>> User_Survivors()
        {
            var profile = await User_PVEQueryProfile();
            var srvvr = await profile.Value.GetSurvivors();
            Assert.True(srvvr != null);
            return srvvr;
        }

        [Fact]
        public void User_Schematics()
        {
            //to do
        }

        [Fact]
        public async void User_Survivor_List()
        {
            var srvvr = await User_Survivors();
            //to do
        }

        [Fact]
        public async void User_SurvivorSquad_Requirements()
        {
            var svvrs = await User_Survivors();
            var squads = await svvrs.GetSurvivorSquads();
        }

        [Fact]
        public void User_Party()
        {
            //to do
        }

        [Fact]
        public async void User_SurvivorSquad_Cobination()
        {
            var profile = await User_PVEQueryProfile();
            var srvvr = profile.Value.GetSurvivorList().Select(f => new SurvivorCombination
            {
                Name = f.Value.templateId.GName().Result,
                Type = f.Value.templateId.GType().Result,
                SlotType = f.Value.attributes.squad_id,
                Level = (byte)f.Value.attributes.level,
                SlotId = (byte)f.Value.attributes.squad_slot_idx,
                Tier = f.Value.templateId.GTier().Result,
                Personality = f.Value.attributes.personality,
                SetBonus = f.Value.attributes.set_bonus
            }).ToList();
        }
        [Fact]
        public  void Local_TOP20()
        {
            try
            {
                var lst =  Repo.StoredProcedure.SP_LocalTop20Async("465028350067605504");
            }
            catch (System.Exception e)
            {

                throw e;
            }
            Assert.True(true);
        }
        [Fact]
        public async void User_Profile()
        {
            var profile = await User_PVPQueryProfile();
        }

        [Fact]
        public async void User_Profile_PVP_STATS()
        {
            var profile = await User_PVPQueryProfile();
            var sq = profile.Value.stats.BR_Placetop1(MatchType.squad, Platform.all, false);
            var sl = profile.Value.stats.BR_Placetop1(MatchType.solo, Platform.all, false);
            var d = profile.Value.stats.BR_Placetop1(MatchType.duo, Platform.all, false);
            Assert.NotNull(profile.Value);
        }



    }
}