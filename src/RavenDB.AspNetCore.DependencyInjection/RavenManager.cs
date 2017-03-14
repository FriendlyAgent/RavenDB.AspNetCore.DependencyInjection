using Microsoft.Extensions.Options;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using System;

namespace RavenDB.AspNetCore.DependencyInjection
{
    public class RavenManager
           : IRavenManager, IDisposable
    {
        private RavenOptions _options;
        private IDocumentStore _cashStore;

        public RavenManager(
            IOptions<RavenOptions> options)
        {
            _options = options.Value;
        }

        public RavenManager()
        {
            _options = new RavenOptions();
        }

        public IDocumentStore GetDefaultStore()
        {
            if (_cashStore == null)
            {
                _cashStore = new DocumentStore()
                {
                    Url = _options.Url != null ?
                        _options.Url : "http://localhost:8080",
                    DefaultDatabase = _options.DefaultDatabase != null ?
                        _options.DefaultDatabase : "",
                    Conventions = _options.Conventions != null ?
                        _options.Conventions : new DocumentConventions(),
                    ApiKey = _options.ApiKey != null ?
                        _options.ApiKey : "",
                };

                _cashStore.Initialize();
            }

            return _cashStore;
        }

        public void Dispose()
        {
            _cashStore = null;
            _options = null;
        }
    }
}