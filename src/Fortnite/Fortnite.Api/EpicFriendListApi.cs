using fortniteLib.Responses.FriendList;
using Global;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Fortnite.Api
{
    public class EpicFriendListApi : BaseApi, IEpicFriendListApi
    {
        public bool IsAuthorized
        {
            get { return base.ReceivedBearerToken; }
        }
        public Task<KeyValuePair<string, List<Friend>>> GetFriends(bool includePending = true)
        {
            return Task.Run(() =>
            {
                var pendingState = includePending ? "true" : "false";
                var rRequest = new RestRequest($"friends/api/public/friends/{base.Authorization.account_id}", Method.GET, DataFormat.Json)
                 .AddQueryParameter("includePending", pendingState);

                var result = RequestExecute(friendListClient, rRequest);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new KeyValuePair<string, List<Friend>>(result.ErrorMessage ?? result.ErrorException?.ToString(), null);
                }

                return new KeyValuePair<string, List<Friend>>(result.StatusCode.ToString(), CastContent<List<Friend>>(result));
            });
        }
        public Task<bool> AcceptFriendRequest(string epicId)
        {
            return Task.Run(() =>
            {
                var rRequest = new RestRequest($"friends/api/public/friends/{base.Authorization.account_id}/{epicId}", Method.DELETE);

                var result = RequestExecute(friendListClient, rRequest);

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
        public Task<bool> DeclineFriendRequest(string epicId)
        {
            return Task.Run(() =>
            {
                var rRequest = new RestRequest($"friends/api/public/friends/{base.Authorization.account_id}/{epicId}", Method.DELETE);

                var result = RequestExecute(friendListClient, rRequest);

                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }
        public Task StartVerifier()
        {
            base.SetIdentity(DIManager.Services.EpicFriendListApiConfigs().UserName, DIManager.Services.EpicFriendListApiConfigs().Password);

            return Task.Run(() =>
            {
                try
                {
                    Task.Delay(4000);
                    var token = FTNToken();
                    if (token != null)
                    {
                        Global.Log.Information("{lt}: Successfully LoggedIn..", "EpicFriendAPI");
                        this.Authorization = token;
                        this.ReceivedBearerToken = true;
                        Global.Log.Information("{lt}: Ready", "EpicFriendAPI");
                        base.StartFortniteTokenVerificator();
                        return;
                    }
                    else
                    {
                          Global.Log.Fatal("{lt} AUTH LOGIN CRITICAL ERROR.", "EpicFriendAPI");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            });

        }

    }
}
