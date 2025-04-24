using Contoso.Tracking.Enums;
using Contoso.Tracking.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.WebApi.Controllers
{
    public class AdminController : Controller
    {
        private readonly IReportJobTracker _tracker;

        public AdminController(IReportJobTracker tracker)
        {
            _tracker = tracker;
        }

        public IActionResult Index()
        {
            var jobs = _tracker.GetAll();

            var durations = jobs
                .Where(j => j.Status == ReportJobStatus.Completed && j.CompletedAt.HasValue)
                .Select(j => j.CompletedAt!.Value - j.RequestedAt)
                .ToList();

            var sizes = jobs
                .Where(j => j.Status == ReportJobStatus.Completed && j.FileSizeBytes.HasValue)
                .Select(j => j.FileSizeMb!.Value)
                .ToList();

            ViewBag.AverageTime = durations.Any()
                ? TimeSpan.FromSeconds(durations.Average(d => d.TotalSeconds))
                : (TimeSpan?)null;

            ViewBag.AverageSizeMb = sizes.Any()
                ? sizes.Average()
                : (double?)null;

            ViewBag.TotalSizeMb = sizes.Sum();

            return View(jobs);
        }
    }
}
