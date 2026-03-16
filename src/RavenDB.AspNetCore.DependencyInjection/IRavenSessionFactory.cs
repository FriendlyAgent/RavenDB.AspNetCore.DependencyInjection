using Raven.Client.Documents.Session;

namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Factory for creating RavenDB sessions on-demand with flexible configuration.
    /// Use this when you need to create multiple sessions with different options within the same scope,
    /// or when you need dynamic control over session creation.
    /// </summary>
    public interface IRavenSessionFactory
    {
        /// <summary>
        /// Creates an asynchronous session using the default cluster and database.
        /// </summary>
        /// <returns>A new asynchronous document session.</returns>
        IAsyncDocumentSession CreateAsyncSession();

        /// <summary>
        /// Creates an asynchronous session for the specified cluster.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <returns>A new asynchronous document session.</returns>
        IAsyncDocumentSession CreateAsyncSession(string clusterKey);

        /// <summary>
        /// Creates an asynchronous session for the specified cluster and database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>A new asynchronous document session.</returns>
        IAsyncDocumentSession CreateAsyncSession(string clusterKey, string database);

        /// <summary>
        /// Creates an asynchronous session with the specified session options.
        /// Uses the default cluster if no cluster is specified in the options.
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>A new asynchronous document session.</returns>
        IAsyncDocumentSession CreateAsyncSession(SessionOptions sessionOptions);

        /// <summary>
        /// Creates an asynchronous session for the specified cluster with session options.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>A new asynchronous document session.</returns>
        IAsyncDocumentSession CreateAsyncSession(string clusterKey, SessionOptions sessionOptions);

        /// <summary>
        /// Creates a synchronous session using the default cluster and database.
        /// </summary>
        /// <returns>A new synchronous document session.</returns>
        IDocumentSession CreateSession();

        /// <summary>
        /// Creates a synchronous session for the specified cluster.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <returns>A new synchronous document session.</returns>
        IDocumentSession CreateSession(string clusterKey);

        /// <summary>
        /// Creates a synchronous session for the specified cluster and database.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="database">The database name to use.</param>
        /// <returns>A new synchronous document session.</returns>
        IDocumentSession CreateSession(string clusterKey, string database);

        /// <summary>
        /// Creates a synchronous session with the specified session options.
        /// Uses the default cluster if no cluster is specified in the options.
        /// </summary>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>A new synchronous document session.</returns>
        IDocumentSession CreateSession(SessionOptions sessionOptions);

        /// <summary>
        /// Creates a synchronous session for the specified cluster with session options.
        /// </summary>
        /// <param name="clusterKey">The key of the cluster to connect to.</param>
        /// <param name="sessionOptions">RavenDB's native SessionOptions for configuration.</param>
        /// <returns>A new synchronous document session.</returns>
        IDocumentSession CreateSession(string clusterKey, SessionOptions sessionOptions);
    }
}
