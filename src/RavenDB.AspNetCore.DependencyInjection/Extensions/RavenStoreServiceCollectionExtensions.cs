using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using RavenDB.AspNetCore.DependencyInjection.Helpers;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;
using System.Linq;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to configure single-cluster RavenDB services.
    /// Use these methods when you only need to connect to one RavenDB cluster.
    /// For multi-cluster scenarios, use the RavenManager extensions instead.
    /// </summary>
    public static class RavenStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a single RavenDB DocumentStore with programmatic configuration.
        /// This is the simplest way to configure RavenDB for a single cluster.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="options">Action to configure the store options (URLs, database, certificates, etc.).</param>
        /// <returns>A <see cref="RavenStoreBuilder"/> for registering sessions.</returns>
        public static RavenStoreBuilder AddRavenStore(
            this IServiceCollection services,
            Action<RavenStoreOptions> options)
        {
            if (options != null)
                services.Configure(options);

            return AddRavenStore(services);
        }

        /// <summary>
        /// Registers a single RavenDB DocumentStore using configuration from appsettings.json or similar.
        /// The configuration section should match <see cref="RavenStoreOptions"/> structure.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The configuration section containing store settings.</param>
        /// <returns>A <see cref="RavenStoreBuilder"/> for registering sessions.</returns>
        public static RavenStoreBuilder AddRavenStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RavenStoreOptions>(configuration);
            return AddRavenStore(services);
        }

        /// <summary>
        /// Registers a single RavenDB DocumentStore.
        /// Use this overload when you've already configured options elsewhere or want to configure them later.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>A <see cref="RavenStoreBuilder"/> for registering sessions.</returns>
        public static RavenStoreBuilder AddRavenStore(
            this IServiceCollection services)
        {
            services.AddOptionsWithValidateOnStart<RavenStoreOptions>()
                .Validate(options =>
                    options.Urls != null && options.Urls.Length > 0 && options.Urls.All(u => !string.IsNullOrWhiteSpace(u)),
                    "RavenStoreOptions.Urls must contain at least one non-empty URL.");

            services.AddSingleton<IDocumentStore>(provider =>
            {
                var options = provider
                    .GetRequiredService<IOptions<RavenStoreOptions>>().Value;

                var store = RavenHelpers.CreateDocumentStore(options);
                try
                {
                    store.Initialize();
                    return store;
                }
                catch
                {
                    store.Dispose();
                    throw;
                }
            });

            return new RavenStoreBuilder(services);
        }
    }
}
