using FTNPower.Model.WebsiteModels;
using FTNPower.Static;
using Newtonsoft.Json;
using System;

namespace FTNPower
{
    class Program
    {
        static void Main(string[] args)
        {
            FTNPowerMain mainBot = new FTNPowerMain();
            mainBot.StartBot().GetAwaiter().GetResult();
        }
    }
}
