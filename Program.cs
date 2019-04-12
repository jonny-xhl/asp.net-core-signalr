using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace Study.SignalRdemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSetting("https_port","1111")
                .UseKestrel(options=>
                {
                    options.UseSystemd(config =>
                    {
                        config.UseHttps(new HttpsConnectionAdapterOptions
                        {                            
                            ServerCertificate=new X509Certificate2(Path.Combine(AppContext.BaseDirectory, "signalr.pfx"),
                            "123456"),
                            ClientCertificateMode=ClientCertificateMode.AllowCertificate
                        });
                    });
                })
                .UseStartup<Startup>();
    }
}
