using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;
using Fortnite.Model.Enums;
using RestSharp.Extensions;
using Global;

namespace Fortnite.Api
{
    public class BaseApi
    {
        public readonly string fortniteapi = "https://fortnite-public-service-prod11.ol.epicgames.com";
        public readonly string oauth = "https://account-public-service-prod03.ol.epicgames.com";
        public readonly string persona = "https://persona-public-service-prod06.ol.epicgames.com";
        public readonly string epicgames = "https://www.epicgames.com";
        public readonly string friendList = "https://friends-public-service-prod06.ol.epicgames.com";
        public readonly string statproxy = "https://statsproxy-public-service-live.ol.epicgames.com";
        public readonly string account = "https://account-public-service-prod.ol.epicgames.com";
        private readonly string basicToken = "basic MzQ0NmNkNzI2OTRjNGE0NDg1ZDgxYjc3YWRiYjIxNDE6OTIwOWQ0YTVlMjVhNDU3ZmI5YjA3NDg5ZDMxM2I0MWE=";
        public readonly RestClient fortniteapiClient = new RestClient("https://fortnite-public-service-prod11.ol.epicgames.com")
        { CookieContainer = new CookieContainer() };
        public readonly RestClient oauthClient = new RestClient("https://account-public-service-prod03.ol.epicgames.com")
        { CookieContainer = new CookieContainer() };
        public readonly RestClient personaClient = new RestClient("https://persona-public-service-prod06.ol.epicgames.com")
        { CookieContainer = new CookieContainer() };
        public readonly RestClient epicgamesClient = new RestClient("https://www.epicgames.com")
        { CookieContainer = new CookieContainer() };
        public readonly RestClient friendListClient = new RestClient("https://friends-public-service-prod06.ol.epicgames.com")
        { CookieContainer = new CookieContainer() };
        public readonly RestClient statproxyClient = new RestClient("https://statsproxy-public-service-live.ol.epicgames.com")
        { CookieContainer = new CookieContainer() };
        public readonly RestClient accountClient = new RestClient("https://account-public-service-prod.ol.epicgames.com")
        { CookieContainer = new CookieContainer() };

        private readonly object _lockAuthorization = new object();
        private AuthToken _authorization;
        private readonly object _lock_ReceivedBearerToken = new object();

        private bool _receivedBearerToken = false;
        public string Username { get; private set; }
        public string Password { get; set; }

        public Action OnTokenSuccessfullyRefreshed = null;
        public Action OnTokenError = null;
        public void SetIdentity(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
        public AuthToken Authorization
        {
            get { lock (_lockAuthorization) { return _authorization; } }
            set
            {
                lock (_lockAuthorization) { _authorization = value; }
                fortniteapiClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
                oauthClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
                personaClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
                epicgamesClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
                friendListClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
                statproxyClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
                accountClient.AddOrUpdateDefaultParameter(new Parameter("Authorization", $"{_authorization.token_type} {_authorization.access_token}", ParameterType.HttpHeader));
            }
        }

        public bool ReceivedBearerToken { get { lock (_lock_ReceivedBearerToken) { return _receivedBearerToken; } } set { lock (_lock_ReceivedBearerToken) { _receivedBearerToken = value; } } }

        public KeyValuePair<string, AuthVerify> Verify()
        {
            var rRequest = new RestRequest($"account/api/oauth/verify", Method.GET, DataFormat.Json);
            var result = RequestExecute(oauthClient, rRequest);

            if (result.ResponseStatus == ResponseStatus.Error || result.StatusCode != HttpStatusCode.OK)
            {
                return new KeyValuePair<string, AuthVerify>(result.Content, null);
            }

            return new KeyValuePair<string, AuthVerify>(result.ResponseStatus.ToString(), CastContent<AuthVerify>(result));
        }
        public bool KillOtherSessions()
        {
            var rRequest = new RestRequest("account/api/oauth/sessions/kill?killType=OTHERS_ACCOUNT_CLIENT_SERVICE", Method.DELETE, DataFormat.Json);

            var response = RequestExecute(oauthClient, rRequest);

            return response.StatusCode == HttpStatusCode.NoContent;
        }

        public AuthToken FTNToken()
        {

            object bodyObj = new
            {
                UserName = this.Username,
                Password = this.Password,
                SecurityCode = Global.DIManager.Services.DiscordBotConfigs().FTNTokenSecurityCode
            };
            var rRequest = new RestRequest("api/FTNPower/Token", Method.POST, DataFormat.Json)
                .AddJsonBody(bodyObj);

            var response = RequestExecute(new RestClient(Global.DIManager.Services.DiscordBotConfigs().FTNTokenHost), rRequest);


            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(response.StatusDescription);
            }

            return CastContent<AuthToken>(response);
        }

