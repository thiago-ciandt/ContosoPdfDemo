using Constoso.Orchestration.Dispatcher;
using Constoso.Orchestration.Events;
using Contoso.Tracking.Enums;
using Contoso.Tracking.Interfaces;
using Contoso.Tracking.Models;
using Contoso.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IEventDispatcher _dispatcher;
        private readonly IReportJobTracker _tracker;

        public ReportController(IEventDispatcher dispatcher, IReportJobTracker tracker)
        {
            _dispatcher = dispatcher;
            _tracker = tracker;
        }

        [HttpPost("generate")]
        public IActionResult GenerateReport([FromBody] GenerateReportRequest request)
        {
            if (request.IncidentIds is null || !request.IncidentIds.Any())
                return BadRequest("At least one incident must be selected.");

            var jobId = Guid.NewGuid();

            var job = new ReportJob
            {
                JobId = jobId,
                ConnectionId = request.ConnectionId,
                IncidentIds = request.IncidentIds,
                RequestedAt = DateTime.UtcNow,
                Status = ReportJobStatus.Pending
            };

            _tracker.AddJob(job);

            _dispatcher.Publish(new ReportRequestedEvent(
                JobId: jobId,
                ConnectionId: request.ConnectionId,
                IncidentIds: request.IncidentIds
            ));

            return Ok(new { jobId });
        }
    }
}
