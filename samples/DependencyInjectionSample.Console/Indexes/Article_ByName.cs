using DependencyInjectionSample.Console.Entities;
using Raven.Client.Documents.Indexes;

namespace DependencyInjectionSample.Console.Indexes;

public class Article_ByName : AbstractIndexCreationTask<Article>
{
    public Article_ByName()
    {
        Map = articles =>
            from article in articles
            select new
            {
                article.Name
            };
    }
}
