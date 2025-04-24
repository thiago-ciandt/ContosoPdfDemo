using Contoso.Tracking.Enums;

namespace Contoso.Tracking.Models
{
    public class ReportJob
    {
        public Guid JobId { get; set; }
        public string ConnectionId { get; set; } = default!;
        public List<Guid> IncidentIds { get; set; } = new();
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? DownloadUrl { get; set; }
        public ReportJobStatus Status { get; set; } = ReportJobStatus.Pending;
        public int Progress { get; set; }
        public string? ErrorMessage { get; set; }
        public TimeSpan? Duration => CompletedAt.HasValue ? CompletedAt - RequestedAt : null;
        public long? FileSizeBytes { get; set; }
        public double? FileSizeMb => FileSizeBytes.HasValue ? FileSizeBytes.Value / 1024d / 1024d : null;
        public DateTime? StartedFinalizationAt { get; set; }

        public TimeSpan? FinalizationDuration =>
            (CompletedAt.HasValue && StartedFinalizationAt.HasValue)
                ? CompletedAt - StartedFinalizationAt
                : null;

        public TimeSpan? GetDuration()
        {
            if (CompletedAt.HasValue)
                return CompletedAt - RequestedAt;

            return null;
        }
    }
}
