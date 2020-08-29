using Fortnite.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fortnite.Core.Services.Events
{
    public class MissionServiceEventArgs : EventArgs, IMissionServiceEventArgs
    {
        public IEnumerable<IMissionX> Missions { set; get; }

        public MissionServiceEventArgs(IEnumerable<IMissionX> missions)
        {
            Missions = missions;
        }
    }
}
