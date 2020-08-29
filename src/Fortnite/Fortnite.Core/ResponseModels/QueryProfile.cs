using Fortnite.Model.Responses.QueryProfile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fortnite.Core.ResponseModels
{
    public class QueryProfile : IQueryProfile, IDisposable
    {
        private bool disposedValue;

        public QueryProfile()
        {
            profileChanges = new List<ProfileChange>();
        }

        public List<ProfileChange> profileChanges { get; set; }
        public int? profileChangesBaseRevision { get; set; }
        public int? profileCommandRevision { get; set; }
        public string profileId { get; set; }
        public int? profileRevision { get; set; }
        public int? responseVersion { get; set; }
        public DateTimeOffset? serverTime { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    profileChanges = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~QueryProfile()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}