using System;
using System.Collections.Generic;

namespace Fortnite.Model.Responses.QueryProfile
{
    public interface IQueryProfile
    {
        List<ProfileChange> profileChanges { get; set; }
        int? profileChangesBaseRevision { get; set; }
        int? profileCommandRevision { get; set; }
        string profileId { get; set; }
        int? profileRevision { get; set; }
        int? responseVersion { get; set; }
        DateTimeOffset? serverTime { get; set; }
    }
}