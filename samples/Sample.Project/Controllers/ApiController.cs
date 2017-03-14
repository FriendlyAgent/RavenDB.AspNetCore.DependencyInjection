using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using RavenDB.AspNetCore.DependencyInjection;
using System;

namespace Sample.Project.Controllers
{
    public class ApiController
        : Controller
    {
        private readonly IRavenManager _manager;
        private readonly IAsyncDocumentSession _session;

        public ApiController(
            IRavenManager manager,
            IAsyncDocumentSession session)
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            if (session == null)
                throw new ArgumentNullException(nameof(session));

            _manager = manager;
            _session = session;
        }

        public IActionResult Index()
        {
            return Content("Example site for RavenDB.AspNetCore.DependencyInjection");
        }
    }
}