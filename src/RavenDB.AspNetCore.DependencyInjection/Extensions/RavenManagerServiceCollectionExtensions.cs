using Microsoft.Extensions.DependencyInjection;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;
using Microsoft.Extensions.Configuration;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Contains extension methods for <see cref="IServiceCollection"/> to configure the Raven services.
    /// </summary>
    public static class RavenManagerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a default manager <see cref="RavenManager"/>.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        /// <param name="options">The options used to configure the Raven manager.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method created.</returns>
        public static RavenManagerBuilder AddRavenManager(
            this IServiceCollection services,
            Action<RavenManagerOptions> options)
        {
            return AddRavenManager<RavenManager, RavenManagerOptions>(services, options);
        }

        /// <summary>
        /// Adds and configures a default <see cref="RavenManager" />
        /// using options
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="options">The options used to configure the default Raven server.</param>
        public static RavenManagerBuilder AddRavenManagerWithDefaultServer(
            this IServiceCollection services,
            Action<RavenStoreOptions> options)
        {
            var serverOptions = new RavenStoreOptions();
            options?.Invoke(serverOptions);

            return AddRavenManager<RavenManager, RavenManagerOptions>(services, moptions =>
            {
                moptions.DefaultServer = "Default";
                moptions.AddServer("Default", serverOptions);
            });
        }

        /// <summary>
        /// Adds and configures a default <see cref="RavenManager" />
        /// using options
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">The configuration used to configure the default Raven server.</param>
        public static RavenManagerBuilder AddRavenManagerWithDefaultServer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var serverOptions = configuration.Get<RavenStoreOptions>();

            return AddRavenManager<RavenManager, RavenManagerOptions>(services, options =>
            {
                options.DefaultServer = "Default";
                options.AddServer("Default", serverOptions);
            });
        }

        /// <summary>
        /// Adds and configures a default <see cref="RavenManager" />
        /// using options from the configuration root (typically a config file) that will
        /// bind to <see cref="RavenManagerOptions" />
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration to map from</param>
        public static RavenManagerBuilder AddRavenManager(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<RavenManagerOptions>(configuration);

            return AddRavenManager<RavenManager>(services);
        }

        /// <summary>
        /// Adds and configures a default <see cref="RavenManager" />
        /// using options from the configuration root (typically a config file) that will
        /// bind to <see cref="RavenManagerOptions" />
        /// </summary>
        /// <typeparam name="TValue">The type of the specified manager <see cref="IRavenManager"/>.</typeparam>
        /// <typeparam name="TOptions">The type of options needed to configure the specified manager.</typeparam>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration to map from</param>
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
        /// Adds and configures the specified manager.
        /// </summary>
        /// <typeparam name="TValue">The type of the specified manager <see cref="IRavenManager"/>.</typeparam>
        /// <typeparam name="TOptions">The type of options needed to configure the specified manager.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="options">The options used to configure the manager.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method created.</returns>
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
        /// Adds the specified manager.
        /// </summary>
        /// <typeparam name="TValue">The type of the specified manager <see cref="IRavenManager"/>.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method created.</returns>
        public static RavenManagerBuilder AddRavenManager<TValue>(
            this IServiceCollection services)
            where TValue : class, IRavenManager
        {
            services.AddSingleton<IRavenManager, TValue>();

            return new RavenManagerBuilder(services);
        }
    }
}