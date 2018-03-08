using DependencyInjection.Sample.Entities;
using DependencyInjection.Sample.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Indexes;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using System;

namespace Sample.Database.Loader
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Prepairing Database for sample");

            var url = "http://localhost:8080";
            var database = "ravendb-dependency-injection";

            var store = new DocumentStore
            {
                Urls = new[] {
                    url
                },
                Database = database
            };

            store.Initialize();

            var doc = new DatabaseRecord(database);

            store.Maintenance.Server.Send(new CreateDatabaseOperation(doc));

            IndexCreation.CreateIndexes(typeof(Article_ByName).Assembly, store);

            using (BulkInsertOperation bulkInsert = store.BulkInsert())
            {
                for (int i = 0; i < 50; i++)
                {
                    bulkInsert.Store(new Article
                    {
                        Name = "Article #" + i
                    });
                }
            }
        }
    }
}
