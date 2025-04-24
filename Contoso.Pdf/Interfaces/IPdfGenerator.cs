using Contoso.Pdf.Models;

namespace Contoso.Pdf.Interfaces
{
    public interface IPdfGenerator
    {
        string GeneratePdf(List<PdfIncidentModel> incidents);
    }
}
