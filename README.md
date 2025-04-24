# 📄 Contoso PDF Demo

A full-stack .NET 8 demo project to generate multi-page incident reports in PDF format using a modular architecture. This solution includes background processing, real-time UI updates with SignalR, layered separation of concerns, and a dynamic admin panel for monitoring.

---

## 🧱 Architecture Overview

The application follows a clean separation of concerns and modular design using the following layers:

### 🔷 **Contoso.WebApi**
- Razor views, controllers, and static files
- SignalR integration for real-time progress updates
- Admin dashboard with status monitoring

### 🔷 **Contoso.Orchestration**
- Background workers and event dispatchers
- In-memory queue for handling asynchronous PDF generation
- Fully decoupled event-driven logic

### 🔷 **Contoso.Tracking**
- In-memory job tracker (`IReportJobTracker`)
- Tracks job lifecycle: pending, in-progress, completed, failed
- Provides job state for the Admin panel

### 🔷 **Contoso.Pdf**
- PDF generation logic (`IPdfGenerator`)
- Uses `PdfSharpCore` for drawing images, text and multi-page output
- Saves reports to `wwwroot/reports/`

### 🔷 **Contoso.Store**
- Domain entities: `Incident`, `IncidentAttachment`
- Static mock data generator for incidents with attachments

### 🔷 **Contoso.Domain**
- Interface definitions for domain-level services (e.g., `IIncident`)

---

## 🧪 Features

- [x] Razor Pages UI with Bootstrap styling
- [x] Multi-select incident table with checkboxes
- [x] Background PDF generation with page-by-page progress
- [x] SignalR-based real-time feedback per report job
- [x] Animated floating progress bars (multi-job)
- [x] Download links available once report is ready
- [x] Admin panel with job status overview
- [x] Error tracking and average generation time stats

---

## 🔗 Libraries & Tools Used

### 📦 Back-End
- [.NET 8](https://dotnet.microsoft.com/en-us/)
- [PdfSharpCore](https://github.com/ststeiger/PdfSharpCore) - PDF generation
- [Microsoft.AspNetCore.SignalR](https://learn.microsoft.com/en-us/aspnet/core/signalr/) - Real-time server-client communication
- [Bootstrap 5](https://getbootstrap.com/) - UI framework

### 📦 Front-End
- [jQuery](https://jquery.com/) - DOM manipulation
- [SignalR JS Client](https://www.npmjs.com/package/@microsoft/signalr) - WebSocket client

---

## 🛠️ How It Works

1. User selects multiple `Incidents` and clicks **Generate Report**
2. UI sends `incidentIds` + `connectionId` via `POST /api/report/generate`
3. Server triggers a `ReportRequestedEvent`
4. Background `ReportWorkerService` listens and generates the PDF:
   - Progress updates via `IReportProgressNotifier` → SignalR
   - Status updates via `IReportJobTracker`
5. UI shows real-time animated bars
6. When complete, download link appears and admin sees full job info

---

## 🔍 Future Improvements

- Queue persistence using Redis or Azure Queue
- PDF stylesheets with HTML-to-PDF engines like `WkHtmlToPdf`
- Authentication & authorization for Admin features
- Retry mechanism for failed jobs
- Report history & CSV export
