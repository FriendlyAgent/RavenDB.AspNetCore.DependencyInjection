using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using RavenDB.AspNetCore.DependencyInjection.Options;

namespace RavenDB.AspNetCore.DependencyInjection.Helpers
{
    /// <summary>
    /// Helper for creating the DocumentStore
    /// </summary>
    public static class RavenHelpers
    {
        /// <summary>
        /// Creates a standard DocumetStore.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="defaultConventions"></param>
        /// <returns></returns>
        public static DocumentStore CreateDocumentStore(
            RavenStoreOptions options,
            DocumentConventions defaultConventions = null)
        {
            var store = new DocumentStore
            {
                Urls = options.Urls,
                Database = options.DefaultDatabase ?? ""
            };

            if (options.Conventions == null && defaultConventions != null)
            {
                store.Conventions = defaultConventions;
            }
            else if (options.Conventions != null)
            {
                store.Conventions = options.Conventions;
            }

            if (options.DefaultCertificate != null)
            {
                store.Certificate = options.DefaultCertificate;
            }

            return store;
        }
    }
}