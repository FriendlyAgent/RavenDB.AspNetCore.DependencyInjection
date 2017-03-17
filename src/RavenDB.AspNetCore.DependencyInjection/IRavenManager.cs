using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public interface IRavenManager
    {
        IDocumentStore GetStore(
            string server);

        IDocumentStore GetStore();

        IAsyncDocumentSession GetAsyncSession(
            RavenConnection connection);

        IAsyncDocumentSession GetAsyncSession();

        IDocumentSession GetSession(
            RavenConnection connection);

        IDocumentSession GetSession();

        bool AddServer(
            string url,
            RavenServerOptions server);

        bool RemoveServer(
            string url);
    }
}
