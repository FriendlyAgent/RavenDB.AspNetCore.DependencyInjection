using DependencyInjectionSample.Console;
using DependencyInjectionSample.Console.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RavenDB.AspNetCore.DependencyInjection;

// Build the host with the same DI patterns used in ASP.NET Core
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Register RavenDB using configuration from appsettings.json
        services
            .AddRavenManager(context.Configuration.GetSection("RavenManager"))
            .AddScopedAsyncSession();

        // Register application services
        services.AddScoped<ArticleService>();
    })
    .Build();

// Initialize the database (create DB, deploy indexes, seed data)
var manager = host.Services.GetRequiredService<IRavenManager>();
var store = manager.GetStore();
await DatabaseInitializer.InitializeAsync(store);

// In a console app there is no HTTP request scope, so we create our own.
// Each scope gives you a fresh IAsyncDocumentSession, just like a web request would.
using (var scope = host.Services.CreateScope())
{
    var articleService = scope.ServiceProvider
        .GetRequiredService<ArticleService>();

    // Create a new article
    var newArticle = await articleService.CreateArticleAsync(
        "Console App Article",
        "This article was created from a console application using dependency injection.");

    Console.WriteLine($"Created article: {newArticle.Name} (Id: {newArticle.Id})");
}

// Open a new scope to demonstrate that each scope gets its own session
using (var scope = host.Services.CreateScope())
{
    var articleService = scope.ServiceProvider
        .GetRequiredService<ArticleService>();

    // Query all articles
    var articles = await articleService.GetAllArticlesAsync();

    Console.WriteLine($"\nAll articles ({articles.Count}):");
    foreach (var article in articles)
    {
        Console.WriteLine($"  - {article.Name}");
    }
}
