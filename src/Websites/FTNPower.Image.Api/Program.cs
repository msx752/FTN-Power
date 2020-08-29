using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime;

namespace FTNPower.Image.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("The '{0}' garbage collector is running.", GCSettings.IsServerGC ? "server" : "workstation");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:5010");
#if DEBUG
                    webBuilder.UseEnvironment("Development");
#elif RELEASE
                    webBuilder.UseEnvironment("Production");
#endif
                });
    }
}