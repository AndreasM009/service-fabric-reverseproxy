﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Fabric;

namespace EchoApi
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
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                var sfcontext = app.ApplicationServices.GetRequiredService<StatelessServiceContext>();
                var serviceName = sfcontext.ServiceName.ToString().Replace("fabric:/", "");
                var path = $"/{serviceName}";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.BasePath = path);
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                var sfcontext = app.ApplicationServices.GetRequiredService<StatelessServiceContext>();
                var serviceName = sfcontext.ServiceName.ToString().Replace("fabric:/", "");
                var path = $"/{serviceName}/swagger/v1/swagger.json";
                c.SwaggerEndpoint(path, "My API V1");
            });
            
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
