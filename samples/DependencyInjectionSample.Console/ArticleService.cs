using DependencyInjectionSample.Console.Entities;
using DependencyInjectionSample.Console.Indexes;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace DependencyInjectionSample.Console;

/// <summary>
/// Demonstrates constructor injection of IAsyncDocumentSession in a console application,
/// the same pattern used in ASP.NET Core controllers.
/// </summary>
public class ArticleService
{
    private readonly IAsyncDocumentSession _session;

    public ArticleService(IAsyncDocumentSession session)
    {
        _session = session;
    }

    public async Task<List<Article>> GetAllArticlesAsync()
    {
        return await _session
            .Query<Article, Article_ByName>()
            .ToListAsync();
    }

    public async Task<Article> CreateArticleAsync(string name, string content)
    {
        var article = new Article
        {
            Name = name,
            Content = content
        };

        await _session.StoreAsync(article);
        await _session.SaveChangesAsync();

        return article;
    }
}
