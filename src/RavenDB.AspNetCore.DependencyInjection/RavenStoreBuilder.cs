using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// A class responsible the the injection of sessions
    /// </summary>
    public class RavenStoreBuilder
    {
        /// <summary>
        /// A class responsible the the injection of sessions
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RavenStoreBuilder class.
        /// <param name="services">The services available in the application.</param>
        /// </summary>
        public RavenStoreBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Adds a synchronous session that uses a specific database.
        /// </summary>
        /// <param name="database">The name of the database which you want to use to establish the session.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> this method is contained in.</returns>
        public RavenStoreBuilder AddScopedSession(
            string database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var store = provider
                      .GetService<IDocumentStore>();

                return store.OpenSession(database);
            });

            return this;
        }

        /// <summary>
        /// Adds a synchronous session that uses the default database.
        /// </summary>
        /// <returns>The <see cref="RavenStoreBuilder"/> this method is contained in.</returns>
        public RavenStoreBuilder AddScopedSession()
        {
            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var store = provider
                    .GetService<IDocumentStore>();

                return store.OpenSession();
            });

            return this;
        }

        /// <summary>
        /// Adds a asynchronous  session that uses the default database.
        /// </summary>
        /// <param name="database">The name of the database which you want to use to establish the session.</param>
        /// <returns>The <see cref="RavenStoreBuilder"/> this method is contained in.</returns>
        public RavenStoreBuilder AddScopedAsyncSession(
            string database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetAsyncSession(
                    new RavenConnection(database));
            });

            return this;
        }

        /// <summary>
        /// Adds a asynchronous session that uses the default database.
        /// </summary>
        /// <returns>The <see cref="RavenStoreBuilder"/> this method is contained in.</returns>
        public RavenStoreBuilder AddScopedAsyncSession()
        {
            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var store = provider
                    .GetService<IDocumentStore>();

                return store.OpenAsyncSession();
            });

            return this;
        }
    }
}