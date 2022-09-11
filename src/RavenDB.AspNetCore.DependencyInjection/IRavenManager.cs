using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection.Options;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Represents an interface used to implement a class responsible for the management of raven servers and stores.
    /// </summary>
    public interface IRavenManager
    {
        /// <summary>
        /// Get the store from the specified server.
        /// </summary>
        /// <param name="serverName">The name of the server to get.</param>
        /// <returns>The store from the specified server.</returns>
        IDocumentStore GetStore(
            string serverName);

        /// <summary>
        /// Get a store from the default server.
        /// </summary>
        /// <returns>The store from the default server.</returns>
        IDocumentStore GetStore();

        /// <summary>
        /// Gets a asynchronous session that uses the specified server and database.
        /// </summary>
        /// <param name="connection">The class containing all the information needed to find the correct server and establish the session.</param>
        /// <returns>The specified asynchronous Session.</returns>
        IAsyncDocumentSession GetAsyncSession(
            RavenConnection connection);

        /// <summary>
        /// Gets a asynchronous session that uses the specified server.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <returns>A asynchronous session from the specified server.</returns>
        IAsyncDocumentSession GetAsyncSession(
            string serverName);

        /// <summary>
        /// Gets a asynchronous session that uses the default server and database.
        /// </summary>
        /// <returns>a asynchronous session for the default server and database.</returns>
        IAsyncDocumentSession GetAsyncSession();
        /// <summary>
        /// Gets a asynchronous session that uses the specified server and database.
        /// </summary>
        /// <param name="connection">The class containing all the information needed to find the correct server and establish the session.</param>
        /// <returns>The specified asynchronous Session.</returns>
        IDocumentSession GetSession(
            RavenConnection connection);

        /// <summary>
        /// Gets a asynchronous session that uses the specified server.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <returns>A asynchronous session from the specified server.</returns>
        IDocumentSession GetSession(
            string serverName);

        ///<summary>
        ///Gets a session that uses the default server and database.
        ///</summary>
        /// <returns>A session for the default server and database.</returns>
        IDocumentSession GetSession();

        /// <summary>
        /// Add a server to the raven manager.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <param name="serverOptions">The options for the server.</param>
        /// <returns>a bool which is true if the server was successfully added.</returns>
        bool AddServer(
            string serverName,
            RavenStoreOptions serverOptions);

        /// <summary>
        /// Remove a server from the raven manager.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <returns>a bool which is true if the server was successfully removed.</returns>
        bool RemoveServer(
            string serverName);
    }
}
