using DependencyInjection.Sample.Entities;
using DependencyInjection.Sample.Indexes;
using DependencyInjection.Sample.Models.ArticleViewModels;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection;
using System.Threading.Tasks;

namespace DependencyInjection.Sample.Controllers
{
    public class ArticleController
        : Controller
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRavenManager _manager;

        public ArticleController(
            IAsyncDocumentSession session,
            IRavenManager manager)
        {
            _session = session;
            _manager = manager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var store = _manager.GetStore();
            var model = new IndexViewModel()
            {
                Store = store.Identifier,
                Articles = await _session
                    .Query<Article>(new Article_ByName().IndexName)
                    .ToListAsync()
            };

            return View(model);
        }
    }
}
