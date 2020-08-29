using Fortnite.Core.Interfaces;
using System.Collections.Generic;

namespace Fortnite.Core.Services.Events
{
    public interface IMissionServiceEventArgs
    {
        IEnumerable<IMissionX> Missions { get; set; }
    }
}