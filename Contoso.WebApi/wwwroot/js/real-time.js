const connection = new signalR.HubConnectionBuilder()
    .withUrl("/reportHub")
    .withAutomaticReconnect()
    .build();

const jobBars = {}; // jobId => DOM elements

connection.start().then(() => {
    console.log("Connected to SignalR:", connection.connectionId);
});

connection.on("ReportProgress", data => {
    updateOrCreateProgressBar(data.jobId, data.percent);
});

connection.on("ReportCompleted", data => {
    completeProgressBar(data.jobId, data.downloadUrl);
});

connection.on("ReportFinalizing", data => {
    const bar = jobBars[data.jobId];
    if (!bar) return;

    const title = bar.querySelector(".report-progress-title");
    const progressBar = bar.querySelector(".progress-bar");

    // Spinner + text
    title.innerHTML = `
        <div class="d-flex align-items-center">
            <div class="spinner-border spinner-border-sm text-primary me-2" role="status"></div>
            <span>Finalizing PDF...</span>
        </div>`;

    // Animate progress bar
    progressBar.classList.add("progress-bar-striped", "progress-bar-animated");
    progressBar.classList.remove("bg-success");
    progressBar.style.width = "95%";
    progressBar.setAttribute("aria-valuenow", 95);
});

// debug
connection.onreconnecting(error => {
    console.warn("SignalR reconnecting...", error);
});

connection.onreconnected(connectionId => {
    console.log("SignalR reconnected! New ID:", connectionId);
});

connection.onclose(error => {
    console.error("SignalR closed:", error);
});

function updateOrCreateProgressBar(jobId, percent) {
    let container = document.getElementById("report-progress-container");
    let bar = jobBars[jobId];

    if (!bar) {
        // Create new progress bar
        bar = document.createElement("div");
        bar.classList.add("report-progress-bar");
        bar.id = `job-${jobId}`;
        bar.innerHTML = `
            <div class="d-flex justify-content-between align-items-start mb-1">
                <div class="report-progress-title">Generating report...</div>
                <button type="button" class="btn-close btn-sm" aria-label="Close" onclick="removeProgressBar('${jobId}')"></button>
            </div>
            <div class="progress mb-2">
                <div class="progress-bar progress-bar-striped progress-bar-animated"
                     role="progressbar" style="width: 0%" aria-valuenow="0"
                     aria-valuemin="0" aria-valuemax="100"></div>
            </div>
            <div class="report-progress-footer text-muted">
                Job ID: ${jobId}
            </div>
        `;
        container.prepend(bar);
        jobBars[jobId] = bar;
    }

    const progressBar = bar.querySelector(".progress-bar");
    progressBar.style.width = `${percent}%`;
    progressBar.setAttribute("aria-valuenow", percent);
}

function completeProgressBar(jobId, downloadUrl) {
    const bar = jobBars[jobId];
    if (!bar) return;

    const title = bar.querySelector(".report-progress-title");
    const progressBar = bar.querySelector(".progress-bar");

    title.innerText = "✅ Report ready!";
    progressBar.classList.remove("progress-bar-animated", "progress-bar-striped");
    progressBar.classList.add("bg-success");
    progressBar.style.width = "100%";
    progressBar.setAttribute("aria-valuenow", 100);

    const footer = bar.querySelector(".report-progress-footer");
    footer.innerHTML = `
        <a class="btn btn-sm btn-success mt-2" href="${downloadUrl}" target="_blank" onclick="removeProgressBar('${jobId}')">
            Download PDF
        </a>`;
}

document.getElementById("generateReport").addEventListener("click", () => {
    const selected = [...document.querySelectorAll("input[name='selectedIncident']:checked")]
        .map(cb => cb.value);

    if (selected.length === 0) {
        alert("Please select at least one incident.");
        return;
    }

    fetch('/api/report/generate', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            connectionId: connection.connectionId,
            incidentIds: selected
        })
    })
        .then(res => res.json())
        .then(data => {
            const jobId = data.jobId;
            console.log("Report generation started:", jobId);
            updateOrCreateProgressBar(jobId, 0);

            document.querySelectorAll("input[name='selectedIncident']:checked").forEach(cb => cb.checked = false);
        })
        .catch(err => {
            console.error("Error sending report request:", err);
            alert("Failed to start report generation.");
        });
});

function removeProgressBar(jobId) {
    const bar = jobBars[jobId];
    if (bar) {
        bar.classList.add("fade-out");

        bar.addEventListener("transitionend", () => {
            bar.remove();
            delete jobBars[jobId];
        }, { once: true });
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const selectAll = document.getElementById("selectAll");
    const checkboxes = document.querySelectorAll("input[name='selectedIncident']");

    if (selectAll) {
        selectAll.addEventListener("change", () => {
            checkboxes.forEach(cb => {
                cb.checked = selectAll.checked;
            });
        });
    }
});