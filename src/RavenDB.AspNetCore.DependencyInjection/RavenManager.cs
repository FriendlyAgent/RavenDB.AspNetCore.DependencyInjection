using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection.Exceptions;
using Sparrow.Collections.LockFree;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public class RavenManager
           : IRavenManager, IDisposable
    {
        public string DefaultServer { get; private set; }
        public DocumentConventions DefaultConventions { get; private set; }

        private bool _disposed;
        private ConcurrentDictionary<string, Lazy<IDocumentStore>> _stores;
        private ConcurrentDictionary<string, RavenServerOptions> _servers;

        public RavenManager(
            IOptions<RavenManagerOptions> options)
        {
            _disposed = false;
            DefaultServer = options.Value.DefaultServer;
            DefaultConventions = options.Value.DefaultConventions != null ?
                options.Value.DefaultConventions : new DocumentConventions();

            _stores = new ConcurrentDictionary<string, Lazy<IDocumentStore>>();
            _servers = options.Value.Servers;
        }

        public IDocumentStore GetStore()
        {
            if (DefaultServer == null)
                throw new ArgumentNullException(nameof(DefaultServer));

            return GetStore(DefaultServer);
        }

        public IDocumentStore GetStore(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_stores));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _stores.GetOrAdd(name, CreateDocumentStore(name)).Value;
        }

        public IAsyncDocumentSession GetAsyncSession()
        {
            if (DefaultServer == null)
                throw new ArgumentNullException(nameof(DefaultServer));

            return GetAsyncSession(new RavenConnection()
            {
                Server = DefaultServer
            });
        }

        public IAsyncDocumentSession GetAsyncSession(
            RavenConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var store = GetStore(connection.Server);
            if (connection.Database == null)
                return store.OpenAsyncSession();
            else
                return store.OpenAsyncSession(connection.Database);
        }

        public IDocumentSession GetSession()
        {
            if (DefaultServer == null)
                throw new ArgumentNullException(nameof(DefaultServer));

            return GetSession(new RavenConnection()
            {
                Server = DefaultServer
            });
        }

        public IDocumentSession GetSession(
            RavenConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            var store = GetStore(connection.Server);
            if (connection.Database == null)
                return store.OpenSession();
            else
                return store.OpenSession(connection.Database);
        }

        private Lazy<IDocumentStore> CreateDocumentStore(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_servers));

            if (!_servers.ContainsKey(name))
                throw new RavenManagerException();

            var server = _servers[name];
            return new Lazy<IDocumentStore>(() =>
            {
                var store = new DocumentStore
                {
                    Url = server.Url,
                    DefaultDatabase = server.DefaultDatabase != null ?
                        server.DefaultDatabase : "",
                    Conventions = server.Conventions != null ?
                        server.Conventions : DefaultConventions,
                    ApiKey = server.ApiKey != null ?
                        server.ApiKey : "",
                };
                store.Initialize();

                return store;
            });
        }

        public bool AddServer(
            string name, RavenServerOptions server)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_servers));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (server == null)
                throw new ArgumentNullException(nameof(server));

            return _servers.TryAdd(name, server);
        }

        public bool RemoveServer(string name)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(_servers));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return (_servers.Remove(name) && _stores.Remove(name));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_stores != null)
                    _stores = null;

                if (_servers != null)
                    _servers = null;

                _disposed = true;
            }
        }
    }
}