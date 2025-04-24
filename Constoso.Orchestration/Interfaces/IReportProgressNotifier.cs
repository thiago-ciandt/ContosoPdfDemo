namespace Constoso.Orchestration.Interfaces
{
    public interface IReportProgressNotifier
    {
        Task SendProgressAsync(string connectionId, Guid jobId, int percent);
        Task SendCompletedAsync(string connectionId, Guid jobId, string downloadUrl);
        Task SendFinalizingAsync(string connectionId, Guid jobId);
    }
}
