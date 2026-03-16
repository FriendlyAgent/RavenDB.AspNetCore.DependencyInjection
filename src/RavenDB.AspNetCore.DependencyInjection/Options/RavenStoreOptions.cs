using Raven.Client.Documents.Conventions;
using System.Security.Cryptography.X509Certificates;

namespace RavenDB.AspNetCore.DependencyInjection.Options
{
    /// <summary>
    /// Represents the options used to configure a RavenDB DocumentStore.
    /// A DocumentStore connects to a RavenDB cluster (one or more node URLs) and manages sessions for a specific database.
    /// The RavenDB client automatically handles failover and load balancing across cluster nodes.
    /// </summary>
    public class RavenStoreOptions
    {
        /// <summary>
        /// The URLs of the cluster nodes to connect to. Specify multiple URLs for high availability and automatic failover.
        /// The RavenDB client will automatically handle failover and load balancing across these nodes.
        /// Example: new[] { "https://node1.example.com", "https://node2.example.com", "https://node3.example.com" }
        /// </summary>
        public string[] Urls { get; set; }

        /// <summary>
        /// The default database to use when no specific database is specified in a session request.
        /// If null, you must specify the database name when opening sessions.
        /// </summary>
        public string DefaultDatabase { get; set; }

        /// <summary>
        /// Certificate configuration for authenticating with this specific RavenDB cluster.
        /// If not specified, the DefaultCertificateOptions from RavenManagerOptions will be used.
        /// Supports multiple loading methods: direct object, file path, certificate store, PEM files, or Base64.
        /// </summary>
        public RavenCertificateOptions CertificateOptions { get; set; }

        /// <summary>
        /// Custom document conventions to determine Client API behavior for this specific cluster.
        /// If not specified, the manager's DefaultConventions will be used.
        /// See https://ravendb.net/docs/csharp/client-api/configuration/conventions/what-are-conventions
        /// </summary>
        public DocumentConventions Conventions { get; set; }
    }
}