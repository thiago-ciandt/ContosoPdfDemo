using Constoso.Orchestration.Dispatcher;
using Constoso.Orchestration.Events;
using Constoso.Orchestration.Interfaces;
using Contoso.Domain.Interfaces;
using Contoso.Orchestration.Interfaces;
using Contoso.Pdf.Interfaces;
using Contoso.Pdf.Models;
using Contoso.Tracking.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace Constoso.Orchestration.Services
{
    public class ReportWorkerService : BackgroundService
    {
        private readonly IEventDispatcher _dispatcher;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly Queue<ReportRequestedEvent> _queue = new();
        private readonly SemaphoreSlim _semaphore = new(5);

        public ReportWorkerService(
            IEventDispatcher dispatcher,
            IServiceScopeFactory scopeFactory)
        {
            _dispatcher = dispatcher;
            _scopeFactory = scopeFactory;

            _dispatcher.Subscribe<ReportRequestedEvent>(OnReportRequested);
        }

        private void OnReportRequested(ReportRequestedEvent request)
        {
            _queue.Enqueue(request);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var request))
                {
                    _ = Task.Run(async () =>
                    {
                        await _semaphore.WaitAsync();
                        try
                        {
                            await ProcessJobAsync(request, stoppingToken);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    });
                }

                await Task.Delay(100, stoppingToken);
            }
        }

        private async Task ProcessJobAsync(ReportRequestedEvent request, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var incidentService = scope.ServiceProvider.GetRequiredService<IIncident>();
            var pdfGenerator = scope.ServiceProvider.GetRequiredService<IPdfGenerator>();
            var notifier = scope.ServiceProvider.GetRequiredService<IReportProgressNotifier>();
            var tracker = scope.ServiceProvider.GetRequiredService<IReportJobTracker>();
            var adminNotifier = scope.ServiceProvider.GetRequiredService<IAdminNotifier>();

            try
            {
                var incidents = incidentService.GetAll()
                    .Where(i => request.IncidentIds.Contains(i.Id))
                    .ToList();

                var models = new List<PdfIncidentModel>();
                int total = incidents.Count;

                for (int i = 0; i < total; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    var incident = incidents[i];

                    models.Add(new PdfIncidentModel
                    {
                        Title = incident.Name,
                        Description = incident.Description,
                        CreatedDate = incident.CreatedDate,
                        Images = incident.Attachments.Select(a => new PdfImageModel
                        {
                            Path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", a.FileName),
                            Title = a.Title,
                            Description = a.Description
                        }).ToList()
                    });

                    int percent = (int)(((double)(i + 1) / total) * 90);
                    await notifier.SendProgressAsync(request.ConnectionId, request.JobId, percent);
                    tracker.UpdateProgress(request.JobId, percent);

                    await adminNotifier.NotifyJobUpdate(tracker.GetById(request.JobId));
                }

                await notifier.SendFinalizingAsync(request.ConnectionId, request.JobId);
                tracker.MarkFinalizationStart(request.JobId);

                if (cancellationToken.IsCancellationRequested)
                    return;

                var url = pdfGenerator.GeneratePdf(models);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", url.TrimStart('/'));

                long fileSize = new FileInfo(filePath).Length;
                tracker.UpdateProgress(request.JobId, 100);
                tracker.Complete(request.JobId, url);
                tracker.SetFileSize(request.JobId, fileSize);
                await adminNotifier.NotifyJobUpdate(tracker.GetById(request.JobId));

                await notifier.SendProgressAsync(request.ConnectionId, request.JobId, 100);
                await notifier.SendCompletedAsync(request.ConnectionId, request.JobId, url);
            }
            catch (Exception ex)
            {
                tracker.Fail(request.JobId, ex.Message);
                await adminNotifier.NotifyJobUpdate(tracker.GetById(request.JobId));
                Console.WriteLine($"Report job failed: {ex.Message}");
            }
        }
    }
}
