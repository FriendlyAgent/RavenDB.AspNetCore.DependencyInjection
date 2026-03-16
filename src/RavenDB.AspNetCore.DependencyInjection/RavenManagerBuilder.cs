using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection.Exceptions;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Builder for configuring multi-cluster RavenDB dependency injection.
    /// The RavenManager maintains one DocumentStore per cluster and allows switching between clusters.
    /// Use this when you need to connect to multiple RavenDB clusters.
    /// For single-cluster scenarios, you can also use AddRavenStore() for simpler configuration.
    /// </summary>
    public class RavenManagerBuilder
    {
        /// <summary>
        /// The service collection to configure.
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RavenManagerBuilder class.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        public RavenManagerBuilder(
            IServiceCollection services)
        {
            Services = services;
        }

        #region Asynchronous Session Methods

        /// <summary>
        /// Registers a scoped asynchronous session that uses the default cluster and its default database.
        /// This is the simplest way to add a session when you have a single cluster or a configured default.
        /// </summary>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="UnknownClusterException">Thrown if no default cluster is configured.</exception>
        public RavenManagerBuilder AddScopedAsyncSession()
        {
            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetAsyncSession();
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped asynchronous session for a specific cluster using its default database.
        /// Use this when you need to connect to a specific cluster but want to use its configured default database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not configured.</exception>
        public RavenManagerBuilder AddScopedAsyncSession(string clusterKey)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetAsyncSession(clusterKey);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped asynchronous session for a specific cluster and database.
        /// Use this when you need full control over which cluster and database to connect to.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="database">The database name to use for this session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not configured.</exception>
        public RavenManagerBuilder AddScopedAsyncSession(
            string clusterKey,
            string database)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetAsyncSession(clusterKey, database);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped asynchronous session for the default cluster with RavenDB's native SessionOptions.
        /// Use this when you need advanced session configuration like NoTracking, NoCaching, or TransactionMode.
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration (NoTracking, NoCaching, TransactionMode, etc.).</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if sessionOptions is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if no default cluster is configured.</exception>
        public RavenManagerBuilder AddScopedAsyncSession(SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetAsyncSession(sessionOptions);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped asynchronous session for a specific cluster with RavenDB's native SessionOptions.
        /// This provides full control over both the cluster selection and session configuration.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration (NoTracking, NoCaching, TransactionMode, etc.).</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey or sessionOptions is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not configured.</exception>
        public RavenManagerBuilder AddScopedAsyncSession(
            string clusterKey,
            SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddScoped(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetAsyncSession(clusterKey, sessionOptions);
            });

            return this;
        }

        #endregion

        #region Synchronous Session Methods

        /// <summary>
        /// Registers a scoped synchronous session that uses the default cluster and its default database.
        /// This is the simplest way to add a session when you have a single cluster or a configured default.
        /// </summary>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="UnknownClusterException">Thrown if no default cluster is configured.</exception>
        public RavenManagerBuilder AddScopedSession()
        {
            Services.AddScoped<IDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetSession();
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped synchronous session for a specific cluster using its default database.
        /// Use this when you need to connect to a specific cluster but want to use its configured default database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not configured.</exception>
        public RavenManagerBuilder AddScopedSession(string clusterKey)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            Services.AddScoped<IDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetSession(clusterKey);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped synchronous session for a specific cluster and database.
        /// Use this when you need full control over which cluster and database to connect to.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="database">The database name to use for this session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not configured.</exception>
        public RavenManagerBuilder AddScopedSession(
            string clusterKey,
            string database)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            Services.AddScoped<IDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetSession(clusterKey, database);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped synchronous session for the default cluster with RavenDB's native SessionOptions.
        /// Use this when you need advanced session configuration like NoTracking, NoCaching, or TransactionMode.
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration (NoTracking, NoCaching, TransactionMode, etc.).</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if sessionOptions is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if no default cluster is configured.</exception>
        public RavenManagerBuilder AddScopedSession(SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddScoped<IDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetSession(sessionOptions);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped synchronous session for a specific cluster with RavenDB's native SessionOptions.
        /// This provides full control over both the cluster selection and session configuration.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration (NoTracking, NoCaching, TransactionMode, etc.).</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey or sessionOptions is null.</exception>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not configured.</exception>
        public RavenManagerBuilder AddScopedSession(
            string clusterKey,
            SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddScoped<IDocumentSession>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetSession(clusterKey, sessionOptions);
            });

            return this;
        }

        #endregion

        #region Keyed Services

        /// <summary>
        /// Registers a keyed asynchronous session.
        /// This allows multiple session registrations with different keys, clusters, and configurations.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="clusterKey">The cluster to use for this session. If null, uses default cluster.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddKeyedAsyncSession(string serviceKey, string clusterKey = null)
        {
            Services.AddKeyedScoped(serviceKey, (provider, key) =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return clusterKey == null
                    ? manager.GetAsyncSession()
                    : manager.GetAsyncSession(clusterKey);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed asynchronous session with session options.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="clusterKey">The cluster to use. If null, uses default cluster.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddKeyedAsyncSession(string serviceKey, string clusterKey, SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddKeyedScoped<IAsyncDocumentSession>(serviceKey, (provider, key) =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return clusterKey == null
                    ? manager.GetAsyncSession(sessionOptions)
                    : manager.GetAsyncSession(clusterKey, sessionOptions);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed asynchronous session for a specific cluster and database.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="clusterKey">The cluster to use.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddKeyedAsyncSession(string serviceKey, string clusterKey, string database)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);
            ArgumentNullException.ThrowIfNull(database);

            Services.AddKeyedScoped<IAsyncDocumentSession>(serviceKey, (provider, key) =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetAsyncSession(clusterKey, database);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed synchronous session.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="clusterKey">The cluster to use. If null, uses default cluster.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddKeyedSession(string serviceKey, string clusterKey = null)
        {
            Services.AddKeyedScoped(serviceKey, (provider, key) =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return clusterKey == null
                    ? manager.GetSession()
                    : manager.GetSession(clusterKey);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed synchronous session with session options.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="clusterKey">The cluster to use. If null, uses default cluster.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddKeyedSession(string serviceKey, string clusterKey, SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddKeyedScoped(serviceKey, (provider, key) =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return clusterKey == null
                    ? manager.GetSession(sessionOptions)
                    : manager.GetSession(clusterKey, sessionOptions);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed synchronous session for a specific cluster and database.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="clusterKey">The cluster to use.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddKeyedSession(string serviceKey, string clusterKey, string database)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);
            ArgumentNullException.ThrowIfNull(database);

            Services.AddKeyedScoped<IDocumentSession>(serviceKey, (provider, key) =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return manager.GetSession(clusterKey, database);
            });

            return this;
        }

        #endregion

        #region Session Factory

        /// <summary>
        /// Registers the RavenSessionFactory as a scoped service.
        /// Use this when you need to create multiple sessions with different configurations within the same scope.
        /// </summary>
        /// <returns>The <see cref="RavenManagerBuilder"/> for method chaining.</returns>
        public RavenManagerBuilder AddSessionFactory()
        {
            Services.AddScoped<IRavenSessionFactory>(provider =>
            {
                var manager = provider.GetRequiredService<IRavenManager>();
                return new RavenSessionFactory(manager);
            });

            return this;
        }

        #endregion
    }
}
