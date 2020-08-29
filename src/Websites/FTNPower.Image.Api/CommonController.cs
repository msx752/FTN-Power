using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FTNPower.Image.Api
{
    public class CommonController : ControllerBase
    {
        internal bool IsAuthorized()
        {
            if (!Request.Query.ContainsKey("aKey") || Request.Query["aKey"] != ImageProvider.ISVC.AKey)
                return false;
            return true;
        }
    }
}
