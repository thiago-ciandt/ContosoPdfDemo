using Contoso.Tracking.Models;

namespace Contoso.Tracking.Interfaces
{
    public interface IReportJobTracker
    {
        void AddJob(ReportJob job);
        void UpdateProgress(Guid jobId, int percent);
        void Complete(Guid jobId, string downloadUrl);
        void Fail(Guid jobId, string errorMessage);
        void SetFileSize(Guid jobId, long fileSizeBytes);
        void MarkFinalizationStart(Guid jobId);
        ReportJob GetById(Guid jobId);
        List<ReportJob> GetAll();
    }
}
