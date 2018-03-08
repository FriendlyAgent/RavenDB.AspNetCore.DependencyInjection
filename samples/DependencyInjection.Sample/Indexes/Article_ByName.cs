using DependencyInjection.Sample.Entities;
using Raven.Client.Documents.Indexes;
using System.Linq;

namespace DependencyInjection.Sample.Indexes
{
    public class Article_ByName
        : AbstractIndexCreationTask<Article>
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
}