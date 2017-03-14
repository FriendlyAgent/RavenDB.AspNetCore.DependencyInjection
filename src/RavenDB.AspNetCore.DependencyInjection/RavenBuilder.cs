using Microsoft.Extensions.DependencyInjection;
using Raven.Client.Documents.Session;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public class RavenBuilder
    {
        public IServiceCollection Services { get; private set; }

        public RavenBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public RavenBuilder AddAsyncSession()
        {
            Services.AddScoped<IAsyncDocumentSession, IAsyncDocumentSession>(provider =>
            {
                var manager = provider.GetService<IRavenManager>();
                return manager.GetDefaultStore().OpenAsyncSession();
            });

            return this;
        }

        public RavenBuilder AddSession()
        {
            Services.AddScoped<IDocumentSession, IDocumentSession>(provider =>
            {
                var manager = provider.GetService<IRavenManager>();
                return manager.GetDefaultStore().OpenSession();
            });

            return this;
        }
    }
}