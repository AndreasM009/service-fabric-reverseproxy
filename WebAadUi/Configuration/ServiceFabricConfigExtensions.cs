using Microsoft.Extensions.Configuration;

namespace WebAadUi.Configuration
{
    public static class ServiceFabricConfigExtensions
    {
        public static IConfigurationBuilder AddServiceFabricConfig(this IConfigurationBuilder builder, string packageName)
        {
            return builder.Add(new ServiceFabricConfigSource(packageName));
        }
    }
}