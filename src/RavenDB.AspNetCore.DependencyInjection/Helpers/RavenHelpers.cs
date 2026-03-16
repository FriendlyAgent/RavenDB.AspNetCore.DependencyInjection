using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using RavenDB.AspNetCore.DependencyInjection.Options;

namespace RavenDB.AspNetCore.DependencyInjection.Helpers
{
    /// <summary>
    /// Helper methods for creating and configuring RavenDB document stores.
    /// </summary>
    public static class RavenHelpers
    {
        /// <summary>
        /// Creates a standard DocumentStore.
        /// </summary>
        /// <param name="options">The store options containing URLs, database, and store-specific settings.</param>
        /// <param name="defaultConventions">Default conventions to use if the store doesn't specify its own.</param>
        /// <param name="defaultCertificateOptions">Default certificate options to use if the store doesn't specify its own.</param>
        /// <returns>A configured DocumentStore ready to be initialized.</returns>
        public static DocumentStore CreateDocumentStore(
            RavenStoreOptions options,
            DocumentConventions defaultConventions = null,
            RavenCertificateOptions defaultCertificateOptions = null)
        {
            var store = new DocumentStore
            {
                Urls = options.Urls,
                Database = options.DefaultDatabase ?? ""
            };

            // Use cluster-specific conventions, otherwise use default conventions
            if (options.Conventions != null)
            {
                store.Conventions = options.Conventions;
            }
            else if (defaultConventions != null)
            {
                store.Conventions = defaultConventions;
            }

            // Load certificate - cluster-specific takes priority over default
            var certificate = options.CertificateOptions?.LoadCertificate()
                ?? defaultCertificateOptions?.LoadCertificate();

            if (certificate != null)
            {
                store.Certificate = certificate;
            }

            return store;
        }
    }
}