        //https://github.com/Terbau/fortnitepy/blob/875ad1e2f6b3da082795c8b10b00bb6075b54b62/fortnitepy/http.py
        private IRestResponse account_generate_device_auth(string client_id)
        {
            IRestRequest devideAuthRequest = new RestRequest($"account/api/public/account/{client_id}/deviceAuth", Method.POST, DataFormat.Json)
                .AddJsonBody(new { });

            var devideAuthResponse = RequestExecute(oauthClient, devideAuthRequest);
            return devideAuthResponse;
        }

        public AuthToken DeviceToken(string device_id, string account_id, string secret)
        {
            var deviceAuthRequest = new RestRequest("account/api/oauth/token", Method.POST)
                .AddHeader("Content-Type", "application/x-www-form-urlencoded")
                .AddHeader("Authorization", basicToken)
                .AddParameter("application/x-www-form-urlencoded", $"grant_type=device_auth&device_id={device_id}&account_id={account_id}&secret={secret}&token_type=eg1", "x-www-form-urlencoded", ParameterType.RequestBody);
            var authResponse = RequestExecute(oauthClient, deviceAuthRequest);
            if (authResponse.StatusCode != HttpStatusCode.OK)
            {
                // MyLogger.Log.Fatal("{lt}: Invalid device auth details passed", "BaseAPI");
            }

            return CastContent<AuthToken>(authResponse);
        }

        private IRestRequest RequestManage(IRestClient restClient, IRestRequest restRequest)
        {
            var restRestRequest = restRequest;
            restRestRequest.AddHeader("User-Agent", "Fortnite/++Fortnite+Release-40.00-CL-5555556 Windows/10.0.19001.2.254.64bit");

            return restRestRequest;
        }
        public IRestResponse RequestExecute(IRestClient restClient, IRestRequest restRequest)
        {
            restRequest = RequestManage(restClient, restRequest);
            if (restClient.RemoteCertificateValidationCallback == null)
            {
                restClient.RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            }
            var resp = restClient.Execute(restRequest);
            return resp;
        }
        public static T CastContent<T>(IRestResponse restResponse)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(restResponse.Content, new JsonSerializerSettings());
            }

            catch (Exception e)
            {
                return default(T);
            }
        }
        Thread thrVerify = null;
        public void StartFortniteTokenVerificator()
        {
            thrVerify = new Thread(Verifier);
            thrVerify.Start();
        }

        private void Verifier()
        {
            int c = 0;
            while (true)
            {
                try
                {
                    var auth = Verify();
                    if (auth.Value == null)
                    {
                        c++;
                        Console.WriteLine("token try->" + c);
                        var token = FTNToken();
                        if (token != null)
                        {
                            this.Authorization = token;
                            this.ReceivedBearerToken = true;
                            if (OnTokenSuccessfullyRefreshed != null)
                                OnTokenSuccessfullyRefreshed();
                        }
                        else
                        {
                            this.ReceivedBearerToken = false;
                            Console.WriteLine($"EPIC AUTH LOGIN ERROR.");
                            Thread.Sleep(4000);
                        }
                    }
                    else
                    {
                        c = 0;
                        if (!this.ReceivedBearerToken)
                        {
                            this.ReceivedBearerToken = true;
                        }
                    }
                }
                catch (Exception e)
                {
                    this.ReceivedBearerToken = false;
                    if (OnTokenError != null)
                        OnTokenError();
                    Console.WriteLine($"EPIC AUTH VERIFIER ERROR. {e.Message}");
                }
                Thread.Sleep(1111);
            }
        }
    }
}
