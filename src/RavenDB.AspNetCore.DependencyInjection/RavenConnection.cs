namespace RavenDB.AspNetCore.DependencyInjection
{
    /// <summary>
    /// Represents The options used to get a specific connection from the manager <see cref="RavenManager"/>.
    /// </summary>
    public class RavenConnection
    {
        /// <summary>
        /// Initializes a new instance of the RavenBuilder class.
        /// </summary>
        public RavenConnection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RavenConnection class with a specified server
        /// </summary>
        /// <param name="server">The url to a server.</param>
        public RavenConnection(
            string server)
            : this(server, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RavenConnection class with a specified server and database
        /// </summary>
        /// <param name="server">The url to a server.</param>
        /// <param name="database">The name of a database.</param>
        public RavenConnection(
            string server,
            string database)
        {
            Database = database;
            Server = server;
        }

        /// <summary>
        /// The database used for the session.
        /// </summary>
        public string Database;

        /// <summary>
        /// The server used for the session.
        /// </summary>
        public string Server;
    }
}