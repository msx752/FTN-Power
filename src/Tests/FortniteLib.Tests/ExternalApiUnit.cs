using Fortnite.External.Responses.BDailyStore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fortnite.Tests
{
    public class ExternalApiUnit : ExternalApiBaseUnit
    {
        [Fact]
        public async void Event_BrDailyStore()
        {
            var brstore = Api.GetBattleRoyaleDailyStore();
            var title = brstore.Value.GetBrDailyTitle();
            await brstore.Value?.GetBrDailyImageAsync();
            Assert.True(brstore.Value != null);
        }

    }
}
