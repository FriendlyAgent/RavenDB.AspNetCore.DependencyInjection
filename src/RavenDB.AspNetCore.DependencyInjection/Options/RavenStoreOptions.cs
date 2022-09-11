﻿using Raven.Client.Documents.Conventions;
using System.Security.Cryptography.X509Certificates;

namespace RavenDB.AspNetCore.DependencyInjection.Options
{
    /// <summary>
    /// Represents the options used to configure a RavenDB Store.
    /// </summary>
    public class RavenStoreOptions
    {
        /// <summary>
        /// The url used to connect to a server.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The database used when no specific database is specified.
        /// </summary>
        public string DefaultDatabase { get; set; }

        /// <summary>
        /// The name of the .pfx file used to connect to a secured Raven instance. For example, if mycert.pfx is in /bin, set this to "mycert.pfx"
        /// </summary>
        public string CertificateFileName { get; set; }

        /// <summary>
        /// The pfx certificate data stream used to connect to a secured Raven instance. You can use this property if your certificate is stored into Azure KeyVault.
        /// </summary>
        public string CertificateBase64 { get; set; }

        /// <summary>
        /// The password for the certificate used to connect to a secured Raven instance.
        /// </summary>
        public string CertificatePassword { get; set; }

        /// <summary>
        /// The pfx certificate used to connect to a secured Raven instance. You can use this property if your certificate is stored into Azure KeyVault.
        /// </summary>
        public X509Certificate2 ClientCertificate { get; set; }

        /// <summary>
        /// The conventions used to determine Client API behavior.
        /// </summary>
        public DocumentConventions Conventions { get; set; }
    }
}