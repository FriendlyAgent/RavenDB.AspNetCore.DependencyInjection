using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Default implementation of IRavenSessionFactory.
    /// Creates sessions on-demand using either a single DocumentStore or a RavenManager.
    /// </summary>
    public class RavenSessionFactory : IRavenSessionFactory
    {
        private readonly IDocumentStore _store;
        private readonly IRavenManager _manager;

        /// <summary>
        /// Initializes a new instance using a RavenManager for multi-cluster scenarios.
        /// </summary>
        /// <param name="manager">The RavenManager to use for session creation.</param>
        public RavenSessionFactory(IRavenManager manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        /// <summary>
        /// Initializes a new instance using a single DocumentStore.
        /// </summary>
        /// <param name="store">The DocumentStore to use for session creation.</param>
        public RavenSessionFactory(IDocumentStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Creates an asynchronous session using the default cluster and database.
        /// </summary>
        public IAsyncDocumentSession CreateAsyncSession()
        {
            if (_manager != null)
                return _manager.GetAsyncSession();

            return _store.OpenAsyncSession();
        }

        /// <summary>
        /// Creates an asynchronous session for the specified cluster.
        /// </summary>
        public IAsyncDocumentSession CreateAsyncSession(string clusterKey)
        {
            if (_manager != null)
                return _manager.GetAsyncSession(clusterKey);

            throw new InvalidOperationException(
                "Cluster-based session creation requires RavenManager. " +
                "Use AddRavenManager() instead of AddRavenStore() to enable multi-cluster support.");
        }

        /// <summary>
        /// Creates an asynchronous session for the specified cluster and database.
        /// </summary>
        public IAsyncDocumentSession CreateAsyncSession(string clusterKey, string database)
        {
            if (_manager != null)
                return _manager.GetAsyncSession(clusterKey, database);

            throw new InvalidOperationException(
                "Cluster-based session creation requires RavenManager. " +
                "Use AddRavenManager() instead of AddRavenStore() to enable multi-cluster support.");
        }

        /// <summary>
        /// Creates an asynchronous session with the specified session options.
        /// </summary>
        public IAsyncDocumentSession CreateAsyncSession(SessionOptions sessionOptions)
        {
            if (_manager != null)
                return _manager.GetAsyncSession(sessionOptions);

            return _store.OpenAsyncSession(sessionOptions);
        }

        /// <summary>
        /// Creates an asynchronous session for the specified cluster with session options.
        /// </summary>
        public IAsyncDocumentSession CreateAsyncSession(string clusterKey, SessionOptions sessionOptions)
        {
            if (_manager != null)
                return _manager.GetAsyncSession(clusterKey, sessionOptions);

            throw new InvalidOperationException(
                "Cluster-based session creation requires RavenManager. " +
                "Use AddRavenManager() instead of AddRavenStore() to enable multi-cluster support.");
        }

        /// <summary>
        /// Creates a synchronous session using the default cluster and database.
        /// </summary>
        public IDocumentSession CreateSession()
        {
            if (_manager != null)
                return _manager.GetSession();

            return _store.OpenSession();
        }

        /// <summary>
        /// Creates a synchronous session for the specified cluster.
        /// </summary>
        public IDocumentSession CreateSession(string clusterKey)
        {
            if (_manager != null)
                return _manager.GetSession(clusterKey);

            throw new InvalidOperationException(
                "Cluster-based session creation requires RavenManager. " +
                "Use AddRavenManager() instead of AddRavenStore() to enable multi-cluster support.");
        }

        /// <summary>
        /// Creates a synchronous session for the specified cluster and database.
        /// </summary>
        public IDocumentSession CreateSession(string clusterKey, string database)
        {
            if (_manager != null)
                return _manager.GetSession(clusterKey, database);

            throw new InvalidOperationException(
                "Cluster-based session creation requires RavenManager. " +
                "Use AddRavenManager() instead of AddRavenStore() to enable multi-cluster support.");
        }

        /// <summary>
        /// Creates a synchronous session with the specified session options.
        /// </summary>
        public IDocumentSession CreateSession(SessionOptions sessionOptions)
        {
            if (_manager != null)
                return _manager.GetSession(sessionOptions);

            return _store.OpenSession(sessionOptions);
        }

        /// <summary>
        /// Creates a synchronous session for the specified cluster with session options.
        /// </summary>
        public IDocumentSession CreateSession(string clusterKey, SessionOptions sessionOptions)
        {
            if (_manager != null)
                return _manager.GetSession(clusterKey, sessionOptions);

            throw new InvalidOperationException(
                "Cluster-based session creation requires RavenManager. " +
                "Use AddRavenManager() instead of AddRavenStore() to enable multi-cluster support.");
        }
    }
}
