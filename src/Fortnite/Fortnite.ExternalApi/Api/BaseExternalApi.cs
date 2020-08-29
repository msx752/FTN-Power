using Global;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;

namespace Fortnite.External.Api
{
    public class BaseExternalApi
    {
        public readonly string fortniteapiExternal = "API_URL_RETRIEWS_BR_STORE_IMAGES_FROM_EXTERNAL_SERVİCE_AS_A_JSON_FORMAT";//determine external services to retriew json format data
        public ExternalResponse DoGet(string url, string path = "", string query = null)
        {
            if (query != null)
                path = $"{path}?{query}";

            var request = CreateRequest($"{url}/{path}", "GET");
            ExternalResponse response = GetResponse(request);
            return response;
        }

        private HttpWebRequest CreateRequest(string url, string method)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            request.Headers.Add("Authorization", DIManager.Services.ExternalApiConfigs().Authorization);
            request.ContentType = "application/json";
            request.ServerCertificateValidationCallback = _ServerCertificateValidationCallback;
            request.Method = method;
            return request;
        }
        public static bool _ServerCertificateValidationCallback(object sender, X509Certificate? cert, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
     

        private ExternalResponse GetResponse(WebRequest request)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return new ExternalResponse()
                {
                    Data = responseString,
                    StatusCode = response.StatusCode,
                    ErrorMessage = null
                };
            }
            catch (WebException e)
            {
                return new ExternalResponse()
                {
                    StatusCode = ((HttpWebResponse)e.Response).StatusCode,
                    ErrorMessage = e.Message
                };
            }
            catch (Exception e)
            {
                return new ExternalResponse()
                {
                    ErrorMessage = e.Message
                };
            }
        }
    }
}