﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MotleyFlash;
using MotleyFlash.AspNetCore.MessageProviders;
using TimeTracking.DataAccess;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Helpers;
using TimeTracking.Web.Config;
using TimeTracking.Web.Helpers;
using TimeTracking.Web.Services;
using TimeTracking.Web.Services.Interfaces;

namespace TimeTracking.Web
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            Configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var sqlConnectionString = Configuration["Database:PostgreSqlConnection"];

            services.AddDbContext<PostGreSqlDbContext>(options =>
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("TimeTracking.Web")
                )
            );


            services.AddTransient<PasswordHasher>();
            services.AddScoped<PostGreSqlDbContext>();
            services.AddTransient<IPostGreSqlService, PostGreSqlService>();


            //sync config json files with config classes
            services.Configure<MailConfig>(Configuration.GetSection("Mail"));
            services.Configure<AdminConfig>(Configuration.GetSection("Admin"));

            // Add application services.
            services.AddTransient<IMailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<PasswordHasher>();
            services.AddTransient<IPostGreSqlService, PostGreSqlService>();
            services.AddDataProtection();
            services.AddTransient<ConfirmationToken>();
            services.AddTransient<FlashMessage>();


            services.AddSession();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Use this section if you want to leverage session.
            services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext.Session);
            services.AddScoped<IMessageProvider, SessionMessageProvider>();

            // Use this section if you want to leverage cookies.
            //services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext.Request.Cookies);
            //services.AddScoped(x => x.GetRequiredService<IHttpContextAccessor>().HttpContext.Response.Cookies);
            //services.AddScoped<IMessageProvider, CookieMessageProvider>();

            services.AddScoped<IMessageTypes>(x =>
            {
                return new MessageTypes(error: "danger");
            });

            services.AddScoped<IMessengerOptions, MessengerOptions>();
            services.AddScoped<IMessenger, StackMessenger>();

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }


            try
            {
                using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<PostGreSqlDbContext>();
                    serviceScope.ServiceProvider.GetService<PasswordHasher>();
                }
            }
            catch { }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            SeedData.CreateAdminAppUser(app.ApplicationServices, Configuration);
        }
    }
}