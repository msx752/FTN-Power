using System;
using System.Collections.Generic;

namespace Fortnite.Model.Responses.QueryProfile
{
    public class Profile
    {
        public Profile()
        {
        }

        public string _id { get; set; }
        public string accountId { get; set; }
        public int commandRevision { get; set; }
        public DateTimeOffset created { get; set; }
        public Dictionary<string, Item> items { get; set; }
        public string profileId { get; set; }
        public int rvn { get; set; }
        public Dictionary<string, StatAttribute> stats { get; set; }
        public DateTimeOffset updated { get; set; }
        public string version { get; set; }
        public int wipeNumber { get; set; }
    }
}