using FTNPower.Model.Enums;
using FTNPower.Model.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Data
{
    public static class Utils
    {
        public static bool USE_LOCAL_DBCONTEXT = CheckLocalDBContext(false);

        public static bool CheckLocalDBContext(bool forceLiveDBContext = false)
        {
            if (forceLiveDBContext)
            {
#if DEBUG
                Console.WriteLine("APPLICATION ON DEBUG MODE but FORCING TO USE [LIVE SQL DATABASE]");
#endif
                return false;//forcing live database
            }
#if DEBUG
            return true;
#endif

#if RELEASE
            return false;
#endif
        }

        public static ulong GetUlongId(this PriorityTable pt)
        {
            return ulong.Parse(pt.Id.Substring(1));
        }
        public static string ToJsonString(this object bgr)
        {
            if (bgr is string)
                return (string)bgr;

            if (bgr == null)
                throw new NullReferenceException("undefined redis value to push into list");

            return JsonConvert.SerializeObject(bgr);
        }
        public static bool CheckValidity(this PriorityTable pt)
        {
            return pt.Remining.TotalSeconds > 0;
        }
        public static PriorityState GetPriorityState(this PriorityTable pt)
        {
            if (pt.CheckValidity())
            {
                return pt.State;
            }
            else
            {
                return PriorityState.Normal;
            }
        }
    }
}
