using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using System.Fabric;
using WebAadUi.Configuration;

namespace WebAadUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder();
            builder
                .AddServiceFabricConfig("Config")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddAzureAd(options => Configuration.Bind("AzureAd", options))
            .AddCookie(options => 
            {
                options.CookieDomain = "rpcluster.m009.de";
                options.Cookie.Domain = "rpcluster.m009.de";
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddRazorPagesOptions(options =>
            {
                options.Conventions.AllowAnonymousToFolder("/Account");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            // Needed settings for hosting the application behind a reverse proxy
            var fhoptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            };

            fhoptions.KnownNetworks.Clear();
            fhoptions.KnownProxies.Clear();
            app.UseForwardedHeaders(fhoptions);

            //Handle X-Forwarded-PathBase and set the ambient context property. After this, '~' will resolve to the original pathbase from the gateway.
            app.Use((context, next) =>
            {
                var sfcontext = app.ApplicationServices.GetRequiredService<StatelessServiceContext>();
                var serviceName = sfcontext.ServiceName.ToString().Replace("fabric:/", "");
                var path = $"/{serviceName}";
                context.Request.PathBase = new PathString(path);
                return next();
            });

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
