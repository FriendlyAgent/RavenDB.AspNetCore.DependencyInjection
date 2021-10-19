using Microsoft.AspNetCore.Hosting;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using RavenDB.AspNetCore.DependencyInjection.Options;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace RavenDB.AspNetCore.DependencyInjection.Helpers
{
    public static class RavenHelpers
    {
        public static DocumentStore CreateDocumentStore(
            IHostingEnvironment host,
            RavenStoreOptions options, 
            DocumentConventions defaultConventions = null)
        {
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

            var hasFileCert = !string.IsNullOrWhiteSpace(options.CertificateFileName);
            if (hasFileCert)
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

            var hasBase64Cert = !string.IsNullOrWhiteSpace(options.CertificateBase64);
            if (hasBase64Cert)
            {
                var certFileData = Convert.FromBase64String(options.CertificateBase64);
                var hasCertPassword = !string.IsNullOrWhiteSpace(options.CertificatePassword);
                if (hasCertPassword)
                {
                    store.Certificate = new X509Certificate2(certFileData, options.CertificatePassword);
                }
                else
                {
                    store.Certificate = new X509Certificate2(certFileData);
                }
            }

            var has509Certificate = options.ClientCertificate != null;
            if (has509Certificate)
            {
                store.Certificate = options.ClientCertificate;
            }

            return store;
        }
    }
}
