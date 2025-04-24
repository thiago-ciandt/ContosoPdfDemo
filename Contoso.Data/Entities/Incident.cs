namespace Contoso.Store.Entities
{
    public class Incident
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTime CreatedDate { get; set; }

        public ICollection<IncidentAttachment> Attachments { get; set; } = [];
    }
}
