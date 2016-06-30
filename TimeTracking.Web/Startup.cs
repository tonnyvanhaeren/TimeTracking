using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MotleyFlash;
using MotleyFlash.AspNetCore.MessageProviders;
using System.IdentityModel.Tokens.Jwt;
using TimeTracking.DataAccess;
using TimeTracking.DataAccess.Interfaces;
using TimeTracking.General.Helpers;
using TimeTracking.Web.Config;
using TimeTracking.Web.Helpers;
using TimeTracking.Web.Services;
using TimeTracking.Web.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

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

            services.AddScoped<PostGreSqlDbContext>();

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

            services.AddScoped<IMessageTypes>(x =>
            {
                return new MessageTypes(error: "danger", information: "info");
            });

            services.AddScoped<IMessengerOptions, MessengerOptions>();
            services.AddScoped<IMessenger, StackMessenger>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdministratorOnly", policy => policy.RequireRole(General.Constants.AppUserPolicyRole.Admin));
                options.AddPolicy("EmployeeOnly", policy => policy.RequireRole(General.Constants.AppUserPolicyRole.Employee));
            });

            // Add framework services.
            services.AddMvc(config => { 
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(Microsoft.AspNetCore.Builder.IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
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

            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AuthenticationScheme = "cookies",
                AutomaticAuthenticate = true,
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions {
                AuthenticationScheme = "oidc",
                SignInScheme = "cookies",
                AutomaticChallenge = true,
                Authority = General.Constants.Idsrv.IdSrvUrl,
                RequireHttpsMetadata = false,
                ClientId = "mvc_implicit",
                ResponseType = "id_token token" , // OpenIdConnectResponseTypes.IdToken,  "id_token token" 
                TokenValidationParameters = {  NameClaimType = "name", RoleClaimType = "role" },
                Scope = { "email", "roles", "timeTrackingAPI" },
                SaveTokens = true,

                Events = new OpenIdConnectEvents()
                {
                    //when a principal is authenticated but has no access to resources (page) redirect to a forbidden page
                    OnRedirectToIdentityProvider = (context) => {
                        if (context.HttpContext.User.Identity.IsAuthenticated)
                        {
                            context.HandleResponse();
                            context.HttpContext.Response.Redirect(Uri.EscapeUriString(General.Constants.MvcClient.ClientForbiddenUrl));
                        }
                        return Task.FromResult(0);
                    },

                    
                    OnTicketReceived = context =>
                    {
                        // Get the ClaimsIdentity
                        var identity = context.Principal.Identity as ClaimsIdentity;
                        if (identity != null)
                        {
                            // Save the tokens as Claims. If you do not want to do this then set SaveTokens above to false, and also comment out this code
                            if (context.Properties.Items.ContainsKey(".TokenNames"))
                            {
                                string[] tokenNames = context.Properties.Items[".TokenNames"].Split(';');

                                foreach (string tokenName in tokenNames)
                                {
                                    string tokenValue = context.Properties.Items[$".Token.{tokenName}"];

                                    if (!identity.HasClaim(c => c.Type == tokenName))
                                        identity.AddClaim(new System.Security.Claims.Claim(tokenName, tokenValue));
                                }
                            }

                            // Add the Name ClaimType. This is required if we want User.Identity.Name to actually return something!
                            if (!context.Principal.HasClaim(c => c.Type == ClaimTypes.Name) &&
                                identity.HasClaim(c => c.Type == "name"))
                                identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Name, identity.FindFirst("name").Value));
                        }


                        return Task.FromResult(0);
                    },
                }
            });



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