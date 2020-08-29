using Discord;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTNPower.Model
{
    public static class Utils
    {
        public static int GetIntegerPower(double AccountPowerLevel)
        {
            var energy = (int)Math.Round(AccountPowerLevel, MidpointRounding.AwayFromZero);
            if (energy > 131)
            {
                energy = 131;
            }
            return energy;
        }
        public static UInt64 ToUlong(this string str)
        {
            return UInt64.Parse(str);
        }

    }
}
