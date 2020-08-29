using System;
using System.Collections.Generic;

namespace FTNPower.Model.WebsiteModels
{
    public class CustomVariables
    {
        public CustomVariables()
        {
            DeveloperDiscordIds = new List<ulong>()
            {
                /*Kesintisiz#0223*/
                193749607107395585,
                /*Alaykaçıran#9458*/
                222421754222608387,
            };
            discordbots_org_Id = "264445053596991498";

            MaxAccountPowerLevel = 131.99;
            Lightning = "⚡";
            Trophy = "🏆";
            Interval = new TimeSpan(0, 0, 5);
            QueueLength = 25;
        }
        public List<ulong> DeveloperDiscordIds { get; set; }
        public string discordbots_org_Id { get; set; }
        public double MaxAccountPowerLevel { get; set; }
        public string Lightning { get; set; }
        public string Trophy { get; set; }
        public TimeSpan Interval { get; set; }
        public ConsoleColor DefaultColor { get; set; }
        public int QueueLength { get; }

        public bool IgnoreRequest { get; set; }
    }
}