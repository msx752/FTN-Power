using System;

namespace Fortnite.Model.Enums
{
    public class AuthVerify
    {
        public string token { get; set; }
        public string session_id { get; set; }
        public string token_type { get; set; }
        public string client_id { get; set; }
        public bool internal_client { get; set; }
        public string client_service { get; set; }
        public string account_id { get; set; }
        public int expires_in { get; set; }
        public DateTime expires_at { get; set; }
        public string auth_method { get; set; }
        public DateTime lastPasswordValidation { get; set; }
        public string app { get; set; }
        public string in_app_id { get; set; }
        public string device_id { get; set; }
    }
}