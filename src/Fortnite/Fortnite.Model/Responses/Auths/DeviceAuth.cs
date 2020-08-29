namespace fortniteLib.Responses.Auths
{
    public class DeviceAuth
    {
        public string deviceId { get; set; }
        public string accountId { get; set; }
        public string secret { get; set; }
        public string userAgent { get; set; }
        public DeviceAuthCreated created { get; set; }
    }
}