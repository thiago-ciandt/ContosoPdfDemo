namespace Contoso.WebApi.Models
{
    public class GenerateReportRequest
    {
        public string ConnectionId { get; set; } = default!;
        public List<Guid> IncidentIds { get; set; } = new();
    }
}
