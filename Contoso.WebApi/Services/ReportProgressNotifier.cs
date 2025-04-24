using Constoso.Orchestration.Interfaces;
using Contoso.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Contoso.WebApi.Services
{
    public class ReportProgressNotifier : IReportProgressNotifier
    {
        private readonly IHubContext<ReportHub> _hubContext;

        public ReportProgressNotifier(IHubContext<ReportHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task SendProgressAsync(string connectionId, Guid jobId, int percent)
        {
            return _hubContext.Clients.Client(connectionId)
                .SendAsync("ReportProgress", new { jobId, percent });
        }

        public Task SendCompletedAsync(string connectionId, Guid jobId, string downloadUrl)
        {
            return _hubContext.Clients.Client(connectionId)
                .SendAsync("ReportCompleted", new { jobId, downloadUrl });
        }

        public Task SendFinalizingAsync(string connectionId, Guid jobId)
        {
            return _hubContext.Clients.Client(connectionId)
                .SendAsync("ReportFinalizing", new { jobId });
        }
    }
}
