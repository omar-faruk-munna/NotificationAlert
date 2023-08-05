using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace NasWorker
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
            //.UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration).CreateLogger();
                services.AddHttpClient("ssl", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("SslSms").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("infobip", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("InfobipSms").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("metrotel", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("MetrotelSms").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("robi", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Robi").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("banglalink", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Banglalink").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("grameenphone", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("GrameenPhone").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("ufl", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("UflSms").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("push", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Push").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("encrypt", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("Encryption").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHttpClient("sslnew", c =>
                {
                    c.BaseAddress = new Uri(hostContext.Configuration.GetSection("SslSmsNew").GetSection("Domain").Value);
                    c.DefaultRequestVersion = new Version(2, 0);
                });

                services.AddHostedService<Worker>();
            });
    }
}
