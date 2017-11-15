using Raven.Client.Documents.Conventions;

namespace RavenDB.AspNetCore.DependencyInjection.Options
{
    /// <summary>
    /// Represents the options used to configure a server.
    /// </summary>
    public class RavenServerOptions
    {
        /// <summary>
        /// The url used to connect to a server.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The database used when no specific database is specified.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// The conventions used to determine Client API behavior.
        /// </summary>
        public DocumentConventions Conventions { get; set; }
    }
}