using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Fabric;

namespace WebUi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
