using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection;

namespace DependencyInjectionWithConfig.Controllers
{
    public class HomeController
        : Controller
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRavenManager _ravenManager;

        public HomeController(
            IAsyncDocumentSession session,
            IRavenManager ravenManager)
        {
            _session = session;
            _ravenManager = ravenManager;
        }

        public IActionResult Index()
        {
            return Content("Demo");
        }
    }
}