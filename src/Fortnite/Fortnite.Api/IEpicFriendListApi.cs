using fortniteLib.Responses.FriendList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fortnite.Api
{
    public interface IEpicFriendListApi
    {
        bool IsAuthorized { get; }

        Task<bool> AcceptFriendRequest(string epicId);
        Task<bool> DeclineFriendRequest(string epicId);
        Task<KeyValuePair<string, List<Friend>>> GetFriends(bool includePending = true);
        Task StartVerifier();
    }
}