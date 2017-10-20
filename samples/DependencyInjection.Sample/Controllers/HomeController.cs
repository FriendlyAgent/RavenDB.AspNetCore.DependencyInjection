using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection;

namespace DependencyInjection.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAsyncDocumentSession _session;
        private readonly IRavenManager _manager;

        public HomeController(
            IAsyncDocumentSession session,
            IRavenManager manager)
        {
            _session = session;
            _manager = manager;
        }

        public async Task<IActionResult> Index()
        {
            await _session.LoadAsync<object>("Test/1");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
