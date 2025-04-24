namespace Contoso.Store.Entities
{
    public class IncidentAttachment
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string FileName { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;

        public Incident Incident { get; set; } = default!;
    }
}
