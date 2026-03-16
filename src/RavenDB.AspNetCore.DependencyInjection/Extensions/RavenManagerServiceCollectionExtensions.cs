using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;
using System.Linq;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to configure multi-cluster RavenDB services.
    /// Use these methods when you need to connect to multiple RavenDB clusters.
    /// The RavenManager maintains exactly one DocumentStore per cluster.
    /// For single-cluster scenarios, consider using AddRavenStore() for simpler configuration.
    /// </summary>
    public static class RavenManagerServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the RavenDB Manager with programmatic configuration for multiple clusters.
        /// Each cluster gets exactly one DocumentStore, and each store can connect to multiple URLs for high availability.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="options">Action to configure the manager options (clusters, default cluster, conventions, etc.).</param>
        /// <returns>A <see cref="RavenManagerBuilder"/> for registering sessions.</returns>
        public static RavenManagerBuilder AddRavenManager(
            this IServiceCollection services,
            Action<RavenManagerOptions> options)
        {
            return AddRavenManager<RavenManager, RavenManagerOptions>(services, options);
        }

        /// <summary>
        /// Registers the RavenDB Manager using configuration from appsettings.json or similar.
        /// The configuration section should match <see cref="RavenManagerOptions"/> structure.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The configuration section containing manager settings.</param>
        /// <returns>A <see cref="RavenManagerBuilder"/> for registering sessions.</returns>
        public static RavenManagerBuilder AddRavenManager(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RavenManagerOptions>(configuration);

            return AddRavenManager<RavenManager>(services);
        }

        /// <summary>
        /// Registers a custom RavenDB Manager implementation using configuration from appsettings.json or similar.
        /// Use this when you have a custom IRavenManager implementation with custom options.
        /// </summary>
        /// <typeparam name="TValue">Your custom manager implementation type (must implement <see cref="IRavenManager"/>).</typeparam>
        /// <typeparam name="TOptions">Your custom options type for configuring the manager.</typeparam>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The configuration section containing manager settings.</param>
        /// <returns>A <see cref="RavenManagerBuilder"/> for registering sessions.</returns>
        public static RavenManagerBuilder AddRavenManager<TValue, TOptions>(
            this IServiceCollection services,
            IConfiguration configuration)
            where TOptions : class
            where TValue : class, IRavenManager
        {
            services.Configure<TOptions>(configuration);

            return AddRavenManager<TValue>(services);
        }

        /// <summary>
        /// Registers a custom RavenDB Manager implementation with programmatic configuration.
        /// Use this when you have a custom IRavenManager implementation with custom options.
        /// </summary>
        /// <typeparam name="TValue">Your custom manager implementation type (must implement <see cref="IRavenManager"/>).</typeparam>
        /// <typeparam name="TOptions">Your custom options type for configuring the manager.</typeparam>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="options">Action to configure your custom options.</param>
        /// <returns>A <see cref="RavenManagerBuilder"/> for registering sessions.</returns>
        public static RavenManagerBuilder AddRavenManager<TValue, TOptions>(
               this IServiceCollection services,
               Action<TOptions> options = null)
                where TOptions : class
                where TValue : class, IRavenManager
        {
            if (options != null)
                services.Configure(options);

            return AddRavenManager<TValue>(services);
        }

        /// <summary>
        /// Registers a custom RavenDB Manager implementation.
        /// Use this when you have a custom IRavenManager implementation.
        /// </summary>
        /// <typeparam name="TValue">Your custom manager implementation type (must implement <see cref="IRavenManager"/>).</typeparam>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>A <see cref="RavenManagerBuilder"/> for registering sessions.</returns>
        public static RavenManagerBuilder AddRavenManager<TValue>(
            this IServiceCollection services)
            where TValue : class, IRavenManager
        {
            services.AddOptionsWithValidateOnStart<RavenManagerOptions>()
                .Validate(options =>
                {
                    if (options.DefaultCluster != null && options.Clusters?.ContainsKey(options.DefaultCluster) != true)
                        return false;

                    if (options.Clusters != null)
                    {
                        foreach (var cluster in options.Clusters)
                        {
                            if (cluster.Value?.Urls == null || cluster.Value.Urls.Length == 0 || cluster.Value.Urls.Any(u => string.IsNullOrWhiteSpace(u)))
                                return false;
                        }
                    }

                    return true;
                },
                "RavenManagerOptions validation failed. Ensure DefaultCluster (if set) exists in Clusters, " +
                "and each cluster has at least one non-empty URL.");

            services.AddSingleton<IRavenManager, TValue>();

            return new RavenManagerBuilder(services);
        }
    }
}
