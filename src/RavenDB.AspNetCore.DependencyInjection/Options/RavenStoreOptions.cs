using Raven.Client.Documents.Conventions;
using System.Security.Cryptography.X509Certificates;

namespace RavenDB.AspNetCore.DependencyInjection.Options
{
    /// <summary>
    /// Represents the options used to configure a RavenDB Store.
    /// </summary>
    public class RavenStoreOptions
    {
        /// <summary>
        /// The URL used to connect to a server.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The database used when no specific database is specified.
        /// </summary>
        public string DefaultDatabase { get; set; }

        /// <summary>
        /// The certificate used to connect to a secured Raven instance.
        /// </summary>
        public X509Certificate2 DefaultCertificate { get; set; }

        /// <summary>
        /// The password for the certificate used to connect to a secured Raven instance.
        /// </summary>
        public string CertificatePassword { get; set; }

        /// <summary>
        /// The conventions used to determine Client API behavior.
        /// </summary>
        public DocumentConventions Conventions { get; set; }
    }
}