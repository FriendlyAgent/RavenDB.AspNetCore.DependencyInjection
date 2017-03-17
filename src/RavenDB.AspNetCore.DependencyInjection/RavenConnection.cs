namespace RavenDB.AspNetCore.DependencyInjection
{
    public class RavenConnection
    {
        public RavenConnection()
        {

        }

        public RavenConnection(
            string server)
                : this(server, null)
        {

        }

        public RavenConnection(
            string server,
            string database)
        {
            Database = database;
            Server = server;
        }

        public string Database;
        public string Server;
    }
}
