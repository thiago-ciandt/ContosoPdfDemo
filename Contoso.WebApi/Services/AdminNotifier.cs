using Contoso.Orchestration.Interfaces;
using Contoso.Tracking.Models;
using Contoso.WebApi.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Contoso.WebApi.Services
{
    public class AdminNotifier : IAdminNotifier
    {
        private readonly IHubContext<AdminHub> _hub;

        public AdminNotifier(IHubContext<AdminHub> hub)
        {
            _hub = hub;
        }

        public Task NotifyJobUpdate(ReportJob job)
        {
            return _hub.Clients.All.SendAsync("JobUpdated", job);
        }
    }
}
