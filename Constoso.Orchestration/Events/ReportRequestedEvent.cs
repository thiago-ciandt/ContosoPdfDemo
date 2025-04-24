namespace Constoso.Orchestration.Events
{
    public record ReportRequestedEvent(Guid JobId, string ConnectionId, List<Guid> IncidentIds);
}
