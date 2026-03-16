using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System.Collections.Generic;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Represents an interface used to implement a class responsible for the management of RavenDB clusters and document stores.
    /// Each cluster maintains a single document store connected to multiple URLs for high availability.
    /// </summary>
    public interface IRavenManager
    {
        /// <summary>
        /// Gets the default cluster key used when no specific cluster is specified.
        /// </summary>
        string DefaultCluster { get; }

        /// <summary>
        /// Get the document store for the specified cluster.
        /// Each cluster has exactly one store that manages connections to all cluster nodes.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to get.</param>
        /// <returns>The document store for the specified cluster.</returns>
        IDocumentStore GetStore(string clusterKey);

        /// <summary>
        /// Get the document store for the default cluster.
        /// </summary>
        /// <returns>The document store for the default cluster.</returns>
        IDocumentStore GetStore();

        #region Asynchronous Session Methods

        /// <summary>
        /// Opens an asynchronous session using the default cluster and its default database.
        /// </summary>
        /// <returns>An asynchronous document session.</returns>
        IAsyncDocumentSession GetAsyncSession();

        /// <summary>
        /// Opens an asynchronous session for the specified cluster using its default database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <returns>An asynchronous document session.</returns>
        IAsyncDocumentSession GetAsyncSession(string clusterKey);

        /// <summary>
        /// Opens an asynchronous session for the specified cluster and database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>An asynchronous document session.</returns>
        IAsyncDocumentSession GetAsyncSession(string clusterKey, string database);

        /// <summary>
        /// Opens an asynchronous session for the default cluster with the specified session options.
        /// Provides full access to RavenDB's SessionOptions (NoTracking, NoCaching, TransactionMode, etc.).
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration.</param>
        /// <returns>An asynchronous document session.</returns>
        IAsyncDocumentSession GetAsyncSession(SessionOptions sessionOptions);

        /// <summary>
        /// Opens an asynchronous session for the specified cluster with the specified session options.
        /// Provides full access to RavenDB's SessionOptions (NoTracking, NoCaching, TransactionMode, etc.).
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration.</param>
        /// <returns>An asynchronous document session.</returns>
        IAsyncDocumentSession GetAsyncSession(string clusterKey, SessionOptions sessionOptions);

        #endregion

        #region Synchronous Session Methods

        /// <summary>
        /// Opens a synchronous session using the default cluster and its default database.
        /// </summary>
        /// <returns>A synchronous document session.</returns>
        IDocumentSession GetSession();

        /// <summary>
        /// Opens a synchronous session for the specified cluster using its default database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <returns>A synchronous document session.</returns>
        IDocumentSession GetSession(string clusterKey);

        /// <summary>
        /// Opens a synchronous session for the specified cluster and database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>A synchronous document session.</returns>
        IDocumentSession GetSession(string clusterKey, string database);

        /// <summary>
        /// Opens a synchronous session for the default cluster with the specified session options.
        /// Provides full access to RavenDB's SessionOptions (NoTracking, NoCaching, TransactionMode, etc.).
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration.</param>
        /// <returns>A synchronous document session.</returns>
        IDocumentSession GetSession(SessionOptions sessionOptions);

        /// <summary>
        /// Opens a synchronous session for the specified cluster with the specified session options.
        /// Provides full access to RavenDB's SessionOptions (NoTracking, NoCaching, TransactionMode, etc.).
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for advanced configuration.</param>
        /// <returns>A synchronous document session.</returns>
        IDocumentSession GetSession(string clusterKey, SessionOptions sessionOptions);

        #endregion

        /// <summary>
        /// Adds a new cluster configuration to the manager.
        /// Creates a new document store for the cluster on first access.
        /// </summary>
        /// <param name="clusterKey">The unique key to identify this cluster.</param>
        /// <param name="storeOptions">The configuration options for the cluster (URLs, database, conventions, etc.).</param>
        /// <returns>True if the cluster was successfully added; false if a cluster with this key already exists.</returns>
        bool AddCluster(string clusterKey, RavenStoreOptions storeOptions);

        /// <summary>
        /// Removes a cluster and disposes its document store.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to remove.</param>
        /// <returns>True if the cluster was successfully removed; false if the cluster was not found.</returns>
        bool RemoveCluster(string clusterKey);

        /// <summary>
        /// Gets all configured cluster keys.
        /// </summary>
        /// <returns>A collection of all cluster keys.</returns>
        IEnumerable<string> GetClusterKeys();

        /// <summary>
        /// Checks if a cluster with the specified key exists.
        /// </summary>
        /// <param name="clusterKey">The cluster key to check.</param>
        /// <returns>True if the cluster exists; otherwise false.</returns>
        bool HasCluster(string clusterKey);

        /// <summary>
        /// Checks if a document store has been created for the specified cluster.
        /// </summary>
        /// <param name="clusterKey">The cluster key to check.</param>
        /// <returns>True if the store has been initialized; otherwise false.</returns>
        bool IsStoreInitialized(string clusterKey);
    }
}
