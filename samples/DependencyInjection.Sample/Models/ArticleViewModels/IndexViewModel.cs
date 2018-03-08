using DependencyInjection.Sample.Entities;
using System.Collections.Generic;

namespace DependencyInjection.Sample.Models.ArticleViewModels
{
    public class IndexViewModel
    {
        public string Store { get; set; }

        public List<Article> Articles { get; set; }
            = new List<Article>();
    }
}
