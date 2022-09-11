#if NETSTANDARD_2_1 || NETCOREAPP_3_1
using Microsoft.Extensions.Hosting;
#else
    using Microsoft.AspNetCore.Hosting;
#endif

using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace RavenDB.AspNetCore.DependencyInjection.Helpers
{
    public static class RavenHelpers
    {
#if NETSTANDARD_2_1 || NETCOREAPP_3_1
        public static DocumentStore CreateDocumentStore(
            IHostEnvironment host,
            RavenStoreOptions options,
            DocumentConventions defaultConventions = null)
        {
#else
        public static DocumentStore CreateDocumentStore(
            IHostingEnvironment host,
            RavenStoreOptions options, 
            DocumentConventions defaultConventions = null)
        {
#endif
            var store = new DocumentStore
            {
                Urls = new[] { options.Url },

            };

            store.Database = options.DefaultDatabase != null ? options.DefaultDatabase : "";

            if(options.Conventions == null && defaultConventions != null)
            {
                store.Conventions = defaultConventions;
            }
            else if(options.Conventions != null)
            {
                store.Conventions = options.Conventions;
            }

            var hasCert = !string.IsNullOrWhiteSpace(options.CertificateFileName);
            if (hasCert)
            {
                var certFilePath = Path.Combine(host.ContentRootPath, options.CertificateFileName);
                var hasCertPassword = !string.IsNullOrWhiteSpace(options.CertificatePassword);
                if (hasCertPassword)
                {
                    store.Certificate = new X509Certificate2(certFilePath, options.CertificatePassword);
                }
                else
                {
                    store.Certificate = new X509Certificate2(certFilePath);
                }
            }

            return store;
        }
    }
}
