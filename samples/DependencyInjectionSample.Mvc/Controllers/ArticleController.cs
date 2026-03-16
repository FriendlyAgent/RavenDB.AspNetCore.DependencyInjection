using DependencyInjectionSample.Mvc.Entities;
using DependencyInjectionSample.Mvc.Indexes;
using DependencyInjectionSample.Mvc.Models.ArticleViewModels;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection;

namespace DependencyInjectionSample.Mvc.Controllers;

public class ArticleController : Controller
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

    public async Task<IActionResult> Index()
    {
        var store = _manager.GetStore();
        var model = new IndexViewModel
        {
            Store = store.Identifier,
            Articles = await _session
                .Query<Article, Article_ByName>()
                .ToListAsync()
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Article article)
    {
        if (!ModelState.IsValid)
            return View(article);

        await _session.StoreAsync(article);
        await _session.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(string id)
    {
        var article = await _session.LoadAsync<Article>(id);
        if (article == null)
            return NotFound();

        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Article article)
    {
        if (!ModelState.IsValid)
            return View(article);

        var existing = await _session.LoadAsync<Article>(id);
        if (existing == null)
            return NotFound();

        existing.Name = article.Name;
        existing.Content = article.Content;
        await _session.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(string id)
    {
        var article = await _session.LoadAsync<Article>(id);
        if (article == null)
            return NotFound();

        return View(article);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var article = await _session.LoadAsync<Article>(id);
        if (article == null)
            return NotFound();

        _session.Delete(article);
        await _session.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
