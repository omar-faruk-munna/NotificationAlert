using Database;
using LogLog4Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationAlertCustom.Middlewares;
using NotificationAlertCustom.Repositories;
using NotificationAlertCustom.Services;
using Sms;
using System;

namespace NotificationAlertCustom
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllers();
            //services.AddHttpClient();

            services.AddHttpClient(Configuration.GetValue<string>("Client:SslName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("SslSms:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddHttpClient(Configuration.GetValue<string>("Client:InfobipName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("InfobipSms:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddHttpClient(Configuration.GetValue<string>("Client:MetrotelName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("MetrotelSms:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddHttpClient(Configuration.GetValue<string>("Client:RobiName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("Robi:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddHttpClient(Configuration.GetValue<string>("Client:BlName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("Banglalink:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddHttpClient(Configuration.GetValue<string>("Client:GpName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("GrameenPhone:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            services.AddHttpClient(Configuration.GetValue<string>("Client:UflName"), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("UflSms:Domain"));
                c.DefaultRequestVersion = new Version(2, 0);
            });

            //services.AddHttpClient("encrypt", c =>
            //{
            //    c.BaseAddress = new Uri(Configuration.GetSection("Encryption").GetSection("Domain").Value);
            //    c.DefaultRequestVersion = new Version(2, 0);
            //});

            //services.AddHttpClient("XmlClientTest", c =>
            //{
            //    c.BaseAddress = new Uri(Configuration.GetValue<string>("PayBillURL"));
            //    // Account API ContentType
            //    c.DefaultRequestHeaders.Add("Accept", "application/xml");
            //});


            // DI
            services.AddSingleton(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICustomSmsRepository, CustomSmsService>();
            services.AddScoped<ISmsRepository, SmsRepository>();
            services.AddScoped<ILoggerRepository, LoggerRepository>();
            services.AddScoped<IDbRepository, DbRepository>();
            //services.AddScoped<IEmailRepository, EmailService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseIPFilterMiddleware();

            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
