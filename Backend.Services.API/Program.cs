
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyShopOnLine.Backend.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#region Top Level Statements

//// https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9
//CreateHostBuilder(args).Build().Run();

//static IHostBuilder CreateHostBuilder(string[] args) =>
//    Host.CreateDefaultBuilder(args)
//    .ConfigureWebHostDefaults(webBuilder =>
//    {
//        webBuilder.UseStartup<Startup>();
//    });

#endregion


namespace MyShopOnLine.Backend.API
{

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
