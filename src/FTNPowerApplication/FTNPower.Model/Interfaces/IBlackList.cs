using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Model.Interfaces
{
    public interface IBlackList
    {
        string Id { get; set; }

        DateTimeOffset Until { get; set; }
    }
}
