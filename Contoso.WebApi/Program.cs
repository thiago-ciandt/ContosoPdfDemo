using Constoso.Orchestration.Dispatcher;
using Constoso.Orchestration.Interfaces;
using Constoso.Orchestration.Services;
using Contoso.Domain.Interfaces;
using Contoso.Domain.Services;
using Contoso.Orchestration.Interfaces;
using Contoso.Pdf.Interfaces;
using Contoso.Pdf.Services;
using Contoso.Store.Runner;
using Contoso.Tracking.Interfaces;
using Contoso.Tracking.Services;
using Contoso.WebApi.Hubs;
using Contoso.WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Enable MVC with views (not just controllers)
builder.Services.AddControllersWithViews();

// Dependency Injection
builder.Services.AddScoped<IIncident, IncidentService>();

// PDFs
builder.Services.AddScoped<IPdfGenerator, PdfGenerator>();

// SignalR for real-time notifications
builder.Services.AddSignalR();
builder.Services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();
builder.Services.AddScoped<IReportProgressNotifier, ReportProgressNotifier>();
builder.Services.AddScoped<IAdminNotifier, AdminNotifier>();
builder.Services.AddHostedService<ReportWorkerService>();

// Tracker
builder.Services.AddSingleton<IReportJobTracker, InMemoryReportJobTracker>();

// Register mock data as singleton
var incidents = Initialize.CreateIncidentsMock();
builder.Services.AddSingleton(incidents);

var app = builder.Build();

app.MapHub<ReportHub>("/reportHub");
app.MapHub<AdminHub>("/adminHub");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

// Map default MVC route: /Home/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
