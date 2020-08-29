using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Fortnite.Tests
{
    public class FriendsUnit : BaseUnit
    {
        [Fact]
        public async void FriendList()
        {
            var friends = await FriendsApi.GetFriends();
            Assert.True(friends.Value != null);
        }
    }
}
