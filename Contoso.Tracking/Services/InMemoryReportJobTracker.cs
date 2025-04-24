using Contoso.Tracking.Enums;
using Contoso.Tracking.Interfaces;
using Contoso.Tracking.Models;
using System.Collections.Concurrent;

namespace Contoso.Tracking.Services
{
    public class InMemoryReportJobTracker : IReportJobTracker
    {
        private readonly ConcurrentDictionary<Guid, ReportJob> _jobs = new();

        public void AddJob(ReportJob job) => _jobs.TryAdd(job.JobId, job);

        public void UpdateProgress(Guid jobId, int percent)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = ReportJobStatus.InProgress;
                job.Progress = percent;
            }
        }

        public void Complete(Guid jobId, string downloadUrl)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = ReportJobStatus.Completed;
                job.DownloadUrl = downloadUrl;
                job.CompletedAt = DateTime.UtcNow;
            }
        }

        public void Fail(Guid jobId, string errorMessage)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.Status = ReportJobStatus.Failed;
                job.ErrorMessage = errorMessage;
            }
        }      

        public void SetFileSize(Guid jobId, long fileSizeBytes)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.FileSizeBytes = fileSizeBytes;
            }
        }

        public void MarkFinalizationStart(Guid jobId)
        {
            if (_jobs.TryGetValue(jobId, out var job))
            {
                job.StartedFinalizationAt = DateTime.UtcNow;
            }
        }

        public ReportJob GetById(Guid jobId) => _jobs.Values.FirstOrDefault(j => j.JobId == jobId)!;

        public List<ReportJob> GetAll() => _jobs.Values.OrderByDescending(j => j.RequestedAt).ToList();        
    }
}
