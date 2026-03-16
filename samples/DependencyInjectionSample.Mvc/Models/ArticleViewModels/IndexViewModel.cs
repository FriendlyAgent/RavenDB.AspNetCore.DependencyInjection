using DependencyInjectionSample.Mvc.Entities;

namespace DependencyInjectionSample.Mvc.Models.ArticleViewModels;

public class IndexViewModel
{
    public string Store { get; set; }
    public List<Article> Articles { get; set; } = [];
}
