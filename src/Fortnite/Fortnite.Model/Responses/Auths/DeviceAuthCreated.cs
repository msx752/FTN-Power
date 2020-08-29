using System;

namespace fortniteLib.Responses.Auths
{
    public class DeviceAuthCreated
    {
        public string location { get; set; }
        public string ipAddress { get; set; }
        public DateTime dateTime { get; set; }
    }
}