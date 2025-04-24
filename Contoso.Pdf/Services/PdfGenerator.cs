using Contoso.Pdf.Interfaces;
using Contoso.Pdf.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace Contoso.Pdf.Services
{
    public class PdfGenerator : IPdfGenerator
    {
        private readonly string _outputPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports");

        public PdfGenerator()
        {
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);
        }

        public string GeneratePdf(List<PdfIncidentModel> incidents)
        {
            var document = new PdfDocument();

            foreach (var incident in incidents)
            {
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                double y = 40;
                var titleFont = new XFont("Verdana", 16, XFontStyle.Bold);
                var regularFont = new XFont("Verdana", 12, XFontStyle.Regular);
                var imageFont = new XFont("Verdana", 10, XFontStyle.Italic);

                gfx.DrawString(incident.Title, titleFont, XBrushes.Black, new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                y += 30;

                gfx.DrawString($"Created: {incident.CreatedDate:g}", regularFont, XBrushes.Gray, new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                y += 25;

                gfx.DrawString(incident.Description, regularFont, XBrushes.Black, new XRect(20, y, page.Width - 40, 40), XStringFormats.TopLeft);
                y += 60;

                foreach (var image in incident.Images)
                {
                    if (File.Exists(image.Path))
                    {
                        using var img = XImage.FromFile(image.Path);
                        double imgHeight = 200;
                        double spaceNeeded = imgHeight + 60; // Space for title + description

                        if (y + spaceNeeded > page.Height - 50)
                        {
                            page = document.AddPage();
                            gfx = XGraphics.FromPdfPage(page);
                            y = 40;
                        }

                        // Title above image
                        gfx.DrawString(image.Title, regularFont, XBrushes.DarkBlue, new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                        y += 20;

                        // Image
                        gfx.DrawImage(img, 20, y, 300, imgHeight);
                        y += imgHeight + 5;

                        // Description below image
                        gfx.DrawString(image.Description, imageFont, XBrushes.Black, new XRect(20, y, page.Width - 40, 20), XStringFormats.TopLeft);
                        y += 25;
                    }
                }

            }

            var filename = $"incident-report-{Guid.NewGuid()}.pdf";
            var fullPath = Path.Combine(_outputPath, filename);
            document.Save(fullPath);

            return $"/reports/{filename}";
        }
    }
}
