using Contoso.Store.Entities;

namespace Contoso.Store.Runner
{
    public static class Initialize
    {
        private const int MaxAttachmentsPerIncident = 20;

        private static readonly string[] IncidentTypes = ["Electrical", "Mechanical", "Safety", "Fire", "Environmental"];
        private static readonly string[] AttachmentTitles = [
            "Overview Image", "Scene Snapshot", "Initial Observation", "Damage Capture", "Evidence Image",
            "Inspection Photo", "Report Header", "Signed Acknowledgement", "Entry Photo", "Conclusion Summary"
        ];

        private static readonly string[] ImageFileNames =
        [
            "dummy_600x400_000000_3fff0a_hello-world.png",
            "dummy_600x400_000000_85e2a6_pdf-demo.png",
            "dummy_600x400_000000_9e8eed_happy-2025.png",
            "dummy_600x400_000000_d596f2_hello-world.png",
            "dummy_600x400_000000_e87aa6.png",
            "dummy_600x400_000000_ea753a_hello-world.png",
            "dummy_600x400_000000_eeff99.png",
            "dummy_600x400_ffffff_122a9e_pdf-demo.png",
            "dummy_600x400_ffffff_142e77_pdf-demo.png",
            "dummy_600x400_ffffff_cccccc.png",
            "incident1.jpg",
            "incident2.jpg",
            "incident3.jpg",
        ];

        public static List<Incident> CreateIncidentsMock(int numberOfRecords = 1000)
        {
            var incidents = new List<Incident>();
            var random = new Random();

            for (int i = 0; i < numberOfRecords; i++)
            {
                var incident = new Incident
                {
                    Id = Guid.NewGuid(),
                    Name = $"Incident #{i + 1}",
                    Description = $"Auto-generated description for Incident #{i + 1}",
                    Type = IncidentTypes[random.Next(IncidentTypes.Length)],
                    CreatedDate = DateTime.Now.AddDays(-random.Next(0, 365)),
                    Attachments = new List<IncidentAttachment>()
                };

                int attachmentsCount = random.Next(1, MaxAttachmentsPerIncident + 1);
                for (int j = 0; j < attachmentsCount; j++)
                {
                    int fileIndex = random.Next(ImageFileNames.Length);
                    var fileName = ImageFileNames[fileIndex];
                    var title = AttachmentTitles[random.Next(AttachmentTitles.Length)];

                    var attachment = new IncidentAttachment
                    {
                        Id = Guid.NewGuid(),
                        CreateDate = incident.CreatedDate.AddHours(random.Next(0, 48)),
                        FileName = fileName,
                        Title = title,
                        Description = $"Auto-generated attachment: {title}",
                        Incident = incident
                    };

                    incident.Attachments.Add(attachment);
                }

                incidents.Add(incident);
            }

            return incidents;
        }
    }
}
