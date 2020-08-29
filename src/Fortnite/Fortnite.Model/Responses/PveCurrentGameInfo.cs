using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fortnite.Model.Responses
{
    public class PveCurrentGameInfo
    {
        public string id { get; set; }
        public string ownerId { get; set; }
        public string ownerName { get; set; }
        public string serverName { get; set; }
        public string serverAddress { get; set; }
        public int serverPort { get; set; }
        public int maxPublicPlayers { get; set; }
        public int openPublicPlayers { get; set; }
        public int maxPrivatePlayers { get; set; }
        public int openPrivatePlayers { get; set; }
        public Attributes attributes { get; set; }
        public List<string> publicPlayers { get; set; }
        public List<object> privatePlayers { get; set; }
        public int totalPlayers { get; set; }
        public bool allowJoinInProgress { get; set; }
        public bool shouldAdvertise { get; set; }
        public bool isDedicated { get; set; }
        public bool usesStats { get; set; }
        public bool allowInvites { get; set; }
        public bool usesPresence { get; set; }
        public bool allowJoinViaPresence { get; set; }
        public bool allowJoinViaPresenceFriendsOnly { get; set; }
        public string buildUniqueId { get; set; }
        public DateTime lastUpdated { get; set; }
        public bool started { get; set; }
    }

    public class Attributes
    {
        public string REGION_s { get; set; }
        public string GAMEMODE_s { get; set; }
        public bool GATHERABLE_b { get; set; }
        public int MMLVL_i { get; set; }
        public string SUBREGION_s { get; set; }
        public string DCID_s { get; set; }
        public bool CRITICALMISSION_b { get; set; }
        public int NEEDS_i { get; set; }
        public int NEEDSSORT_i { get; set; }
        public string tenant_s { get; set; }
        public string MATCHMAKINGPOOL_s { get; set; }
        public int PLAYLISTID_i { get; set; }
        public int MAXDIFFICULTY_i { get; set; }
        public int HOTFIXVERSION_i { get; set; }
        public int PARTITION_i { get; set; }
        public int MAXDIFFICULTYSORT_i { get; set; }
        public string THEATERID_s { get; set; }
        public string TENANT_s { get; set; }
        public string ZONEINSTANCEID_s { get; set; }
        public int BEACONPORT_i { get; set; }
        public int MINDIFFICULTY_i { get; set; }

        public ZoneInstance GetZoneInstance()
        {
            ZoneInstance obj = JsonConvert.DeserializeObject<ZoneInstance>(ZONEINSTANCEID_s);
            return obj;
        }
    }

    public class ZoneInstance
    {
        public string worldId { get; set; }
        public string theaterId { get; set; }
        public string theaterMissionId { get; set; }
        public string theaterMissionAlertId { get; set; }
        public string zoneThemeClass { get; set; }

        [JsonIgnore]
        public bool IsSSD
        {
            get
            {
                var mapn = zoneThemeClass?.Split('.')[1];
                return mapn.Contains("TheOutpost_PvE", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public string GetMapName()
        {
            var mapn = zoneThemeClass?.Split('.')[1];
            if (IsSSD)
            {
                mapn = "**Storm Shield Defense**";
            }
            else
            {
                mapn = "ordinary";
            }
            return mapn;
        }
    }
}