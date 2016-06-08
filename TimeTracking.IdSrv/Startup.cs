using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MotleyFlash;
using MotleyFlash.AspNetCore.MessageProviders;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using TimeTracking.DataAccess;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Helpers;
using TimeTracking.IdSrv.configuration;
using TimeTracking.IdSrv.Configuration;
using TimeTracking.IdSrv.Database;
using TimeTracking.IdSrv.Extensions;
using TimeTracking.IdSrv.Helpers;
using TimeTracking.IdSrv.Services.Interfaces;

namespace TimeTracking.IdSrv
{
    public class Startup
    {
        private readonly IHostingEnvironment _environment;

        public IConfigurationRoot configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            _environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var cert = new X509Certificate2(Path.Combine(_environment.ContentRootPath, "idsrv3test.pfx"), "idsrv3test");

            var sqlConnectionString = configuration["Database:PostgreSqlConnection"];

            services.AddDbContext<PostGreSqlDbContext>(options =>
                options.UseNpgsql(
                    sqlConnectionString,
                    b => b.MigrationsAssembly("TimeTracking.IdSrv")
                )
            );

            services.AddScoped<PostGreSqlDbContext>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var builder = services.AddIdentityServer(options =>
            {
                options.SigningCertificate = cert;
                options.SiteName = "Time Tracking Identity Server";
                
            });

            builder.AddInMemoryClients(Clients.Get());
            builder.AddInMemoryScopes(Scopes.Get());
            builder.AddInMemoryUsers(Users.Get());

            builder.AddCustomGrantValidator<CustomGrantValidator>();


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




            // for the UI
            services
                .AddMvc()
                .AddRazorOptions(razor =>
                {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander());
                });
            services.AddTransient<UI.Login.LoginService>();

            //sync config json files with config classes
            services.Configure<MailConfig>(configuration.GetSection("Mail"));
            services.Configure<AdminConfig>(configuration.GetSection("Admin"));

            // Add application services.
            services.AddTransient<IMailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<PasswordHasher>();
            services.AddTransient<IPostGreSqlService, PostGreSqlService>();
            services.AddDataProtection();
            services.AddTransient<ConfirmationToken>();
            services.AddTransient<FlashMessage>();
 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseRuntimeInfoPage("/info");
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Temp",
                AutomaticAuthenticate = false,
                AutomaticChallenge = false
            });

            app.UseIdentityServer();

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvcWithDefaultRoute();

            SeedData.CreateAdminUser(app.ApplicationServices, configuration);
        }
    }
}
