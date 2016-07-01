using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace TimeTracking.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add Cors support to the service
            services.AddCors();

            var corspolicy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

            corspolicy.Headers.Add("*");
            corspolicy.Methods.Add("*");
            corspolicy.Origins.Add("*");
            corspolicy.SupportsCredentials = true;

            services.AddCors(x => x.AddPolicy("corsGlobalPolicy", corspolicy));

            var guestPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("scope", General.Constants.Idsrv.ScopeTimeTrackingRecords)
                .Build();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("OnlyTimeTrackingAdmin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", General.Constants.AppUserPolicyRole.TimeTrackingAdmin);
                });
                options.AddPolicy("OnlyTimeTrackingEmployee", policyUser =>
                {
                    policyUser.RequireClaim("role", General.Constants.AppUserPolicyRole.TimeTrackingEmployee);
                });

            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AuthorizeFilter(guestPolicy));
            });

            //services.AddScoped<IDataEventRecordRepository, DataEventRecordRepository>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            var jwtBearerOptions = new JwtBearerOptions()
            {
                //Authority = "xxxxx://localhost:44345",
                Authority = General.Constants.Idsrv.IdSrvUrl,
                //Audience = "xxxxx://localhost:44345/resources",
                Audience = General.Constants.Idsrv.IdSrvEndResourcesEndPoint,
                AutomaticAuthenticate = true,

                // required if you want to return a 403 and not a 401 for forbidden responses
                AutomaticChallenge = true
            };

            app.UseJwtBearerAuthentication(jwtBearerOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
