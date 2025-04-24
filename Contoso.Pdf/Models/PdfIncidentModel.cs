namespace Contoso.Pdf.Models
{
    public class PdfIncidentModel
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public List<PdfImageModel> Images { get; set; } = new();
    }

    public class PdfImageModel
    {
        public string Path { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
    }
}
