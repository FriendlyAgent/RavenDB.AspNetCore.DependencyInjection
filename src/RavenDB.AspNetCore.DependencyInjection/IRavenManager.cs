using Raven.Client.Documents;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public interface IRavenManager
       : IDisposable
    {
        IDocumentStore GetDefaultStore();
    }
}
