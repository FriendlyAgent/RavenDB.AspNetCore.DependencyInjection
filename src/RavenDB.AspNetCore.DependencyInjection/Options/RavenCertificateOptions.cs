using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace RavenDB.AspNetCore.DependencyInjection.Options
{
    /// <summary>
    /// Flexible certificate configuration options supporting multiple loading methods.
    /// RavenDB uses X.509 certificates for secure authentication with clusters.
    /// </summary>
    public class RavenCertificateOptions
    {
        /// <summary>
        /// Method 1: Direct certificate object (for programmatic configuration).
        /// Use this when you've already loaded the certificate in code.
        /// Example: new X509Certificate2("path/to/cert.pfx", "password")
        /// </summary>
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Method 2: File path to a PFX/P12 certificate file.
        /// Requires CertificatePassword if the file is password-protected.
        /// Example: "certs/ravendb.pfx"
        /// </summary>
        public string CertificatePath { get; set; }

        /// <summary>
        /// Password for the certificate file specified in CertificatePath or CertificateBase64.
        /// </summary>
        public string CertificatePassword { get; set; }

        /// <summary>
        /// Method 3: Certificate thumbprint for loading from the certificate store.
        /// The thumbprint is a unique identifier (SHA-1 hash) of the certificate.
        /// Example: "ABC123DEF456789..."
        /// </summary>
        public string CertificateThumbprint { get; set; }

        /// <summary>
        /// Certificate store name when using CertificateThumbprint.
        /// Defaults to StoreName.My (Personal store).
        /// </summary>
        public StoreName CertificateStoreName { get; set; } = StoreName.My;

        /// <summary>
        /// Certificate store location when using CertificateThumbprint.
        /// Defaults to StoreLocation.CurrentUser.
        /// Use StoreLocation.LocalMachine for system-wide certificates.
        /// </summary>
        public StoreLocation CertificateStoreLocation { get; set; } = StoreLocation.CurrentUser;

        /// <summary>
        /// Method 4: Path to a PEM-encoded certificate file.
        /// Requires CertificateKeyPath for the private key.
        /// Example: "certs/cert.pem"
        /// </summary>
        public string CertificatePemPath { get; set; }

        /// <summary>
        /// Path to the private key file (PEM format) for CertificatePemPath.
        /// Example: "certs/key.pem"
        /// </summary>
        public string CertificateKeyPath { get; set; }

        /// <summary>
        /// Password for the private key if it's encrypted.
        /// Optional - only needed if CertificateKeyPath points to an encrypted key.
        /// </summary>
        public string CertificateKeyPassword { get; set; }

        /// <summary>
        /// Method 5: Base64-encoded certificate (PFX format).
        /// Useful for storing certificates in environment variables or configuration.
        /// Requires CertificatePassword if the certificate is password-protected.
        /// </summary>
        public string CertificateBase64 { get; set; }

        /// <summary>
        /// Loads the certificate using the configured options.
        /// Priority order: Certificate > CertificatePath > CertificateThumbprint > PEM > Base64
        /// </summary>
        /// <returns>The loaded X509Certificate2, or null if no valid configuration is provided.</returns>
        /// <exception cref="InvalidOperationException">Thrown when certificate loading fails.</exception>
        public X509Certificate2 LoadCertificate()
        {
            // Method 1: Direct certificate object
            if (Certificate != null)
                return Certificate;

            // Method 2: File path
            if (!string.IsNullOrEmpty(CertificatePath))
                return LoadFromFile();

            // Method 3: Certificate store
            if (!string.IsNullOrEmpty(CertificateThumbprint))
                return LoadFromStore();

            // Method 4: PEM files
            if (!string.IsNullOrEmpty(CertificatePemPath))
                return LoadFromPem();

            // Method 5: Base64 encoded
            if (!string.IsNullOrEmpty(CertificateBase64))
                return LoadFromBase64();

            return null;
        }

        /// <summary>
        /// Loads certificate from a file (PFX/P12 format).
        /// </summary>
        private X509Certificate2 LoadFromFile()
        {
            try
            {
                if (!File.Exists(CertificatePath))
                {
                    throw new InvalidOperationException(
                        $"Certificate file not found at path: {CertificatePath}");
                }

#if NET9_0_OR_GREATER
                return X509CertificateLoader.LoadPkcs12FromFile(CertificatePath, CertificatePassword);
#else
                return new X509Certificate2(CertificatePath, CertificatePassword);
#endif
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"Failed to load certificate from file '{CertificatePath}'. " +
                    $"Ensure the file exists and the password is correct. Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads certificate from the Windows/macOS certificate store using thumbprint.
        /// </summary>
        private X509Certificate2 LoadFromStore()
        {
            try
            {
                using (var store = new X509Store(CertificateStoreName, CertificateStoreLocation))
                {
                    store.Open(OpenFlags.ReadOnly);

                    var certificates = store.Certificates.Find(
                        X509FindType.FindByThumbprint,
                        CertificateThumbprint,
                        validOnly: false);

                    if (certificates.Count == 0)
                    {
                        throw new InvalidOperationException(
                            $"Certificate with thumbprint '{CertificateThumbprint}' not found in store " +
                            $"'{CertificateStoreName}' at location '{CertificateStoreLocation}'. " +
                            $"Verify the thumbprint and ensure the certificate is installed.");
                    }

                    return certificates[0];
                }
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"Failed to load certificate from store. Thumbprint: {CertificateThumbprint}, " +
                    $"Store: {CertificateStoreName}, Location: {CertificateStoreLocation}. Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads certificate from PEM files (certificate + private key).
        /// </summary>
        private X509Certificate2 LoadFromPem()
        {
            try
            {
                if (!File.Exists(CertificatePemPath))
                {
                    throw new InvalidOperationException(
                        $"Certificate PEM file not found at path: {CertificatePemPath}");
                }

                if (!File.Exists(CertificateKeyPath))
                {
                    throw new InvalidOperationException(
                        $"Certificate key file not found at path: {CertificateKeyPath}");
                }

                var certPem = File.ReadAllText(CertificatePemPath);
                var keyPem = File.ReadAllText(CertificateKeyPath);

                return X509Certificate2.CreateFromPem(certPem, keyPem);
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                throw new InvalidOperationException(
                    $"Failed to load certificate from PEM files. " +
                    $"Cert: {CertificatePemPath}, Key: {CertificateKeyPath}. Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Loads certificate from a Base64-encoded string (PFX format).
        /// </summary>
        private X509Certificate2 LoadFromBase64()
        {
            try
            {
                var certBytes = Convert.FromBase64String(CertificateBase64);

#if NET9_0_OR_GREATER
                return X509CertificateLoader.LoadPkcs12(certBytes, CertificatePassword);
#else
                return new X509Certificate2(certBytes, CertificatePassword);
#endif
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to load certificate from Base64 string. " +
                    $"Ensure the string is valid Base64-encoded PFX data. Error: {ex.Message}", ex);
            }
        }
    }
}
