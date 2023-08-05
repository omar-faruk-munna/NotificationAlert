using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace WorkerServiceCustom
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient("ssl2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("SslSms").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("infobip2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("InfobipSms").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("metrotel2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("MetrotelSms").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("robi2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Robi").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("banglalink2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Banglalink").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("grameenphone2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("GrameenPhone").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("ufl2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("UflSms").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("push2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Push").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHttpClient("encrypt2", c =>
                    {
                        c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Encryption").GetSection("Domain").Value);
                        c.DefaultRequestVersion = new Version(2, 0);
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
