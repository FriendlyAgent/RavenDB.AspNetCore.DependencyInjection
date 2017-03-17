using Raven.Client.Documents.Conventions;
using Sparrow.Collections.LockFree;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public class RavenManagerOptions
    {

        public RavenManagerOptions()
        {
            DefaultConventions = new DocumentConventions();
            Servers = new ConcurrentDictionary<string, RavenServerOptions>();
        }

        public string DefaultServer { get; set; }

        public DocumentConventions DefaultConventions { get; set; } = new DocumentConventions();

        public ConcurrentDictionary<string, RavenServerOptions> Servers { get; set; } = new ConcurrentDictionary<string, RavenServerOptions>();

        public void AddServer(
            string name,
            RavenServerOptions options)
        {
            if (Servers == null)
                Servers = new ConcurrentDictionary<string, RavenServerOptions>();

            Servers.Add(name, options);
        }
    }
}