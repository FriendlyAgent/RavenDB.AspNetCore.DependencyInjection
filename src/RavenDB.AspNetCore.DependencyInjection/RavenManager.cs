using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection.Exceptions;
using RavenDB.AspNetCore.DependencyInjection.Helpers;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Represents a class responsible for managing RavenDB clusters, sessions, and stores.
    /// </summary>
    public class RavenManager
         : IRavenManager, IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The cluster key used when no specific cluster is specified.
        /// </summary>
        public string DefaultCluster { get; private set; }

        /// <summary>
        /// The conventions used when no specific conventions are specified.
        /// </summary>
        public DocumentConventions DefaultConventions { get; private set; }

        /// <summary>
        /// The default certificate options used when no specific certificate is specified.
        /// </summary>
        public RavenCertificateOptions DefaultCertificateOptions { get; private set; }

        private volatile bool _disposed;
        private readonly ConcurrentDictionary<string, Lazy<IDocumentStore>> _stores;
        private readonly ConcurrentDictionary<string, RavenStoreOptions> _clusters;

        /// <summary>
        /// Initializes a new instance of the RavenManager class with specified options.
        /// </summary>
        /// <param name="options">The options used to configure the RavenManager.</param>
        /// <exception cref="ArgumentNullException">Thrown if options is null.</exception>
        public RavenManager(
            IOptions<RavenManagerOptions> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            _disposed = false;
            DefaultCluster = options.Value.DefaultCluster;
            DefaultConventions = options.Value.DefaultConventions ?? new DocumentConventions();
            DefaultCertificateOptions = options.Value.DefaultCertificateOptions;

            _stores = new ConcurrentDictionary<string, Lazy<IDocumentStore>>();
            _clusters = options.Value.Clusters;
        }

        #region Store Methods

        /// <summary>
        /// Get a store from the default cluster.
        /// </summary>
        /// <returns>The store from the default cluster.</returns>
        /// <exception cref="UnknownClusterException">Thrown if no default cluster is configured.</exception>
        public IDocumentStore GetStore()
        {
            if (DefaultCluster == null)
            {
                var availableClusters = _clusters?.Keys.Any() == true
                    ? string.Join(", ", _clusters.Keys)
                    : "none";
                throw new UnknownClusterException(
                    $"No default cluster has been configured. Available clusters: {availableClusters}. " +
                    $"Set 'DefaultCluster' in RavenManagerOptions or use GetStore(clusterKey) with an explicit cluster key.");
            }

            return GetStore(DefaultCluster);
        }

        /// <summary>
        /// Get the store from the specified cluster.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to get.</param>
        /// <returns>The store from the specified cluster.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        public IDocumentStore GetStore(
            string clusterKey)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(clusterKey);

            return _stores.GetOrAdd(clusterKey, CreateDocumentStore(clusterKey)).Value;
        }

        #endregion

        #region Asynchronous Session Methods

        /// <summary>
        /// Opens an asynchronous session using the default cluster and its default database.
        /// </summary>
        public IAsyncDocumentSession GetAsyncSession()
        {
            if (DefaultCluster == null)
            {
                var availableClusters = _clusters?.Keys.Any() == true
                    ? string.Join(", ", _clusters.Keys)
                    : "none";
                throw new UnknownClusterException(
                    $"No default cluster has been configured. Available clusters: {availableClusters}. " +
                    $"Set 'DefaultCluster' in RavenManagerOptions or use GetAsyncSession(clusterKey) with an explicit cluster key.");
            }

            return GetStore(DefaultCluster).OpenAsyncSession();
        }

        /// <summary>
        /// Opens an asynchronous session for the specified cluster using its default database.
        /// </summary>
        public IAsyncDocumentSession GetAsyncSession(string clusterKey)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            return GetStore(clusterKey).OpenAsyncSession();
        }

        /// <summary>
        /// Opens an asynchronous session for the specified cluster and database.
        /// </summary>
        public IAsyncDocumentSession GetAsyncSession(string clusterKey, string database)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            return GetStore(clusterKey).OpenAsyncSession(database);
        }

        /// <summary>
        /// Opens an asynchronous session for the default cluster with the specified session options.
        /// </summary>
        public IAsyncDocumentSession GetAsyncSession(SessionOptions sessionOptions)
        {
            if (DefaultCluster == null)
            {
                var availableClusters = _clusters?.Keys.Any() == true
                    ? string.Join(", ", _clusters.Keys)
                    : "none";
                throw new UnknownClusterException(
                    $"No default cluster has been configured. Available clusters: {availableClusters}. " +
                    $"Set 'DefaultCluster' in RavenManagerOptions or use GetAsyncSession(clusterKey, sessionOptions) with an explicit cluster key.");
            }

            return GetStore(DefaultCluster).OpenAsyncSession(sessionOptions);
        }

        /// <summary>
        /// Opens an asynchronous session for the specified cluster with the specified session options.
        /// </summary>
        public IAsyncDocumentSession GetAsyncSession(string clusterKey, SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            return GetStore(clusterKey).OpenAsyncSession(sessionOptions);
        }

        #endregion

        #region Synchronous Session Methods

        /// <summary>
        /// Opens a synchronous session using the default cluster and its default database.
        /// </summary>
        public IDocumentSession GetSession()
        {
            if (DefaultCluster == null)
            {
                var availableClusters = _clusters?.Keys.Any() == true
                    ? string.Join(", ", _clusters.Keys)
                    : "none";
                throw new UnknownClusterException(
                    $"No default cluster has been configured. Available clusters: {availableClusters}. " +
                    $"Set 'DefaultCluster' in RavenManagerOptions or use GetSession(clusterKey) with an explicit cluster key.");
            }

            return GetStore(DefaultCluster).OpenSession();
        }

        /// <summary>
        /// Opens a synchronous session for the specified cluster using its default database.
        /// </summary>
        public IDocumentSession GetSession(string clusterKey)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            return GetStore(clusterKey).OpenSession();
        }

        /// <summary>
        /// Opens a synchronous session for the specified cluster and database.
        /// </summary>
        public IDocumentSession GetSession(string clusterKey, string database)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            return GetStore(clusterKey).OpenSession(database);
        }

        /// <summary>
        /// Opens a synchronous session for the default cluster with the specified session options.
        /// </summary>
        public IDocumentSession GetSession(SessionOptions sessionOptions)
        {
            if (DefaultCluster == null)
            {
                var availableClusters = _clusters?.Keys.Any() == true
                    ? string.Join(", ", _clusters.Keys)
                    : "none";
                throw new UnknownClusterException(
                    $"No default cluster has been configured. Available clusters: {availableClusters}. " +
                    $"Set 'DefaultCluster' in RavenManagerOptions or use GetSession(clusterKey, sessionOptions) with an explicit cluster key.");
            }

            return GetStore(DefaultCluster).OpenSession(sessionOptions);
        }

        /// <summary>
        /// Opens a synchronous session for the specified cluster with the specified session options.
        /// </summary>
        public IDocumentSession GetSession(string clusterKey, SessionOptions sessionOptions)
        {
            ArgumentNullException.ThrowIfNull(clusterKey);

            return GetStore(clusterKey).OpenSession(sessionOptions);
        }

        #endregion

        #region Cluster Management

        /// <summary>
        /// Adds a cluster to the RavenManager.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <param name="storeOptions">The options for the cluster.</param>
        /// <returns>True if the cluster was successfully added; false if a cluster with this key already exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey or storeOptions is null.</exception>
        public bool AddCluster(
            string clusterKey,
            RavenStoreOptions storeOptions)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(clusterKey);

            ArgumentNullException.ThrowIfNull(storeOptions);

            return _clusters.TryAdd(clusterKey, storeOptions);
        }

        /// <summary>
        /// Removes a cluster from the RavenManager and disposes its document store.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <returns>True if the cluster was successfully removed; false if the cluster was not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        public bool RemoveCluster(
            string clusterKey)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(clusterKey);

            // Remove cluster configuration
            var clusterRemoved = _clusters.TryRemove(clusterKey, out _);

            // Try to remove and dispose the store if it was initialized
            if (_stores.TryRemove(clusterKey, out var lazyStore) && lazyStore.IsValueCreated)
            {
                lazyStore.Value?.Dispose();
            }

            return clusterRemoved;
        }

        /// <summary>
        /// Gets all configured cluster keys.
        /// </summary>
        /// <returns>A collection of all cluster keys.</returns>
        public IEnumerable<string> GetClusterKeys()
        {
            ThrowIfDisposed();
            return _clusters.Keys.ToList();
        }

        /// <summary>
        /// Checks if a cluster with the specified key exists.
        /// </summary>
        /// <param name="clusterKey">The cluster key to check.</param>
        /// <returns>True if the cluster exists; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        public bool HasCluster(string clusterKey)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(clusterKey);

            return _clusters.ContainsKey(clusterKey);
        }

        /// <summary>
        /// Checks if a document store has been created for the specified cluster.
        /// </summary>
        /// <param name="clusterKey">The cluster key to check.</param>
        /// <returns>True if the store has been initialized; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if clusterKey is null.</exception>
        public bool IsStoreInitialized(string clusterKey)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(clusterKey);

            return _stores.TryGetValue(clusterKey, out var lazyStore) && lazyStore.IsValueCreated;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Dispose the stores and clusters.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the stores and clusters.
        /// </summary>
        /// <param name="disposing">Whether the class is actually disposing.</param>
        protected virtual void Dispose(
            bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                // Dispose all initialized DocumentStores, collecting any failures
                List<Exception> exceptions = null;
                foreach (var kvp in _stores)
                {
                    if (kvp.Value.IsValueCreated)
                    {
                        try
                        {
                            kvp.Value.Value?.Dispose();
                        }
                        catch (Exception ex)
                        {
                            exceptions ??= new List<Exception>();
                            exceptions.Add(ex);
                        }
                    }
                }
                _stores.Clear();
                _clusters.Clear();

                if (exceptions != null)
                    throw new AggregateException("One or more document stores failed to dispose.", exceptions);
            }
        }

        /// <summary>
        /// Asynchronously dispose the stores and clusters.
        /// </summary>
        public ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;

                // Dispose all initialized DocumentStores, collecting any failures
                List<Exception> exceptions = null;
                foreach (var kvp in _stores)
                {
                    if (kvp.Value.IsValueCreated)
                    {
                        try
                        {
                            kvp.Value.Value?.Dispose();
                        }
                        catch (Exception ex)
                        {
                            exceptions ??= new List<Exception>();
                            exceptions.Add(ex);
                        }
                    }
                }
                _stores.Clear();
                _clusters.Clear();

                if (exceptions != null)
                {
                    GC.SuppressFinalize(this);
                    return ValueTask.FromException(
                        new AggregateException("One or more document stores failed to dispose.", exceptions));
                }
            }

            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        #endregion

        /// <summary>
        /// Creates a new document store for the specified cluster.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to create a document store for.</param>
        /// <returns>A lazily-initialized document store.</returns>
        /// <exception cref="UnknownClusterException">Thrown if the specified cluster is not found.</exception>
        private Lazy<IDocumentStore> CreateDocumentStore(
            string clusterKey)
        {
            if (!_clusters.ContainsKey(clusterKey))
            {
                var availableClusters = _clusters.Keys.Any()
                    ? string.Join(", ", _clusters.Keys)
                    : "none";
                throw new UnknownClusterException(
                    $"Cluster '{clusterKey}' not found. Available clusters: {availableClusters}. " +
                    $"Add the cluster using AddCluster(\"{clusterKey}\", options) in RavenManagerOptions or check your configuration.");
            }

            var cluster = _clusters[clusterKey];
            return new Lazy<IDocumentStore>(() =>
            {
                var store = RavenHelpers.CreateDocumentStore(
                    cluster,
                    DefaultConventions,
                    DefaultCertificateOptions);
                try
                {
                    store.Initialize();
                    return store;
                }
                catch
                {
                    store.Dispose();
                    throw;
                }
            });
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the manager has been disposed.</exception>
        protected void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }
    }
}
