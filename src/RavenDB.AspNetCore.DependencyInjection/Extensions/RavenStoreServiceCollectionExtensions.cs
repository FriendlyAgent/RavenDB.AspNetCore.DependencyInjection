#if NETSTANDARD_2_1 || NETCOREAPP_3_1
using Microsoft.Extensions.Hosting;
#else
    using Microsoft.AspNetCore.Hosting;
#endif
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RavenDB.AspNetCore.DependencyInjection.Helpers;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Contains extension methods for <see cref="IServiceCollection"/> to configure the Raven services.
    /// </summary>
    public static class RavenStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a RavenDB store <see cref="RavenStoreBuilder"/>.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="options">The options used to configure the RavenDB store.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> this method created.</returns>
        public static RavenStoreBuilder AddRavenStore(
            this IServiceCollection services,
            Action<RavenStoreOptions> options)
        {
            if (options != null)
                services.Configure(options);

            return AddRavenStore(services);
        }

        public static RavenStoreBuilder AddRavenStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RavenStoreOptions>(configuration);
            return AddRavenStore(services);
        }

        public static RavenStoreBuilder AddRavenStore(
            this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var options = provider
                    .GetService<IOptions<RavenStoreOptions>>()?.Value;

#if NETSTANDARD_2_1 || NETCOREAPP_3_1
                var host = provider
                    .GetService<IHostEnvironment>();
#else
                var host = provider
                    .GetService<IHostingEnvironment>();
#endif

                var store = RavenHelpers.CreateDocumentStore(host, options);
                store.Initialize();

                return store;
            });

            return new RavenStoreBuilder(services);
        }
    }
}