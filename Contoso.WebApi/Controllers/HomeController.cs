using Contoso.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.WebApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIncident _incidentService;

        public HomeController(IIncident incidentService)
        {
            _incidentService = incidentService;
        }

        public IActionResult Index(int page = 1)
        {
            const int pageSize = 20;

            var incidents = _incidentService.GetPaged(page, pageSize, out int totalItems);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(incidents);
        }

        public IActionResult Details(Guid id)
        {
            var incident = _incidentService.GetById(id);
            if (incident == null)
                return NotFound();

            return View(incident);
        }
    }
}
