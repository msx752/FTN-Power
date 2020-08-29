
using Fortnite.Api;
using Global;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace fortniteLib.Tests
{
    public class AuthUnit
    {

        [Fact]
        public void Grant_Token_RestSharp_Bot()
        {
            EpicApi api = new EpicApi();
            api.SetIdentity(DIManager.Services.EpicApiConfigs().UserName, DIManager.Services.EpicApiConfigs().Password);
            var token = api.DeviceToken(DIManager.Services.EpicApiConfigs().DeviceId, DIManager.Services.EpicApiConfigs().AccountId, DIManager.Services.EpicApiConfigs().DeviceSecret);
            var verify = api.Verify();
            //api.KillOtherSessions();
            Assert.NotNull(token);
            Assert.NotNull(verify.Value);
        }
        [Fact]
        public void Grant_Token_RestSharp_FriendList()
        {
            EpicFriendListApi api = new EpicFriendListApi();
            api.SetIdentity(DIManager.Services.EpicFriendListApiConfigs().UserName, DIManager.Services.EpicFriendListApiConfigs().Password);
            var token = api.DeviceToken(DIManager.Services.EpicFriendListApiConfigs().DeviceId, DIManager.Services.EpicFriendListApiConfigs().AccountId, DIManager.Services.EpicFriendListApiConfigs().DeviceSecret);
            var verify = api.Verify();
            //api.KillOtherSessions();
            Assert.NotNull(token);
            Assert.NotNull(verify.Value);
        }
    }
}
