using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Builder for configuring simple single-store dependency injection.
    /// Use this when you only need to connect to a single RavenDB cluster.
    /// For multi-cluster scenarios, use RavenManagerBuilder instead.
    /// </summary>
    public class RavenStoreBuilder
    {
        /// <summary>
        /// The service collection to configure.
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RavenStoreBuilder class.
        /// </summary>
        /// <param name="services">The services available in the application.</param>
        public RavenStoreBuilder(
            IServiceCollection services)
        {
            Services = services;
        }

        #region Asynchronous Session Methods

        /// <summary>
        /// Registers a scoped asynchronous session that uses the default database.
        /// This is the simplest way to add a session when you have configured a default database.
        /// </summary>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddScopedAsyncSession()
        {
            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession();
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped asynchronous session for a specific database.
        /// Use this when you need to connect to a different database than the default.
        /// </summary>
        /// <param name="database">The database name to use for this session.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if database is null.</exception>
        public RavenStoreBuilder AddScopedAsyncSession(string database)
        {
            ArgumentNullException.ThrowIfNull(database);

            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession(database);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped asynchronous session with RavenDB's native SessionOptions.
        /// Use this when you need advanced session configuration like NoTracking, NoCaching, or TransactionMode.
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration (NoTracking, NoCaching, TransactionMode, etc.).</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if sessionOptions is null.</exception>
        public RavenStoreBuilder AddScopedAsyncSession(SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddScoped<IAsyncDocumentSession>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession(sessionOptions);
            });

            return this;
        }

        #endregion

        #region Synchronous Session Methods

        /// <summary>
        /// Registers a scoped synchronous session that uses the default database.
        /// This is the simplest way to add a session when you have configured a default database.
        /// </summary>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddScopedSession()
        {
            Services.AddScoped<IDocumentSession>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenSession();
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped synchronous session for a specific database.
        /// Use this when you need to connect to a different database than the default.
        /// </summary>
        /// <param name="database">The database name to use for this session.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if database is null.</exception>
        public RavenStoreBuilder AddScopedSession(string database)
        {
            ArgumentNullException.ThrowIfNull(database);

            Services.AddScoped<IDocumentSession>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenSession(database);
            });

            return this;
        }

        /// <summary>
        /// Registers a scoped synchronous session with RavenDB's native SessionOptions.
        /// Use this when you need advanced session configuration like NoTracking, NoCaching, or TransactionMode.
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration (NoTracking, NoCaching, TransactionMode, etc.).</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        /// <exception cref="ArgumentNullException">Thrown if sessionOptions is null.</exception>
        public RavenStoreBuilder AddScopedSession(SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddScoped<IDocumentSession>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenSession(sessionOptions);
            });

            return this;
        }

        #endregion

        #region Keyed Services

        /// <summary>
        /// Registers a keyed asynchronous session.
        /// This allows multiple session registrations with different keys and configurations.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddKeyedAsyncSession(string serviceKey)
        {
            Services.AddKeyedScoped<IAsyncDocumentSession>(serviceKey, (provider, key) =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession();
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed asynchronous session with session options.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddKeyedAsyncSession(string serviceKey, SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddKeyedScoped<IAsyncDocumentSession>(serviceKey, (provider, key) =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession(sessionOptions);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed asynchronous session for a specific database.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddKeyedAsyncSession(string serviceKey, string database)
        {
            ArgumentNullException.ThrowIfNull(database);

            Services.AddKeyedScoped<IAsyncDocumentSession>(serviceKey, (provider, key) =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenAsyncSession(database);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed synchronous session.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddKeyedSession(string serviceKey)
        {
            Services.AddKeyedScoped<IDocumentSession>(serviceKey, (provider, key) =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenSession();
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed synchronous session with session options.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddKeyedSession(string serviceKey, SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(sessionOptions);

            Services.AddKeyedScoped<IDocumentSession>(serviceKey, (provider, key) =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenSession(sessionOptions);
            });

            return this;
        }

        /// <summary>
        /// Registers a keyed synchronous session for a specific database.
        /// </summary>
        /// <param name="serviceKey">The key to identify this session registration.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddKeyedSession(string serviceKey, string database)
        {
            ArgumentNullException.ThrowIfNull(database);

            Services.AddKeyedScoped<IDocumentSession>(serviceKey, (provider, key) =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return store.OpenSession(database);
            });

            return this;
        }

        #endregion

        #region Session Factory

        /// <summary>
        /// Registers the RavenSessionFactory as a scoped service.
        /// Use this when you need to create multiple sessions with different configurations within the same scope.
        /// </summary>
        /// <returns>The <see cref="RavenStoreBuilder"/> for method chaining.</returns>
        public RavenStoreBuilder AddSessionFactory()
        {
            Services.AddScoped<IRavenSessionFactory>(provider =>
            {
                var store = provider.GetRequiredService<IDocumentStore>();
                return new RavenSessionFactory(store);
            });

            return this;
        }

        #endregion
    }
}
