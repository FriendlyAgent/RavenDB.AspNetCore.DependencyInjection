using Raven.Client.Documents.Conventions;
using System.Collections.Concurrent;

namespace RavenDB.AspNetCore.DependencyInjection.Options
{
    /// <summary>
    /// Represents the options used to configure the default manager <see cref="RavenManager"/>.
    /// </summary>
    public class RavenManagerOptions
    {
        /// <summary>
        /// Initializes a new instance of the RavenManagerOptions class.
        /// </summary>
        public RavenManagerOptions()
        {
            Servers = new ConcurrentDictionary<string, RavenStoreOptions>();
        }

        /// <summary>
        /// The server used when no specific server is specified.
        /// </summary>
        public string DefaultServer { get; set; }

        /// <summary>
        /// The conventions used when no specific conventions are specified.
        /// </summary>
        public DocumentConventions DefaultConventions { get; set; }

        /// <summary>
        /// The collection containing all servers used to build stores.
        /// </summary>
        public ConcurrentDictionary<string, RavenStoreOptions> Servers { get; set; }

        /// <summary>
        /// Adds a server to the server collection.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <param name="options">The options used to configure this server.</param>
        public void AddServer(
            string serverName,
            RavenStoreOptions options)
        {
            if (Servers == null)
                Servers = new ConcurrentDictionary<string, RavenStoreOptions>();

            Servers.TryAdd(serverName, options);
        }
    }
}