# Sample Applications

Shows usage of ServiceFabric with Reverse Proxy.

## Hint ASPNET.Core with ReverseProxy

ÝÝÝ C#
// rewrite path base if request is from reverse proxy like 19081 port (temporary)
            var rewriteOptions = new RewriteOptions()
               .Add(rewriteContext =>
               {
                   var request = rewriteContext.HttpContext.Request;
                   if (request.Headers.ContainsKey("X-Forwarded-Host"))
                   {
                       try
                       {
                           var fabricServiceName = Environment.GetEnvironmentVariable("Fabric_ServiceName"); // ex) fabric:/AppName/ServiceName
                           var fabricServiceUri = new Uri(fabricServiceName);
                           var servicePathBase = fabricServiceUri.AbsolutePath; // ex) /AppName/ServiceName
                           request.PathBase = servicePathBase;
                       }
                       catch (Exception)
                       {
                       }
                   }
               });
            app.UseRewriter(rewriteOptions);
ÝÝÝ
