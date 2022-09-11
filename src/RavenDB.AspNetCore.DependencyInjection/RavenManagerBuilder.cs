using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// The builder used to inject the RavenDB Manager.
    /// </summary>
    public class RavenManagerBuilder
    {
        /// <summary>
        /// The service used to inject everything.
        /// </summary>
        public IServiceCollection Services { get; private set; }

        /// <summary>
        /// Initializes a new instance of the RavenManagerBuilder class.
        /// <param name="services">The services available in the application.</param>
        /// </summary>
        public RavenManagerBuilder(
            IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Adds a asynchronous session that uses a Func to get the specifies server and database when requested.
        /// </summary>
        /// <param name="getConnection">The action used to get the connection class which in turn will be used to establish the session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedAsyncSession(
            Func<IServiceProvider,
            RavenConnection> getConnection)
        {
            if (getConnection == null)
                throw new ArgumentNullException(nameof(getConnection));

            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var connection = getConnection(provider);
                var manager = provider
                    .GetService<IRavenManager>();

                return manager
                    .GetAsyncSession(connection);
            });

            return this;
        }

        /// <summary>
        /// Adds a asynchronous session that uses the specified server and database.
        /// </summary>
        /// <param name="connection">The class containing all the information needed to find the correct server and establish the session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedAsyncSession(
            RavenConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetAsyncSession(connection);
            });

            return this;
        }

        /// <summary>
        /// Adds a asynchronous session that uses a specific server and it's default database.
        /// </summary>
        /// <param name="serverName">The name of the server which you want to use to establish the session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedAsyncSession(
            string serverName)
        {
            if (serverName == null)
                throw new ArgumentNullException(nameof(serverName));

            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetAsyncSession(
                    new RavenConnection(serverName));
            });

            return this;
        }

        /// <summary>
        /// Adds a asynchronous session that uses the default server and database.
        /// </summary>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedAsyncSession()
        {
            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetAsyncSession();
            });

            return this;
        }

        /// <summary>
        /// Adds a synchronous session that uses a Func to get the specifies server and database when requested.
        /// </summary>
        /// <param name="getConnection">The action used to get the connection class which in turn will be used to establish the session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedSession(
            Func<IServiceProvider, RavenConnection> getConnection)
        {
            if (getConnection == null)
                throw new ArgumentNullException(nameof(getConnection));

            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var connection = getConnection(provider);
                var manager = provider
                    .GetService<IRavenManager>();

                return manager
                    .GetSession(connection);
            });

            return this;
        }

        /// <summary>
        /// Adds a synchronous session that uses the specified server and database
        /// </summary>
        /// <param name="connection">The class containing all the information needed to find the correct server and establish the session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedSession(
            RavenConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager
                    .GetSession(connection);
            });

            return this;
        }

        /// <summary>
        /// Adds a synchronous session that uses a specific server and it's default database.
        /// </summary>
        /// <param name="serverName">The name of the server which you want to use to establish the session.</param>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedSession(
            string serverName)
        {
            if (serverName == null)
                throw new ArgumentNullException(nameof(serverName));

            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetSession(
                    new RavenConnection(serverName));
            });

            return this;
        }

        /// <summary>
        /// Adds a synchronous session that uses the default server and database.
        /// </summary>
        /// <returns>The <see cref="RavenManagerBuilder"/> this method is contained in.</returns>
        public RavenManagerBuilder AddScopedSession()
        {
            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetSession();
            });

            return this;
        }
    }
}