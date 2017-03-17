using Microsoft.Extensions.Configuration;
using Sparrow.Collections.LockFree;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public static class IConfigurationRootExtensions
    {
        public static RavenManagerOptions GetRavenSettings(
            this IConfigurationRoot configuration)
        {
            return configuration?
                .GetSection("RavenManagerOptions")?
                .Get<RavenManagerOptions>();
        }

        public static string GetRavenSettingDefaultServer(
            this IConfigurationRoot configuration)
        {
            return configuration?
                .GetSection("RavenManagerOptions:DefaultServer")?
                .Get<string>();
        }

        public static ConcurrentDictionary<string, RavenServerOptions> GetRavenSettingServers(
            this IConfigurationRoot configuration)
        {
            return configuration?
                .GetSection("RavenManagerOptions:Servers")?
                .Get<ConcurrentDictionary<string, RavenServerOptions>>();
        }
    }
}