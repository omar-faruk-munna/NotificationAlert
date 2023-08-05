using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationAlert.Api.Repositories;
using NotificationAlert.Api.Services;
using System;

namespace NotificationAlert.Api
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
            services.AddHttpClient("SslClient", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection("SSL_SMS").GetSection("URI").Value);
            });
            services.AddHttpClient("CustomSslClient", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection("SSL_SMS").GetSection("URI").Value);
            });
            services.AddHttpClient("BankClient", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("WebAPIBaseUrl"));
                //c.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            services.AddHttpClient("CustomBankClient", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("WebAPIBaseUrl"));
                //c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            //services.AddHttpClient("XmlClientTest", c =>
            //{
            //    c.BaseAddress = new Uri(Configuration.GetValue<string>("PayBillURL"));
            //    // Account API ContentType
            //    c.DefaultRequestHeaders.Add("Accept", "application/xml");
            //});


            // DI
            services.AddSingleton(Configuration);
            services.AddScoped<INasRepo, NasRepo>();
            services.AddScoped<ICustomSmsRepository, CustomSmsService>();
            services.AddScoped<IEmailRepository, EmailService>();
            services.AddScoped<ISmsRepository, SmsService>();
            services.AddScoped<ILogRepository, LogService>();
            services.AddScoped<IUltimusConString, UltimusConString>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
