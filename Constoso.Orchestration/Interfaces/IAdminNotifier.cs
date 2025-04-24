using Contoso.Tracking.Models;

namespace Contoso.Orchestration.Interfaces
{
    public interface IAdminNotifier
    {
        Task NotifyJobUpdate(ReportJob job);
    }
}
