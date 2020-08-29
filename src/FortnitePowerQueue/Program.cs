using Fortnite;
using Fortnite.External.ServiceStore;
using FTNPower.Core;
using FTNPower.Core.DomainService;
using FTNPower.Core.Interfaces;
using FTNPower.Model.Tables;
using FTNPower.Queue;
using FTNPower.Redis.Messaging.AutoRemove;
using FTNPower.Redis.Messaging.AutoUserUpdate;
using FTNPower.Redis.Messaging.ProfileVerifier;
using Global;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Globalization;
using System.Runtime;
using System.Threading.Tasks;

namespace FortnitePowerQueue
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
                   webBuilder.UseUrls("http://127.0.0.1:5011");
#if DEBUG
                   webBuilder.UseEnvironment("Development");
#elif RELEASE
                   webBuilder.UseEnvironment("Production");
#endif
               });

        private static void Main(string[] args)
        {
            var cultureInfo = new CultureInfo("en-GB");
            cultureInfo.NumberFormat.CurrencySymbol = "£";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            Console.WriteLine("The '{0}' garbage collector is running.", GCSettings.IsServerGC ? "server" : "workstation");
            CreateHostBuilder(args).Build().Run();
        }

    }
}