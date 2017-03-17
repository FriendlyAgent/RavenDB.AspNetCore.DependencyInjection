using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public class RavenBuilder
    {
        public IServiceCollection Services { get; private set; }

        public RavenBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public RavenBuilder AddAsyncSession(
            Func<IServiceProvider, RavenConnection> getConnection)
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

        public RavenBuilder AddAsyncSession(
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

        public RavenBuilder AddAsyncSession(
            string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetAsyncSession(
                    new RavenConnection(name));
            });

            return this;
        }

        public RavenBuilder AddAsyncSession()
        {
            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetAsyncSession();
            });

            return this;
        }

        public RavenBuilder AddSession(
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

        public RavenBuilder AddSession(
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

        public RavenBuilder AddSession(
            string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var manager = provider
                    .GetService<IRavenManager>();

                return manager.GetSession(
                    new RavenConnection(name));
            });

            return this;
        }

        public RavenBuilder AddSession()
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