using Raven.Client.Documents.Conventions;
using System;
using System.Collections.Concurrent;
using System.Linq;

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
            Clusters = new ConcurrentDictionary<string, RavenStoreOptions>();
        }

        /// <summary>
        /// The cluster key used when no specific cluster is specified.
        /// </summary>
        public string DefaultCluster { get; set; }

        /// <summary>
        /// The conventions used when no specific conventions are specified.
        /// </summary>
        public DocumentConventions DefaultConventions { get; set; }

        /// <summary>
        /// Default certificate configuration for authenticating with RavenDB clusters.
        /// This will be used for all clusters that don't specify their own CertificateOptions.
        /// Supports multiple loading methods: direct object, file path, certificate store, PEM files, or Base64.
        /// </summary>
        public RavenCertificateOptions DefaultCertificateOptions { get; set; }

        /// <summary>
        /// The collection containing all stores. Each store connects to a RavenDB cluster with multiple URLs for high availability.
        /// </summary>
        public ConcurrentDictionary<string, RavenStoreOptions> Clusters { get; set; }

        /// <summary>
        /// Adds a cluster to the cluster collection.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster.</param>
        /// <param name="options">The options used to configure this cluster.</param>
        public void AddCluster(
            string clusterKey,
            RavenStoreOptions options)
        {
            Clusters ??= new ConcurrentDictionary<string, RavenStoreOptions>();

            Clusters.TryAdd(clusterKey, options);
        }
    }
}