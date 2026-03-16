using DependencyInjectionSample.Console.Entities;
using DependencyInjectionSample.Console.Indexes;
using Raven.Client.Documents;

namespace DependencyInjectionSample.Console.Data;

/// <summary>
/// Initializes the RavenDB database with indexes and seed data.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Creates the database if it doesn't exist, deploys indexes, and seeds test data.
    /// </summary>
    public static async Task InitializeAsync(IDocumentStore store)
    {
        EnsureServerIsReachable(store);

        if (!DatabaseExists(store))
        {
            CreateDatabase(store);
            DeployIndexes(store);
            await SeedTestDataAsync(store);
        }
    }

    private static void EnsureServerIsReachable(IDocumentStore store)
    {
        try
        {
            store.Maintenance.Server.Send(new Raven.Client.ServerWide.Operations.GetBuildNumberOperation());
        }
        catch (Exception ex) when (ex is HttpRequestException or AggregateException)
        {
            var urls = string.Join(", ", store.Urls);
            throw new InvalidOperationException(
                $"Cannot connect to RavenDB at [{urls}]. " +
                $"Make sure RavenDB is running at the configured URL.",
                ex);
        }
    }

    private static bool DatabaseExists(IDocumentStore store)
    {
        var dbNames = store.Maintenance.Server.Send(
            new Raven.Client.ServerWide.Operations.GetDatabaseNamesOperation(0, int.MaxValue));

        return dbNames.Contains(store.Database);
    }

    private static void CreateDatabase(IDocumentStore store)
    {
        var dbRecord = new Raven.Client.ServerWide.DatabaseRecord(store.Database);
        store.Maintenance.Server.Send(
            new Raven.Client.ServerWide.Operations.CreateDatabaseOperation(dbRecord));
    }

    private static void DeployIndexes(IDocumentStore store)
    {
        new Article_ByName().Execute(store);
    }

    private static async Task SeedTestDataAsync(IDocumentStore store)
    {
        using var session = store.OpenAsyncSession();

        await session.StoreAsync(new Article
        {
            Name = "Getting Started with RavenDB",
            Content = "RavenDB is a NoSQL document database for .NET."
        });

        await session.StoreAsync(new Article
        {
            Name = "Dependency Injection in Console Apps",
            Content = "The Generic Host brings the same DI experience from ASP.NET Core to console applications."
        });

        await session.StoreAsync(new Article
        {
            Name = "Using RavenDB with DI",
            Content = "This library makes it easy to register RavenDB sessions in your DI container."
        });

        await session.SaveChangesAsync();
    }
}
