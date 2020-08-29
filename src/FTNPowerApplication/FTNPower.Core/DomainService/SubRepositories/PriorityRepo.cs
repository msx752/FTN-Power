using FTNPower.Core.ApplicationService;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FTNPower.Core.DomainService.SubRepositories
{
    public class PriorityRepo
    {
        private readonly Random _random;
        private readonly IRedisService _redis;
        private readonly IUnitOfWork _uow;

        public PriorityRepo(IUnitOfWork uow, IRedisService redisService)
        {
            _random = new Random(Guid.NewGuid().GetHashCode());
            _uow = uow;
            _redis = redisService;
        }

        public IEnumerable<PriorityTable> Priorities
        {
            get
            {
                IEnumerable<PriorityTable> cachedPriorityTable = _redis.JsonGet<List<PriorityTable>>();
                if (cachedPriorityTable == null)
                {
                    cachedPriorityTable = _uow.Db<PriorityTable>().All();
                    _redis.JsonSet(cachedPriorityTable, expiry: new TimeSpan(6, 0, 0));
                }
                return cachedPriorityTable;
            }
        }

        public PriorityTable GetPriorityTable(string id, PriorityState pstate = PriorityState.User)
        {
            string type = null;
            switch (pstate)
            {
                case PriorityState.User:
                    type = "u";
                    break;

                case PriorityState.Guild:
                    type = "s";
                    break;
            }
            var Qid = $"{type}{id}";

            return Priorities.Where(f => f.Id == Qid).FirstOrDefault();
        }

        public PriorityTable GetRandomAdvertGuildPartners()
        {
            var adverts = GetValidGuildPartners().Where(f => f.AdvertOn).ToList();
            var count = adverts.Count;
            if (count > 0)
            {
                var index = _random.Next(0, (int)(count * 1.2));
                if (index > count - 1)
                    return null;
                var guid = adverts[index];
                return guid;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<PriorityTable> GetValidGuildPartners(string spesificGuildId = null)
        {
            var lst = Priorities.Where(f => f.State == PriorityState.Guild && (f.Deadline - DateTimeOffset.UtcNow).TotalSeconds > 0);

            if (!string.IsNullOrWhiteSpace(spesificGuildId))
                lst = lst.Where(p => p.Id == $"s{spesificGuildId}");
            return lst;
        }

        public IEnumerable<PriorityTable> GetValidUserPartners(string spesificUserId = null)
        {
            var lst = Priorities.Where(f => f.State == PriorityState.User && (f.Deadline - DateTimeOffset.UtcNow).TotalSeconds > 0);

            if (!string.IsNullOrWhiteSpace(spesificUserId))
                lst = lst.Where(p => p.Id == $"u{spesificUserId}");
            return lst;
        }

        public KeyValuePair<bool, PriorityTable> IsPartnerGuild(object guildId)
        {
            var isPartner = GetValidGuildPartners(guildId.ToString()).FirstOrDefault();
            if (isPartner != null)
                return new KeyValuePair<bool, PriorityTable>(true, isPartner);
            else
                return new KeyValuePair<bool, PriorityTable>(true, new PriorityTable()/*project has been shutdown, must be 'false'*/
                {
                    Deadline = DateTime.UtcNow.AddDays(3),
                    Id = $"s{guildId.ToString()}",
                    AdvertOn = true,
                    AdvertCustomText = "Kesintisiz",
                });
        }

        public KeyValuePair<bool, PriorityTable> IsPartnerUser(object userId)
        {
            var isPartner = GetValidUserPartners(userId.ToString()).FirstOrDefault();
            if (isPartner != null)
                return new KeyValuePair<bool, PriorityTable>(true, isPartner);
            else
                return new KeyValuePair<bool, PriorityTable>(false, new PriorityTable());
        }

        public PriorityTable UpdatePriority(string id, string extendFormat, string customText = null)
        {
            Regex extendRegex = new Regex("(\\d+)M|(\\d+)d|(\\d+)h");
            Regex extendId = new Regex("(s|u)(\\d+)");
            var extendMatch = extendRegex.Match(extendFormat);
            var idMatch = extendId.Match(id.ToLowerInvariant());
            if (extendMatch.Success && idMatch.Success)
            {
                PriorityTable tbl = _uow.Db<PriorityTable>().GetById(id);
                if (tbl == null)
                {
                    tbl = new PriorityTable
                    {
                        Id = id,
                        Deadline = DateTimeOffset.UtcNow.AddMilliseconds(-1),
                        Notified = false
                    };
                    tbl = _uow.Db<PriorityTable>().Add(tbl).Entity;
                    _uow.Commit();
                }
                else
                {
                    if (!((tbl.Deadline - DateTimeOffset.UtcNow).TotalSeconds > 0))
                        tbl.Deadline = DateTimeOffset.UtcNow.AddMilliseconds(-1);
                }
                if (!string.IsNullOrWhiteSpace(customText))
                {
                    if (customText.Equals("null", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tbl.AdvertOn = false;
                    }
                    else
                    {
                        tbl.AdvertCustomText = customText;
                        tbl.AdvertOn = true;
                    }
                }
                var months = extendMatch.Groups[1];
                if (months.Success)
                {
                    var m = int.Parse(months.Value);
                    tbl.Deadline = tbl.Deadline.AddMonths(m).AddSeconds(15);
                    if (m > 0) tbl.Notified = false;
                }
                var days = extendMatch.Groups[2];
                if (days.Success)
                {
                    var d = int.Parse(days.Value);
                    tbl.Deadline = tbl.Deadline.AddDays(d).AddSeconds(15);
                    if (d > 0) tbl.Notified = false;
                }
                var hours = extendMatch.Groups[3];
                if (hours.Success)
                {
                    var h = int.Parse(hours.Value);
                    tbl.Deadline = tbl.Deadline.AddHours(h).AddSeconds(15);
                    if (h > 0) tbl.Notified = false;
                }
                _uow.Db<PriorityTable>().Update(tbl);
                _uow.Commit();

                _redis.JsonDelete<List<PriorityTable>>();
                return tbl;
            }
            else
            {
                return null;
            }
        }
    }
}